using AppMonederoCommand.Entities.Config;
using AppMonederoCommand.Entities.Usuarios;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;

namespace AppMonederoCommand.Business.BusUsuarios
{
    public class BusUsuario : IBusUsuario
    {
        private readonly ILogger<BusUsuario> _logger;
        private readonly IDatUsuario _datUsuario;
        private readonly IBusMonedero _busMonedero;
        private readonly IBusLenguaje _busLenguaje;
        //Se agrega para crear poder enviar publicación al bus
        private readonly ExchangeConfig _exchangeConfig;
        private readonly IMDRabbitNotifications _rabbitNotifications;
        //Para crear el JWT
        private readonly IBusJwToken _busJwToken;
        //Parametros
        private readonly IBusParametros _busParametros;
        //Llamados Http
        private readonly IAuthService _auth;
        private readonly IServGenerico _servGenerico;
        private readonly string _urlHttpClientMonederoC;
        //Favoritos
        private readonly string _urlCatalogo = Environment.GetEnvironmentVariable("URLBASE_PAQUETES") ?? "";
        //Azure Blob Storage
        private readonly IServAzureBlobStorage _servAzureBlobStorage;
        private readonly string _errorCodeSesion = Environment.GetEnvironmentVariable("ERROR_CODE_SESION") ?? "";
        private readonly IMDParametroConfig _IMDParametroConfig;

        public BusUsuario(ILogger<BusUsuario> logger, IDatUsuario datUusario,
            IServiceProvider serviceProvider, ExchangeConfig exchangeConfig, //Se agrega para crear poder enviar publicación al bus
            IBusJwToken busJwToken, IBusParametros busParametros,
            IAuthService auth, IServGenerico servGenerico, IServAzureBlobStorage servAzureBlobStorage,
            IBusMonedero busMonedero, IBusLenguaje busLenguaje,
            IMDParametroConfig iMDParametroConfig)
        {
            this._logger = logger;
            this._datUsuario = datUusario;
            //Se rellena la variable para poder publicar en el bus
            this._rabbitNotifications = serviceProvider.GetRequiredService<IMDRabbitNotifications>();
            this._exchangeConfig = exchangeConfig;
            this._busJwToken = busJwToken;
            this._busParametros = busParametros;
            this._auth = auth;
            this._servGenerico = servGenerico;
            this._urlHttpClientMonederoC = Environment.GetEnvironmentVariable("MONEDEROC_URL") ?? string.Empty;
            this._servAzureBlobStorage = servAzureBlobStorage;
            _busMonedero = busMonedero;
            _busLenguaje = busLenguaje;
            _IMDParametroConfig = iMDParametroConfig;
        }

        #region Métodos Service Default  

        [IMDMetodo(67823463614746, 67823463613969)]
        public async Task<IMDResponse<EntUsuario>> BGet(Guid uIdUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                response = await _datUsuario.DGet(uIdUsuario);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }
        #endregion

        #region Métodos Service BusUsuario
        //Crear nuevo usuario
        public async Task<IMDResponse<EntUsuarioResponse>> BCreateUsuario(EntCreateUsuario pCreateModel)
        {
            IMDResponse<EntUsuarioResponse> response = new IMDResponse<EntUsuarioResponse>();

            string metodo = nameof(this.BCreateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823461719643, $"Inicia {metodo}(EntCreateUsuario pCreateModel)", pCreateModel, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                IMDResponse<dynamic> httpMonederoC = new IMDResponse<dynamic>();

                //Valida
                var datosCompletos = await BValidaDatosCompletos(pCreateModel);
                if (datosCompletos.HasError)
                {
                    response.ErrorCode = datosCompletos.ErrorCode;
                    response.SetError(datosCompletos.Message);
                    return response;
                }

                var isSocialNetwork = false;
                string sRedSocial = string.Empty;
                string idRedSocial = string.Empty;
                EntRedSocial redesSociales = new EntRedSocial();
                if (!string.IsNullOrEmpty(pCreateModel.uIdRedSocialGoogle) || !string.IsNullOrEmpty(pCreateModel.uIdRedSocialFaceBook) || !string.IsNullOrEmpty(pCreateModel.uIdRedSocialApple))
                {
                    isSocialNetwork = true;
                    if (!string.IsNullOrEmpty(pCreateModel.uIdRedSocialGoogle))
                    {
                        sRedSocial = redesSociales.sRedSocialGoogle;
                        idRedSocial = pCreateModel.uIdRedSocialGoogle;
                    }
                    else if (!string.IsNullOrEmpty(pCreateModel.uIdRedSocialFaceBook))
                    {
                        sRedSocial = redesSociales.sRedSocialFacebook;
                        idRedSocial = pCreateModel.uIdRedSocialFaceBook;
                    }
                    else if (!string.IsNullOrEmpty(pCreateModel.uIdRedSocialApple))
                    {
                        sRedSocial = redesSociales.sRedSocialApple;
                        idRedSocial = pCreateModel.uIdRedSocialApple;
                    }
                }

                //valida estructura de correo
                IMDResponse<bool> correoValido = await BValidaEmail(pCreateModel.sCorreo);
                if (datosCompletos.HasError)
                {
                    response.ErrorCode = correoValido.ErrorCode;
                    response.SetError(correoValido.Message);
                    return response;
                }

                if (!correoValido.Result)
                {
                    if (!isSocialNetwork)
                    {
                        response.SetError(Menssages.BusEmailNoValidExtended);
                        return response;
                    }
                    else
                    {
                        response.SetError(Menssages.BusEmailNoValid);
                        return response;
                    }
                }

                //validacion de correo dispomible
                IMDResponse<EntUsuario> usuarioRegistradoResponse = await BGetByCorreo(pCreateModel.sCorreo);
                EntUsuario? usuarioRegistrado = null;
                if (usuarioRegistradoResponse.HasError)
                {
                    response.ErrorCode = usuarioRegistradoResponse.ErrorCode;
                    response.SetError(usuarioRegistradoResponse.Message);
                    return response;
                }
                else
                {
                    if (usuarioRegistradoResponse.Result == null)
                    {
                        //Validar si el usuario está registrado por red social...
                        if (isSocialNetwork)
                        {
                            usuarioRegistradoResponse = await _datUsuario.DGetByRedSocial(idRedSocial, sRedSocial);
                            if (usuarioRegistradoResponse.HasError)
                            {
                                response.ErrorCode = usuarioRegistradoResponse.ErrorCode;
                                response.SetError(usuarioRegistradoResponse.Message);
                                return response;
                            }
                        }
                    }

                    usuarioRegistrado = usuarioRegistradoResponse.Result;

                    if (usuarioRegistrado != null)
                    {
                        if (!usuarioRegistrado.bActivo)
                        {
                            response.SetError(Menssages.BusEmailInactiveAccount);
                            return response;
                        }

                        if (!isSocialNetwork)
                        {

                            if (usuarioRegistrado.bBaja == false)
                            {
                                if (usuarioRegistrado.bCuentaVerificada == true && usuarioRegistrado.bMigrado == true)
                                {
                                    response.SetError(Menssages.BusEmailInactiveAccount);
                                    return response;
                                }
                            }
                        }
                        else
                        {
                            if (usuarioRegistrado.bBaja == false)
                            {
                                //Validacion de cuenta registrada por red social
                                if (!string.IsNullOrEmpty(pCreateModel.uIdRedSocialGoogle))
                                {
                                    if (usuarioRegistrado!.uIdRedSocialGoogle == idRedSocial)
                                    {
                                        if (usuarioRegistrado.bCuentaVerificada == true && usuarioRegistrado.bMigrado == true)
                                        {
                                            response.SetError($"{Menssages.BusAccountExistSocial} {sRedSocial}");
                                            return response;
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(pCreateModel.uIdRedSocialFaceBook))
                                {
                                    if (usuarioRegistrado!.uIdRedSocialFaceBook == idRedSocial)
                                    {
                                        if (usuarioRegistrado.bCuentaVerificada == true && usuarioRegistrado.bMigrado == true)
                                        {
                                            response.SetError($"{Menssages.BusAccountExistSocial} {sRedSocial}");
                                            return response;
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(pCreateModel.uIdRedSocialApple))
                                {
                                    if (usuarioRegistrado!.uIdRedSocialApple == idRedSocial)
                                    {
                                        if (usuarioRegistrado.bCuentaVerificada == true && usuarioRegistrado.bMigrado == true)
                                        {
                                            response.SetError($"{Menssages.BusAccountExistSocial} {sRedSocial}");
                                            return response;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                IMDResponse<string> codigoVerificacion = await BGeneraCodigoVerificacion();
                EntUsuario entUsuario = new EntUsuario();
                entUsuario.sNombre = pCreateModel.sNombre;
                entUsuario.sApellidoPaterno = pCreateModel.sApellidoPaterno;
                entUsuario.sApellidoMaterno = pCreateModel.sApellidoMaterno!;
                entUsuario.sTelefono = pCreateModel.sTelefono;
                entUsuario.sCorreo = pCreateModel.sCorreo;
                var tempContrasena = await BEncryptData(pCreateModel.sContrasena!, true);
                entUsuario.sContrasena = tempContrasena.Result;
                var tempCodigoVerificacion = await BEncryptData(codigoVerificacion.Result, true);
                entUsuario.sCodigoVerificacion = tempCodigoVerificacion.Result;
                entUsuario.dtFechaNacimiento = pCreateModel.dtFechaNacimiento;
                entUsuario.sCURP = pCreateModel.sCURP;
                entUsuario.cGenero = pCreateModel.cGenero;
                entUsuario.uIdRedSocialGoogle = pCreateModel.uIdRedSocialGoogle;
                entUsuario.sRedSocialGoogle = pCreateModel.sRedSocialGoogle;
                entUsuario.uIdRedSocialFaceBook = pCreateModel.uIdRedSocialFaceBook;
                entUsuario.sRedSocialFaceBook = pCreateModel.sRedSocialFaceBook;
                entUsuario.uIdRedSocialApple = pCreateModel.uIdRedSocialApple;
                entUsuario.sRedSocialApple = pCreateModel.sRedSocialApple;
                entUsuario.sFotografia = pCreateModel.sFotografia;
                entUsuario.bMigrado = false;
                entUsuario.dtFechaCreacion = DateTime.UtcNow;
                entUsuario.bActivo = true;
                entUsuario.bBaja = false;
                entUsuario.sLada = string.IsNullOrEmpty(pCreateModel.sLada) ? "52" : pCreateModel.sLada;
                entUsuario.sIdAplicacion = pCreateModel.sIdAplicacion;
                entUsuario.iEstatusCuenta = (int)eEstatusCuenta.DESBLOQUEADO;

                string sConfirmaContrasenia = pCreateModel.sConfirmaContrasenia ?? string.Empty;
                string sFcmToken = pCreateModel.sFcmToken ?? string.Empty;

                if (usuarioRegistrado == null)
                {
                    IMDResponse<bool> existeTelefono = await BExisteTelefono(pCreateModel.sTelefono);
                    IMDResponse<bool> existeCURP = await BExisteCURP(pCreateModel.sCURP!);
                    IMDResponse<bool> contrasenaValida = await BVerificaContrasena(pCreateModel.sContrasena!, pCreateModel.sConfirmaContrasenia!);

                    if (existeTelefono.Result == false)
                    {
                        if (existeCURP.Result == false)
                        {
                            if (contrasenaValida.Result == true)
                            {

                                /*Guardar usuario*/
                                var resp = await _datUsuario.DSave(entUsuario);

                                if (resp.HasError)
                                {
                                    response.ErrorCode = resp.ErrorCode;
                                    response.SetError(resp.Message);
                                }
                                else
                                {
                                    EntUsuarioResponse entUsuarioResponse = new EntUsuarioResponse();

                                    var parametroReintentosSegundos = await _busParametros.BObtener("ReintentosSegundos");
                                    entUsuarioResponse.sReintentosSegundos = parametroReintentosSegundos.Result.sValor;
                                    var parametroMaximoReintentos = await _busParametros.BObtener("MaxReintentos");
                                    entUsuarioResponse.sMaxReintentos = parametroMaximoReintentos.Result.sValor;

                                    httpMonederoC = await BHttpMonederoC(resp.Result);
                                    if (httpMonederoC.HasError != true)
                                    {

                                        foreach (var monedero in httpMonederoC.Result)
                                        {
                                            var monederoAsigando = await _datUsuario.DUpdateMonederoUsuario(resp.Result.uIdUsuario, monedero.IdMonedero);
                                            if (monederoAsigando.HasError == true)
                                            {
                                                response.ErrorCode = monederoAsigando.ErrorCode;
                                                response.SetError(monederoAsigando.Message);
                                                return response;
                                            }
                                        }
                                    }
                                    //Carga de imagaen
                                    entUsuario.sFotografia = null;//se quita la imagen
                                    if (string.IsNullOrEmpty(entUsuario.sFotografia) == false)
                                    {
                                        EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                                        {
                                            uIdUsuario = resp.Result.uIdUsuario,
                                            sImagen = entUsuario.sFotografia
                                        };

                                        var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                                        if (guardaImagen.HasError)
                                        {
                                            response.ErrorCode = guardaImagen.ErrorCode;
                                            response.SetError(guardaImagen.Message);
                                            return response;
                                        }

                                    }
                                    //Termina Carga Imagen

                                    var firebaseToken = await BGuardaFireBaseToken(resp.Result.uIdUsuario, sFcmToken, pCreateModel.sInfoAppOS, pCreateModel.sIdAplicacion);
                                    if (firebaseToken.Result == true)
                                    {
                                        response.SetCreated(entUsuarioResponse, Menssages.DatCompleteSucces);
                                        //Envío de mensaje SMS

                                        try
                                        {
                                            bool sendCodeByEmail = bool.Parse(_busParametros.BObtener("APP_VERIFICACION_MAIL").Result.Result.sValor ?? "false");
                                            if (sendCodeByEmail)
                                            {
                                                string nombre = $"{entUsuario.sNombre} {entUsuario.sApellidoPaterno} {entUsuario.sApellidoMaterno}";
                                                await EnviarCodigoVerificacionEmail(resp.Result.uIdUsuario, resp.Result.sCorreo!, codigoVerificacion.Result, nombre.Trim());
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(IMDSerializer.Serialize(67823461720420, $"Error en {metodo}(EntCreateUsuario pCreateModel): {ex.Message}", pCreateModel, ex, response));
                                        }

                                        EntBusMessUsuarioCodigoVerificacion busMessage = new EntBusMessUsuarioCodigoVerificacion
                                        {
                                            uIdUsuario = resp.Result.uIdUsuario,
                                            sNumeroTelefono = $"+{entUsuario.sLada}{resp.Result.sTelefono}",
                                            sMensaje = $"{Menssages.BusCodeVerification}: {codigoVerificacion.Result}"
                                        };
                                        ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                                        await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionSms.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessUsuarioCodigoVerificacion>
                                        {
                                            Content = busMessage
                                        });
                                    }
                                    else
                                    {
                                        response.ErrorCode = firebaseToken.ErrorCode;
                                        response.SetError(firebaseToken.Message);
                                    }
                                }
                                /*Termina Guardar usuario*/
                            }
                            else
                            {
                                response.SetError(contrasenaValida.Message);
                            }
                        }
                        else
                        {
                            response.SetError(Menssages.BusCrupNoValid);
                        }

                    }
                    else
                    {
                        response.SetError(Menssages.BusPhoneNoValid);
                    }
                }
                else
                {
                    //Actualiza al usuario solamente
                    var datosCuenta = await _datUsuario.DGetExisteCuenta(entUsuario.sCorreo);

                    //Si  es por red social y esta dado de baja, se reactiva
                    if (isSocialNetwork == true && datosCuenta.HttpCode == HttpStatusCode.NotFound)
                    {
                        //datosCuenta = await _datUsuario.DGetExisteCuenta(entUsuario.sCorreo, isSocialNetwork);
                        EntUpdateUsuarioRedSocial creaUsarioRedSocialBaja = new EntUpdateUsuarioRedSocial
                        {
                            uIdUsuario = usuarioRegistrado.uIdUsuario,
                            sNombre = entUsuario.sNombre,
                            sCorreo = pCreateModel.sCorreo,
                            sApellidoPaterno = entUsuario.sApellidoPaterno,
                            sApellidoMaterno = entUsuario.sApellidoMaterno,
                            sTelefono = entUsuario.sTelefono,
                            dtFechaNacimiento = entUsuario.dtFechaNacimiento,
                            sCURP = entUsuario.sCURP,
                            cGenero = entUsuario.cGenero,
                            sContrasena = entUsuario.sContrasena,
                            sCodigoVerificacion = entUsuario.sCodigoVerificacion,
                            uIdRedSocialGoogle = entUsuario.uIdRedSocialGoogle,
                            sRedSocialGoogle = entUsuario.sRedSocialGoogle,
                            uIdRedSocialFaceBook = entUsuario.uIdRedSocialFaceBook,
                            sRedSocialFaceBook = entUsuario.sRedSocialFaceBook,
                            uIdRedSocialApple = entUsuario.uIdRedSocialApple,
                            sRedSocialApple = entUsuario.sRedSocialApple,
                            sFotografia = entUsuario.sFotografia,
                            uIdUsuarioModificacion = datosCuenta.Result.uIdUsuario,
                            bBaja = false,
                            sLada = entUsuario.sLada,
                            sIdAplicacion = entUsuario.sIdAplicacion

                        };

                        var usuarioActualizado = await BUpdateUsuario(creaUsarioRedSocialBaja);

                        if (usuarioActualizado.HasError != true)
                        {
                            ////Update
                            if (usuarioActualizado.Result.uIdMonedero == null)
                            {
                                httpMonederoC = await BHttpMonederoC(usuarioActualizado.Result);
                                if (httpMonederoC.HasError != true)
                                {
                                    foreach (var monedero in httpMonederoC.Result)
                                    {
                                        var monederoAsigando = await _datUsuario.DUpdateMonederoUsuario(usuarioActualizado.Result.uIdUsuario, monedero.IdMonedero);
                                        if (monederoAsigando.HasError == true)
                                        {
                                            response.ErrorCode = monederoAsigando.ErrorCode;
                                            response.SetError(monederoAsigando.Message);
                                            return response;
                                        }
                                    }
                                }
                            }


                            EntUsuarioResponse usuarioResponse = new EntUsuarioResponse();

                            var parametroReintentosSegundos = await _busParametros.BObtener("ReintentosSegundos");
                            usuarioResponse.sReintentosSegundos = parametroReintentosSegundos.Result.sValor;
                            var parametroMaximoReintentos = await _busParametros.BObtener("MaxReintentos");
                            usuarioResponse.sMaxReintentos = parametroMaximoReintentos.Result.sValor;

                            //Carga de imagaen
                            entUsuario.sFotografia = null;//se quita la imagen
                            if (string.IsNullOrEmpty(entUsuario.sFotografia) == false)
                            {
                                EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                                {
                                    uIdUsuario = datosCuenta.Result.uIdUsuario,
                                    sImagen = entUsuario.sFotografia
                                };

                                var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                                if (guardaImagen.HasError)
                                {
                                    response.ErrorCode = guardaImagen.ErrorCode;
                                    response.SetError(guardaImagen.Message);
                                    return response;
                                }

                            }
                            //Termina Carga Imagen
                            var firebaseToken = await BGuardaFireBaseToken(usuarioActualizado.Result.uIdUsuario, sFcmToken, pCreateModel.sInfoAppOS, pCreateModel.sIdAplicacion);
                            if (firebaseToken.Result == true)
                            {
                                response.SetCreated(usuarioResponse, Menssages.DatCompleteSucces);
                                //Envío de mensaje 

                                try
                                {
                                    bool sendCodeByEmail = bool.Parse(_busParametros.BObtener("APP_VERIFICACION_MAIL").Result.Result.sValor ?? "false");
                                    if (sendCodeByEmail)
                                    {
                                        string nombre = $"{usuarioActualizado.Result.sNombre} {usuarioActualizado.Result.sApellidoPaterno} {usuarioActualizado.Result.sApellidoMaterno}";
                                        await EnviarCodigoVerificacionEmail(usuarioActualizado.Result.uIdUsuario, usuarioActualizado.Result.sCorreo!, codigoVerificacion.Result, nombre);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(IMDSerializer.Serialize(67823461720420, $"Error en {metodo}(EntCreateUsuario pCreateModel): {ex.Message}", pCreateModel, ex, response));
                                }

                                EntBusMessUsuarioCodigoVerificacion busMessage = new EntBusMessUsuarioCodigoVerificacion
                                {
                                    uIdUsuario = usuarioActualizado.Result.uIdUsuario,
                                    sNumeroTelefono = $"+{entUsuario.sLada}{usuarioActualizado.Result.sTelefono}",
                                    sMensaje = $"{Menssages.BusCodeVerification}: {codigoVerificacion.Result}"
                                };
                                ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionSms.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessUsuarioCodigoVerificacion>
                                {
                                    Content = busMessage
                                });
                            }
                            else
                            {
                                response.ErrorCode = firebaseToken.ErrorCode;
                                response.SetError(firebaseToken.Message);
                            }
                            ///Termina Update

                        }
                        else
                        {
                            response.ErrorCode = usuarioActualizado.ErrorCode;
                            response.SetError(usuarioActualizado.Message);
                        }
                    }
                    else
                    {
                        //Si usuario registrado existe, pero esta dado de baja
                        if (usuarioRegistrado != null && usuarioRegistrado.bBaja == true)
                        {
                            entUsuario.uIdUsuario = usuarioRegistrado.uIdUsuario;
                            if (entUsuario.uIdMonedero != null)
                            {
                                entUsuario.uIdMonedero = usuarioRegistrado.uIdMonedero;
                            }

                            var usuarioActualizado = await BUpdateUsuario(entUsuario);


                            if (usuarioActualizado.HasError != true)
                            {
                                ////Update
                                if (usuarioRegistrado.uIdMonedero == null)
                                {
                                    httpMonederoC = await BHttpMonederoC(usuarioActualizado.Result);
                                    if (httpMonederoC.HasError != true)
                                    {

                                        foreach (var monedero in httpMonederoC.Result)
                                        {
                                            var monederoAsigando = await _datUsuario.DUpdateMonederoUsuario(usuarioActualizado.Result.uIdUsuario, monedero.IdMonedero);
                                            if (monederoAsigando.HasError == true)
                                            {
                                                response.ErrorCode = monederoAsigando.ErrorCode;
                                                response.SetError(monederoAsigando.Message);
                                                return response;
                                            }
                                        }
                                    }
                                }

                                EntUsuarioResponse usuarioResponse = new EntUsuarioResponse();

                                var parametroReintentosSegundos = await _busParametros.BObtener("ReintentosSegundos");
                                usuarioResponse.sReintentosSegundos = parametroReintentosSegundos.Result.sValor;
                                var parametroMaximoReintentos = await _busParametros.BObtener("MaxReintentos");
                                usuarioResponse.sMaxReintentos = parametroMaximoReintentos.Result.sValor;

                                //Carga de imagaen
                                entUsuario.sFotografia = null;//se quita la imagen
                                if (string.IsNullOrEmpty(entUsuario.sFotografia) == false)
                                {
                                    EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                                    {
                                        uIdUsuario = usuarioRegistrado.uIdUsuario,
                                        sImagen = entUsuario.sFotografia
                                    };

                                    var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                                    if (guardaImagen.HasError)
                                    {
                                        response.ErrorCode = guardaImagen.ErrorCode;
                                        response.SetError(guardaImagen.Message);
                                        return response;
                                    }

                                }
                                //Termina Carga Imagen
                                var firebaseToken = await BGuardaFireBaseToken(usuarioActualizado.Result.uIdUsuario, sFcmToken, pCreateModel.sInfoAppOS, pCreateModel.sIdAplicacion);
                                if (firebaseToken.Result == true)
                                {
                                    response.SetCreated(usuarioResponse, Menssages.DatCompleteSucces);
                                    //Envío de mensaje 

                                    try
                                    {
                                        bool sendCodeByEmail = bool.Parse(_busParametros.BObtener("APP_VERIFICACION_MAIL").Result.Result.sValor ?? "false");
                                        if (sendCodeByEmail)
                                        {
                                            string nombre = $"{usuarioActualizado.Result.sNombre} {usuarioActualizado.Result.sApellidoPaterno} {usuarioActualizado.Result.sApellidoMaterno}";
                                            await EnviarCodigoVerificacionEmail(usuarioActualizado.Result.uIdUsuario, usuarioActualizado.Result.sCorreo!, codigoVerificacion.Result, nombre.Trim());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(IMDSerializer.Serialize(67823461720420, $"Error en {metodo}(EntCreateUsuario pCreateModel): {ex.Message}", pCreateModel, ex, response));
                                    }

                                    EntBusMessUsuarioCodigoVerificacion busMessage = new EntBusMessUsuarioCodigoVerificacion
                                    {
                                        uIdUsuario = usuarioActualizado.Result.uIdUsuario,
                                        sNumeroTelefono = $"+{entUsuario.sLada}{usuarioActualizado.Result.sTelefono}",
                                        sMensaje = $"{Menssages.BusCodeVerification}: {codigoVerificacion.Result}"
                                    };
                                    ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                                    await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionSms.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessUsuarioCodigoVerificacion>
                                    {
                                        Content = busMessage
                                    });
                                }
                                else
                                {
                                    response.ErrorCode = firebaseToken.ErrorCode;
                                    response.SetError(firebaseToken.Message);
                                }
                                ///Termina Update

                            }
                            else
                            {
                                response.ErrorCode = usuarioActualizado.ErrorCode;
                                response.SetError(usuarioActualizado.Message);
                            }
                        }
                        else
                        {
                            //Si usuario registrado existe, y no esta dado de baja
                            EntUpdateUsarioActivo actualizarUsuarioNoVerificado = new EntUpdateUsarioActivo
                            {
                                uIdUsuario = datosCuenta.Result.uIdUsuario,
                                sNombre = entUsuario.sNombre,
                                sApellidoPaterno = entUsuario.sApellidoPaterno,
                                sApellidoMaterno = entUsuario.sApellidoMaterno,
                                sContrasena = entUsuario.sContrasena,
                                sTelefono = entUsuario.sTelefono,
                                dtFechaNacimiento = entUsuario.dtFechaNacimiento,
                                sCURP = entUsuario.sCURP,
                                cGenero = entUsuario.cGenero,
                                sCodigoVerificacion = entUsuario.sCodigoVerificacion,
                                sFotografia = entUsuario.sFotografia,
                                uIdUsuarioModificacion = datosCuenta.Result.uIdUsuario,
                                bBaja = false,
                                bMigrado = true,
                                sLada = entUsuario.sLada
                            };

                            var usuarioActualizado = await BUpdateUsuario(actualizarUsuarioNoVerificado);


                            if (usuarioActualizado.HasError != true)
                            {

                                EntUsuarioResponse usuarioResponse = new EntUsuarioResponse();

                                var parametroReintentosSegundos = await _busParametros.BObtener("ReintentosSegundos");
                                usuarioResponse.sReintentosSegundos = parametroReintentosSegundos.Result.sValor;
                                var parametroMaximoReintentos = await _busParametros.BObtener("MaxReintentos");
                                usuarioResponse.sMaxReintentos = parametroMaximoReintentos.Result.sValor;

                                //Carga de imagaen
                                entUsuario.sFotografia = null;//se quita la imagen
                                if (string.IsNullOrEmpty(entUsuario.sFotografia) == false)
                                {
                                    EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                                    {
                                        uIdUsuario = datosCuenta.Result.uIdUsuario,
                                        sImagen = entUsuario.sFotografia
                                    };

                                    var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                                    if (guardaImagen.HasError)
                                    {
                                        response.ErrorCode = guardaImagen.ErrorCode;
                                        response.SetError(guardaImagen.Message);
                                        return response;
                                    }

                                }
                                //Termina Carga Imagen
                                var firebaseToken = await BGuardaFireBaseToken(usuarioActualizado.Result.uIdUsuario, sFcmToken, pCreateModel.sInfoAppOS, pCreateModel.sIdAplicacion);
                                if (firebaseToken.Result == true)
                                {
                                    response.SetCreated(usuarioResponse, Menssages.DatCompleteSucces);
                                    //Envío de mensaje 

                                    try
                                    {
                                        bool sendCodeByEmail = bool.Parse(_busParametros.BObtener("APP_VERIFICACION_MAIL").Result.Result.sValor ?? "false");
                                        if (sendCodeByEmail)
                                        {
                                            string nombre = $"{usuarioActualizado.Result.sNombre} {usuarioActualizado.Result.sApellidoPaterno} {usuarioActualizado.Result.sApellidoMaterno}";
                                            await EnviarCodigoVerificacionEmail(usuarioActualizado.Result.uIdUsuario, usuarioActualizado.Result.sCorreo!, codigoVerificacion.Result, nombre.Trim());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(IMDSerializer.Serialize(67823461720420, $"Error en {metodo}(EntCreateUsuario pCreateModel): {ex.Message}", pCreateModel, ex, response));
                                    }

                                    EntBusMessUsuarioCodigoVerificacion busMessage = new EntBusMessUsuarioCodigoVerificacion
                                    {
                                        uIdUsuario = usuarioActualizado.Result.uIdUsuario,
                                        sNumeroTelefono = $"+{entUsuario.sLada}{usuarioActualizado.Result.sTelefono}",
                                        sMensaje = $"{Menssages.BusCodeVerification}: {codigoVerificacion.Result}"
                                    };
                                    ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                                    await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionSms.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessUsuarioCodigoVerificacion>
                                    {
                                        Content = busMessage
                                    });
                                }
                                else
                                {
                                    response.ErrorCode = firebaseToken.ErrorCode;
                                    response.SetError(firebaseToken.Message);
                                }
                                ///Termina Update

                            }
                            else
                            {
                                response.ErrorCode = usuarioActualizado.ErrorCode;
                                response.SetError(usuarioActualizado.Message);
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461720420;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461720420, $"Error en {metodo}(EntCreateUsuario pCreateModel): {ex.Message}", pCreateModel, ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823461719643, $"Termina {metodo}(EntCreateUsuario pCreateModel)", pCreateModel, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        //Login            
        public async Task<IMDResponse<dynamic>> BIniciarSesion(EntLogin pUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BIniciarSesion);
            _logger.LogInformation(IMDSerializer.Serialize(67823462019565, $"Inicia {metodo}(EntLogin pUsuario)", pUsuario));

            try
            {
                bool pTipoLogin = false;
                bool bRequiereCode = false;
                IMDResponse<dynamic> httpMonederoC = new IMDResponse<dynamic>();

                if (string.IsNullOrEmpty(pUsuario.uIdNetwork) && pUsuario.sGrantType.Equals("password"))
                {
                    pTipoLogin = false; //Login normal
                }
                else
                {
                    pTipoLogin = true;//Login por red social
                }


                IMDResponse<EntUsuario> usuario = await _datUsuario.DGetUsuario(pUsuario, pTipoLogin, null);
                if (usuario.HasError)
                {
                    response.ErrorCode = usuario.ErrorCode;
                    response.SetError(usuario.Message);
                    return response;
                }

                EntRedSocial redesSociales = new EntRedSocial();

                //Se valida que el usuario ya este registrado pero si por algun motivo no se autentifico le devuelve un codigo de error controlado para que se procese en la app para se verifique
                if (usuario.Result != null && !usuario.Result.bCuentaVerificada)
                {
                    response.ErrorCode = 302;

                    dynamic entUsuarioResponse = new ExpandoObject();
                    var parametroReintentosSegundos = await _busParametros.BObtener("ReintentosSegundos");
                    entUsuarioResponse.ReintentosSegundos = parametroReintentosSegundos.Result.sValor;
                    var parametroMaximoReintentos = await _busParametros.BObtener("MaxReintentos");
                    entUsuarioResponse.MaxReintentos = parametroMaximoReintentos.Result.sValor;
                    entUsuarioResponse.Telefono = usuario.Result.sTelefono;

                    if (!string.IsNullOrEmpty(usuario.Result.sCorreo))
                    {
                        entUsuarioResponse.Email = usuario.Result.sCorreo;
                    }
                    else
                    {
                        if (redesSociales.sRedSocialGoogle.Equals(pUsuario.sNewtwork))
                        {
                            entUsuarioResponse.Email = usuario.Result.uIdRedSocialGoogle;
                        }
                        else if (redesSociales.sRedSocialFacebook.Equals(pUsuario.sNewtwork))
                        {
                            entUsuarioResponse.Email = usuario.Result.uIdRedSocialFaceBook;
                        }
                        else if (redesSociales.sRedSocialApple.Equals(pUsuario.sNewtwork))
                        {
                            entUsuarioResponse.Email = usuario.Result.uIdRedSocialApple;
                        }
                        else
                        {
                            entUsuarioResponse.Email = string.Empty;
                        }
                    }
                    response.Result = entUsuarioResponse;
                    response.HttpCode = HttpStatusCode.NotFound;

                    response.GetBadRequest(usuario.Message);
                    return response;
                }

                //Verifica existencia de correo e inicio de sesion por red social
                if (usuario.Result == null && pTipoLogin)
                {

                    IMDResponse<EntUsuario> usuarioRegistradoResponse;
                    EntUsuario? usuarioRegistrado = null;
                    usuarioRegistradoResponse = await BGetByCorreo(pUsuario.sUsername);
                    if (usuarioRegistradoResponse.HasError)
                    {
                        response.ErrorCode = usuarioRegistradoResponse.ErrorCode;
                        response.SetError(usuarioRegistradoResponse.Message);
                        return response;
                    }

                    usuarioRegistrado = usuarioRegistradoResponse.Result;
                    if (usuarioRegistrado != null)
                    {
                        if (!usuarioRegistrado.bActivo)
                        {
                            response.SetError(Menssages.BusSessionFailInactive);
                            return response;
                        }

                        if (usuarioRegistrado.bBaja == true)
                        {
                            response.SetNotFound(new EntLoginResponse(), Menssages.BusCredentialsIncorrect);
                            return response;
                        }

                        //Actualiza al usuario solamente
                        EntUpdateUsuario actualizarUsuarioNoVerificado = new EntUpdateUsuario
                        {
                            uIdUsuario = usuarioRegistrado.uIdUsuario,
                            sNombre = usuarioRegistrado.sNombre,
                            sApellidoPaterno = usuarioRegistrado.sApellidoPaterno,
                            sApellidoMaterno = usuarioRegistrado.sApellidoMaterno,
                            sTelefono = usuarioRegistrado.sTelefono,
                            dtFechaNacimiento = usuarioRegistrado.dtFechaNacimiento,
                            sCURP = usuarioRegistrado.sCURP,
                            cGenero = usuarioRegistrado.cGenero,
                            sCodigoVerificacion = usuarioRegistrado.sCodigoVerificacion,
                            uIdRedSocialGoogle = usuarioRegistrado.uIdRedSocialGoogle,
                            sRedSocialGoogle = usuarioRegistrado.sRedSocialGoogle,
                            uIdRedSocialFaceBook = usuarioRegistrado.uIdRedSocialFaceBook,
                            sRedSocialFaceBook = usuarioRegistrado.sRedSocialFaceBook,
                            uIdRedSocialApple = usuarioRegistrado.uIdRedSocialApple,
                            sRedSocialApple = usuarioRegistrado.sRedSocialApple,
                            sFotografia = usuarioRegistrado.sFotografia,
                            uIdUsuarioModificacion = usuarioRegistrado.uIdUsuario,
                            bBaja = false
                        };

                        if (redesSociales.sRedSocialGoogle.Equals(pUsuario.sNewtwork))
                        {
                            actualizarUsuarioNoVerificado.uIdRedSocialGoogle = pUsuario.uIdNetwork;
                            actualizarUsuarioNoVerificado.sRedSocialGoogle = pUsuario.sNewtwork;
                        }
                        if (redesSociales.sRedSocialFacebook.Equals(pUsuario.sNewtwork))
                        {
                            actualizarUsuarioNoVerificado.uIdRedSocialFaceBook = pUsuario.uIdNetwork;
                            actualizarUsuarioNoVerificado.sRedSocialFaceBook = pUsuario.sNewtwork;
                        }
                        if (redesSociales.sRedSocialApple.Equals(pUsuario.sNewtwork))
                        {
                            actualizarUsuarioNoVerificado.uIdRedSocialApple = pUsuario.uIdNetwork;
                            actualizarUsuarioNoVerificado.sRedSocialApple = pUsuario.sNewtwork;
                        }

                        var usuarioActualizado = await BUpdateUsuario(actualizarUsuarioNoVerificado);

                        if (usuarioActualizado.HasError)
                        {
                            response.ErrorCode = usuarioRegistradoResponse.ErrorCode;
                            response.SetError(usuarioRegistradoResponse.Message);
                            return response;
                        }


                        usuario = await _datUsuario.DGetUsuario(pUsuario, pTipoLogin, null);
                        if (usuario.HasError)
                        {
                            response.ErrorCode = usuario.ErrorCode;
                            response.SetError(usuario.Message);
                            return response;
                        }

                        //Descarga de imagen del Azure Blob Storage
                        usuario.Result.sFotografia = "";//Se quita descargar imagen
                        if (!string.IsNullOrEmpty(usuario.Result.sFotografia))
                        {
                            var imageUsuario = await BDescargaImagenPerfil(usuario.Result.uIdUsuario);
                            if (imageUsuario.HasError != true)
                            {
                                usuario.Result.sFotografia = imageUsuario.Result;
                            }
                            else
                            {
                                response.ErrorCode = imageUsuario.ErrorCode;
                                response.SetError(imageUsuario.Message);
                            }
                        }
                        //Termina Descarga de imagen del Azure Blob Storage

                    }
                }

                //Consume servicio seguridad
                if (usuario.Result != null)
                {
                    //Descarga de imagen del Azure Blob Storage
                    usuario.Result.sFotografia = "";//Se quita descargar imagen
                    if (!string.IsNullOrEmpty(usuario.Result.sFotografia))
                    {
                        var imageUsuario = await BDescargaImagenPerfil(usuario.Result.uIdUsuario);
                        if (imageUsuario.HasError != true)
                        {
                            usuario.Result.sFotografia = imageUsuario.Result;
                        }
                        else
                        {
                            response.ErrorCode = imageUsuario.ErrorCode;
                            response.SetError(imageUsuario.Message);
                        }
                    }
                    //Termina Descarga de imagen del Azure Blob Storage
                    var firebaseToken = await BGuardaFireBaseToken(usuario.Result.uIdUsuario, pUsuario.uFcmToken, pUsuario.sInfoAppOS, pUsuario.sIdAplicacion);

                    EntValidaCuenta entValidaCuenta = new EntValidaCuenta()
                    {
                        uIdUsuario = usuario.Result.uIdUsuario,
                        sIdAplicacionBD = usuario.Result.sIdAplicacion,
                        iEstatusCuenta = usuario.Result.iEstatusCuenta,
                        sIdAplicacionRequest = pUsuario.sIdAplicacion,
                        sCorreo = usuario.Result.sCorreo,
                        bTipoLogin = pTipoLogin,
                        bRedSocialGoogle = !usuario.Result.uIdRedSocialGoogle.IsNullOrEmpty()
                    };

                    //Llamada Método
                    response = BValidarCuenta(entValidaCuenta).Result;

                    if (response.Result != null)
                        bRequiereCode = response.Result.bRequiereCode != null ? response.Result.bRequiereCode : false;

                    if (response.ErrorCode == int.Parse(_errorCodeSesion))
                        return response;

                    if (firebaseToken.HasError != true)
                    {
                        var sTokenJWT = await _busJwToken.BGenerarTokenJWT(usuario.Result.uIdUsuario);
                        var sRfreshTokenJWT = await _busJwToken.BGenerarRefreshToken();

                        string tokenJWT = string.Empty;
                        string refreshToken = string.Empty;
                        string fechaExpiraToken = string.Empty;
                        Nullable<DateTime> fechaExpiraRefreshToken;

                        if (sTokenJWT.Result != null)
                        {
                            tokenJWT = sTokenJWT.Result.token;
                            fechaExpiraToken = sTokenJWT.Result.expira;
                        }
                        if (sTokenJWT.Result == null)
                        {
                            return sTokenJWT;
                        }

                        //Refreshtoken

                        if (sRfreshTokenJWT.HasError != true)
                        {
                            refreshToken = sRfreshTokenJWT.Result;
                            //Guardar Historial token
                            var historialGuardado = await _busJwToken.BGuardaHistorialRefreshToken(usuario.Result.uIdUsuario, tokenJWT, refreshToken);

                            if (historialGuardado.HasError == true)
                            {
                                response.ErrorCode = historialGuardado.ErrorCode;
                                response.SetError(historialGuardado.Message);
                                return response;
                            }
                            fechaExpiraRefreshToken = historialGuardado.Result.dtFechaExpiracion;
                            //Termina guardado de historia
                        }
                        else
                        {
                            return sRfreshTokenJWT;
                        }

                        
                            EntLoginResponse responseAutorization = new EntLoginResponse();

                            responseAutorization.entUsuario = new EntLoginUsuario
                            {
                                uIdUsuario = usuario.Result.uIdUsuario,
                                sNombre = usuario.Result.sNombre,
                                sApellidoPaterno = usuario.Result.sApellidoPaterno,
                                sApellidoMaterno = usuario.Result.sApellidoMaterno,
                                sTelefono = usuario.Result.sTelefono,
                                sCorreo = usuario.Result.sCorreo,
                                dtFechaNacimiento = usuario.Result.dtFechaNacimiento,
                                sCURP = usuario.Result.sCURP,
                                cGenero = usuario.Result.cGenero,
                                sFotografia = usuario.Result.sFotografia,
                                uIdMonedero = usuario.Result.uIdMonedero,
                                bMigrado = usuario.Result.bMigrado
                            };

                            responseAutorization.entToken = new EntLoginToken
                            {
                                sAccessToken = tokenJWT,
                                sRefreshToken = refreshToken,
                                dtFechaExpiracion = fechaExpiraRefreshToken

                            };

                            responseAutorization.bRequiereCode = bRequiereCode;

                            /*Crea monedero si y solo si, no tiene monedero asignado */
                            if (usuario.Result.uIdMonedero == null)
                            {
                                httpMonederoC = await BHttpMonederoC(usuario.Result);

                                if (!httpMonederoC.HasError)
                                {

                                    foreach (var monedero in httpMonederoC.Result)
                                    {
                                        responseAutorization.entUsuario.uIdMonedero = monedero.IdMonedero;
                                        responseAutorization.entUsuario.sNoMonedero = monedero.NumMonedero.ToString();
                                        var monederoAsigando = await _datUsuario.DUpdateMonederoUsuario(usuario.Result.uIdUsuario, monedero.IdMonedero);
                                        if (monederoAsigando.HasError == true)
                                        {
                                            response.ErrorCode = monederoAsigando.ErrorCode;
                                            response.SetError(monederoAsigando.Message);
                                            return response;
                                        }
                                    }
                                    response.SetSuccess(responseAutorization, Menssages.DatCompleteSucces);
                                }
                                else
                                {
                                    response.ErrorCode = httpMonederoC.ErrorCode;
                                    response.SetSuccess(responseAutorization, Menssages.BusFailedWallet + httpMonederoC.Message);
                                }
                            }
                            else
                            {
                                //Obtener los datos del monedero...
                                //var token = _auth.BIniciarSesion().Result;
                                var monedero = _busMonedero.BDatosMonedero(Guid.Parse(responseAutorization.entUsuario.uIdMonedero.ToString())).Result;
                                if (!monedero.HasError)
                                {
                                    responseAutorization.entUsuario.sNoMonedero = monedero.Result.numMonedero.ToString();
                                }


                                response.SetSuccess(responseAutorization, Menssages.DatCompleteSucces);
                            }


                    }
                    else
                    {
                        response.ErrorCode = firebaseToken.ErrorCode;
                        response.SetError(firebaseToken.Message);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(pUsuario.sPassword))
                    {
                        var tempContrasena = await BEncryptData(pUsuario.sPassword, false);
                        pUsuario.sPassword = tempContrasena.Result;
                    }
                    //Valida si es un error
                    if (usuario.HasError == true)
                    {
                        response.ErrorCode = usuario.ErrorCode;
                        response.SetError(usuario.Message);
                        return response;
                    }
                    else
                    {
                        //Se valida que si es  un  migradao  
                        if (pTipoLogin == false)
                        {
                            IMDResponse<EntUsuario> usuarioMigrado = await _datUsuario.DGetUsuarioMigrado(pUsuario);

                            if (usuarioMigrado.HasError != true)
                            {
                                if (usuarioMigrado == null)
                                {
                                    response.SetNotFound(new EntLoginResponse(), Menssages.BusVerifiCredentials);
                                    return response;
                                }

                                if ((usuarioMigrado.Result.bBaja ?? true))
                                {
                                    response.SetNotFound(new EntLoginResponse(), Menssages.BusCredentialIncorrect);
                                    return response;
                                }

                                if (!usuarioMigrado.Result.bActivo)
                                {
                                    response.SetNotFound(new EntLoginResponse(), Menssages.BusAccountInactive);
                                    return response;
                                }

                                if ((usuarioMigrado.Result.bMigrado ?? true))
                                {
                                    response.SetNotFound(new EntLoginResponse(), Menssages.BusVerifiCredentials);
                                    return response;
                                }

                                var datosUsuarioMigrado = usuarioMigrado.Result;
                                //Usuario Migrado ingresa por primera vez
                                if (datosUsuarioMigrado.bMigrado == false && datosUsuarioMigrado.dtFechaVencimientoContrasena == null && datosUsuarioMigrado.sContrasena == null)
                                {
                                    //Genera la contraseña, envía el correo y retorna el ErrorCode 301

                                    var contrasenaAleatoria = await BGeneraContrasenaAleatoria();

                                    if (contrasenaAleatoria.HasError != true)
                                    {
                                        string contrasena = contrasenaAleatoria.Result;
                                        var tempContrasena = await BEncryptData(contrasena, true);
                                        var contrasenaInsertada = await _datUsuario.DUpdateContrasenaTemporal(datosUsuarioMigrado.uIdUsuario, tempContrasena.Result);

                                        if (contrasenaInsertada.HasError != true && contrasenaInsertada.Result == true)
                                        {

                                            string plantilla = string.Empty;
                                            try
                                            {
                                                plantilla = _busLenguaje.BusSetLanguajeTemporalPass();
                                                if (string.IsNullOrEmpty(plantilla))
                                                {
                                                    return response;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                response.ErrorCode = 67823462020342;
                                                response.SetError($"{Menssages.BusPlantillaError} {ex}");
                                                _logger.LogError(IMDSerializer.Serialize(67823462020342, $"Error en {metodo}(EntLogin pUsuario): {ex.Message}", pUsuario, ex, response));
                                                return response;
                                            }

                                            string nombre = datosUsuarioMigrado.sNombre + " " + datosUsuarioMigrado.sApellidoPaterno + " " + datosUsuarioMigrado.sApellidoMaterno;
                                            plantilla = plantilla.Replace("{Nombre}", nombre.Trim());
                                            plantilla = plantilla.Replace("{PassTemp}", contrasena);

                                            EntBusMessCorreo busMessage = new EntBusMessCorreo
                                            {
                                                uIdUsuario = datosUsuarioMigrado.uIdUsuario,
                                                sMensaje = plantilla,
                                                sCorreoElectronico = datosUsuarioMigrado.sCorreo,
                                                bHtml = true
                                            };
                                            ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                                            await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                                            {
                                                Content = busMessage
                                            });

                                            response.ErrorCode = 301;
                                            response.SetError(Menssages.BusTemporalPass);
                                            return response;
                                        }
                                        else
                                        {
                                            response.ErrorCode = contrasenaInsertada.ErrorCode;
                                            response.SetError(contrasenaInsertada.Message);
                                            return response;
                                        }
                                    }
                                    else
                                    {
                                        response.ErrorCode = contrasenaAleatoria.ErrorCode;
                                        response.SetError(contrasenaAleatoria.Message);
                                        return response;
                                    }
                                }

                                //Usuario Migrado con contraseña vencida
                                if (datosUsuarioMigrado.bMigrado == false && datosUsuarioMigrado.dtFechaVencimientoContrasena < DateTime.UtcNow)
                                {
                                    //Desactivar los refresh token

                                    var refreshToken = await _busJwToken.BDesactivaRefreshToken(datosUsuarioMigrado.uIdUsuario);

                                    if (refreshToken.HasError == true && refreshToken.Result == null)
                                    {
                                        response.ErrorCode = refreshToken.ErrorCode;
                                        response.SetError(refreshToken.Message);

                                    }
                                    else
                                    {
                                        response.SetError(Menssages.BusTemporalPassExpired);
                                    }

                                    return response;
                                }

                                //Usuario Migrado contraseña vigente
                                if (datosUsuarioMigrado.bMigrado == false && datosUsuarioMigrado.dtFechaVencimientoContrasena > DateTime.UtcNow)
                                {

                                    datosUsuarioMigrado.sFotografia = "";//Se quita descargar imagen
                                    //Descarga de imagen del Azure Blob Storage
                                    if (!string.IsNullOrEmpty(datosUsuarioMigrado.sFotografia))
                                    {
                                        var imageUsuario = await BDescargaImagenPerfil(datosUsuarioMigrado.uIdUsuario);
                                        if (imageUsuario.HasError != true)
                                        {
                                            datosUsuarioMigrado.sFotografia = imageUsuario.Result.sFotografia;
                                        }
                                        else
                                        {
                                            response.ErrorCode = imageUsuario.ErrorCode;
                                            response.SetError(imageUsuario.Message);
                                        }
                                    }
                                    //Termina Descarga de imagen del Azure Blob Storage
                                    //Usuario se logea y se validas
                                    string PCKEY = Environment.GetEnvironmentVariable("PCKEY") ?? "";
                                    string PCIV = Environment.GetEnvironmentVariable("PCIV") ?? "";
                                    pUsuario.sPassword = IMDSecurity.BEncrypt(pUsuario.sPassword!, PCKEY, PCIV);
                                    if (pUsuario.sPassword!.Equals(datosUsuarioMigrado.sContrasena))
                                    {
                                        var firebaseToken = await BGuardaFireBaseToken(datosUsuarioMigrado.uIdUsuario, pUsuario.uFcmToken, pUsuario.sInfoAppOS, pUsuario.sIdAplicacion);

                                        if (firebaseToken.HasError != true)
                                        {
                                            var sTokenJWT = await _busJwToken.BGenerarTokenJWT(datosUsuarioMigrado.uIdUsuario);
                                            var sRfreshTokenJWT = await _busJwToken.BGenerarRefreshToken();

                                            string tokenJWT = string.Empty;
                                            string refreshToken = string.Empty;
                                            string fechaExpiraToken = string.Empty;
                                            Nullable<DateTime> fechaExpiraRefreshToken;

                                            if (sTokenJWT.Result != null)
                                            {
                                                tokenJWT = sTokenJWT.Result.token;
                                                fechaExpiraToken = sTokenJWT.Result.expira;
                                            }
                                            if (sTokenJWT.Result == null)
                                            {
                                                return sTokenJWT;
                                            }

                                            //Refreshtoken
                                            if (sRfreshTokenJWT.HasError != true)
                                            {
                                                refreshToken = sRfreshTokenJWT.Result;
                                                //Guardar Historial token
                                                var historialGuardado = await _busJwToken.BGuardaHistorialRefreshToken(datosUsuarioMigrado.uIdUsuario, tokenJWT, refreshToken);

                                                if (historialGuardado.HasError == true)
                                                {
                                                    response.ErrorCode = historialGuardado.ErrorCode;
                                                    response.SetError(historialGuardado.Message);
                                                    return response;
                                                }
                                                fechaExpiraRefreshToken = historialGuardado.Result.dtFechaExpiracion;
                                                //Termina guardado de historia
                                            }
                                            else
                                            {
                                                return sRfreshTokenJWT;
                                            }

                                            //Termina RefreshToke
                                            EntLoginResponse responseAutorization = new EntLoginResponse();

                                            responseAutorization.entUsuario = new EntLoginUsuario
                                            {
                                                uIdUsuario = datosUsuarioMigrado.uIdUsuario,
                                                sNombre = datosUsuarioMigrado.sNombre,
                                                sApellidoPaterno = datosUsuarioMigrado.sApellidoPaterno,
                                                sApellidoMaterno = datosUsuarioMigrado.sApellidoMaterno,
                                                sTelefono = datosUsuarioMigrado.sTelefono,
                                                sCorreo = datosUsuarioMigrado.sCorreo,
                                                dtFechaNacimiento = datosUsuarioMigrado.dtFechaNacimiento,
                                                sCURP = datosUsuarioMigrado.sCURP,
                                                cGenero = datosUsuarioMigrado.cGenero,
                                                sFotografia = datosUsuarioMigrado.sFotografia,
                                                uIdMonedero = datosUsuarioMigrado.uIdMonedero,
                                                bMigrado = datosUsuarioMigrado.bMigrado
                                            };

                                            responseAutorization.entToken = new EntLoginToken
                                            {
                                                sAccessToken = tokenJWT,
                                                sRefreshToken = refreshToken,
                                                dtFechaExpiracion = fechaExpiraRefreshToken

                                            };

                                            if (datosUsuarioMigrado.uIdMonedero == null)
                                            {
                                                httpMonederoC = await BHttpMonederoC(usuario.Result);

                                                if (httpMonederoC.HasError != true)
                                                {

                                                    foreach (var monedero in httpMonederoC.Result)
                                                    {
                                                        responseAutorization.entUsuario.uIdMonedero = monedero.IdMonedero;
                                                        responseAutorization.entUsuario.sNoMonedero = monedero.NumMonedero.ToString();
                                                        var monederoAsigando = await _datUsuario.DUpdateMonederoUsuario(usuario.Result.uIdUsuario, monedero.IdMonedero);
                                                        if (monederoAsigando.HasError == true)
                                                        {
                                                            response.ErrorCode = monederoAsigando.ErrorCode;
                                                            response.SetError(monederoAsigando.Message);
                                                            return response;
                                                        }
                                                    }

                                                    response.SetSuccess(responseAutorization, Menssages.DatCompleteSucces);
                                                }
                                                else
                                                {
                                                    response.ErrorCode = httpMonederoC.ErrorCode;
                                                    response.SetSuccess(responseAutorization, Menssages.BusFailedWallet + httpMonederoC.Message);
                                                }
                                            }
                                            else
                                            {
                                                //Obtener los datos del monedero...
                                                //var token = _auth.BIniciarSesion().Result;
                                                var monedero = _busMonedero.BDatosMonedero(Guid.Parse(responseAutorization.entUsuario.uIdMonedero.ToString())).Result;
                                                if (!monedero.HasError)
                                                {
                                                    responseAutorization.entUsuario.sNoMonedero = monedero.Result.numMonedero.ToString();
                                                }

                                                response.SetSuccess(responseAutorization, Menssages.DatCompleteSucces);
                                            }
                                        }
                                        else
                                        {
                                            response.ErrorCode = firebaseToken.ErrorCode;
                                            response.SetError(firebaseToken.Message);
                                        }

                                    }
                                    else
                                    {
                                        response.SetNotFound(pUsuario, Menssages.BusVerifiCredentials);
                                    }
                                }

                            }
                            else
                            {
                                response = new IMDResponse<dynamic>
                                {
                                    Result = usuarioMigrado.Result,
                                    HttpCode = usuarioMigrado.HttpCode,
                                    HasError = usuarioMigrado.HasError,
                                    ErrorCode = usuarioMigrado.ErrorCode,
                                    Message = usuarioMigrado.Message
                                };
                            }
                        }
                        else
                        {
                            response.SetNotFound(pUsuario, Menssages.BusVerifiCredentials);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462020342;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462020342, $"Error en {metodo}(EntLogin pUsuario): {ex.Message}", pUsuario, ex, response));
            }
            return response;
        }

        //Login token      
        public async Task<IMDResponse<dynamic>> BIniciaSesionToken(EntRefreshTokenRequest pToken, string sIdAplicacion)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BIniciaSesionToken);
            _logger.LogInformation(IMDSerializer.Serialize(67823462106589, $"Inicia {metodo}(EntRefreshTokenRequest pToken, string sIdAplicacion)", pToken, sIdAplicacion));

            try
            {
                if (!string.IsNullOrEmpty(pToken.sTokenExpirado))
                {
                    if (!string.IsNullOrEmpty(pToken.sRefreshToken))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokenValidaExpirado = tokenHandler.ReadJwtToken(pToken.sTokenExpirado);

                        Guid sUIdUusario = Guid.Parse(tokenValidaExpirado.Claims.First(u =>
                        u.Type == "uIdUsuario"
                        ).Value.ToString());

                        var buscaquedaUsuarios = await _datUsuario.DGetUsuario(sUIdUusario);

                        if (buscaquedaUsuarios.HasError == true)
                        {
                            response.ErrorCode = buscaquedaUsuarios.ErrorCode;
                            response.SetError(buscaquedaUsuarios.Message);
                            return response;
                        }
                        else
                        {
                            var usuarioValido = buscaquedaUsuarios.Result;

                            // Validacion de usuario para poder refrescar el token
                            if (usuarioValido == null)
                            {
                                response.SetNotFound(new EntRefreshTokenRequest(), Menssages.BusUserRefreshTokennoExits);
                                return response;
                            }

                            if (usuarioValido.bBaja == true)
                            {
                                response.SetNotFound(new EntRefreshTokenRequest(), "El Usuario asociado al RefreshToken  se encuentra eliminado.");
                                return response;
                            }
                            if (usuarioValido.bActivo == false)
                            {
                                response.SetNotFound(new EntRefreshTokenRequest(), Menssages.BusUserRefreshTokenInactive);
                                return response;
                            }
                            if (usuarioValido.bMigrado == false)
                            {
                                if (usuarioValido.sContrasena != null && !(DateTime.UtcNow < usuarioValido.dtFechaVencimientoContrasena))
                                {

                                    var refreshToken = await _busJwToken.BDesactivaRefreshToken(usuarioValido.uIdUsuario);

                                    if (refreshToken.HasError == true && refreshToken.Result == null)
                                    {
                                        response.ErrorCode = refreshToken.ErrorCode;
                                        response.SetError(refreshToken.Message);

                                    }
                                    else
                                    {
                                        response.SetError(Menssages.BusPassUserExpired);
                                    }

                                    return response;
                                }
                            }

                            if (sIdAplicacion != null)
                            {
                                if ((usuarioValido.iEstatusCuenta == (int)eEstatusCuenta.BLOQUEADO || usuarioValido.iEstatusCuenta == (int)eEstatusCuenta.DESBLOQUEADO) && usuarioValido.sIdAplicacion != sIdAplicacion)
                                {
                                    response.SetError(Menssages.BusLoginOtherDevice);
                                    response.Result = new EntRefreshTokenResponse();
                                    response.HttpCode = HttpStatusCode.PreconditionFailed;
                                    response.ErrorCode = int.Parse(_errorCodeSesion);
                                    return response;
                                }
                                if (usuarioValido.iEstatusCuenta == (int)eEstatusCuenta.REPORTADO)
                                {
                                    response.SetError(Menssages.BusBlockedAccountApp);
                                    response.Result = new EntRefreshTokenResponse();
                                    response.HttpCode = HttpStatusCode.PreconditionFailed;
                                    response.ErrorCode = int.Parse(_errorCodeSesion);
                                    return response;
                                }
                            }

                            //Si el usuario es valido 
                            var autorizacionResponse = await _busJwToken.BDevolverRefreshToken(pToken, sUIdUusario);
                            if (autorizacionResponse.HasError != true)
                            {
                                var responseIniciasesion = autorizacionResponse.Result;

                                EntRefreshTokenResponse tokenActualizado = new EntRefreshTokenResponse
                                {
                                    sAccessToken = responseIniciasesion.sToken,
                                    sRefreshToken = responseIniciasesion.sRefreshToken,
                                    dtFechaExpiracion = responseIniciasesion.dtFechaExpiracion
                                };

                                response.SetSuccess(tokenActualizado, Menssages.BusSuccesss);
                            }
                            else
                            {
                                response = autorizacionResponse;
                            }
                        }

                    }
                    else
                    {
                        response.SetError(Menssages.BusRequiredRefreshToken);
                        response.HttpCode = HttpStatusCode.Unauthorized;
                    }
                }
                else
                {
                    response.SetError(Menssages.BusRequiredTokenExpired);
                    response.HttpCode = HttpStatusCode.Unauthorized;
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462107366;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462107366, $"Error en {metodo}(EntRefreshTokenRequest pToken, string sIdAplicacion): {ex.Message}", pToken, sIdAplicacion, ex, response));
            }
            return response;
        }

        //Verifica Usuario        
        public async Task<IMDResponse<dynamic>> BVerificaCodigo(EntCodigoVerificacion codigo)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BVerificaCodigo);
            _logger.LogInformation(IMDSerializer.Serialize(67823462143885, $"Inicia {metodo}(EntCodigoVerificacion codigo)", codigo));

            try
            {

                var existeUsuario = await _datUsuario.DGetValidaUsuario(codigo);

                if (existeUsuario.Result != false)
                {
                    var codigoValido = await _datUsuario.DGetCodigoValido(codigo);
                    if (codigoValido.HasError != true)
                    {
                        if (!string.IsNullOrEmpty(codigoValido.Message))
                        {
                            //Inicia codigo valida
                            var usuarioActualizado = await _datUsuario.DUpdateUsuarioVerificado(codigo);
                            if (usuarioActualizado.HasError != true)
                            {
                                EntCodigoResponse entCodigoResponse = new EntCodigoResponse();

                                var entUsuario = usuarioActualizado.Result;
                                if (entUsuario != null)
                                {

                                    var sTokenJWT = await _busJwToken.BGenerarTokenJWT(entUsuario.uIdUsuario);
                                    var sRfreshTokenJWT = await _busJwToken.BGenerarRefreshToken();

                                    string tokenJWT = string.Empty;
                                    string refreshToken = string.Empty;
                                    string fechaExpiraToken = string.Empty;
                                    Nullable<DateTime> fechaExpiraRefreshToken;


                                    if (sTokenJWT.HasError == false)
                                    {
                                        tokenJWT = sTokenJWT.Result.token;
                                        fechaExpiraToken = sTokenJWT.Result.expira;
                                    }
                                    else
                                    {
                                        return sTokenJWT;
                                    }


                                    //Refreshtoken

                                    if (sRfreshTokenJWT.HasError != true)
                                    {
                                        refreshToken = sRfreshTokenJWT.Result;
                                        //Guardar Historial token
                                        var historialGuardado = await _busJwToken.BGuardaHistorialRefreshToken(entUsuario.uIdUsuario, tokenJWT, refreshToken);

                                        if (historialGuardado.HasError == true)
                                        {
                                            response.ErrorCode = historialGuardado.ErrorCode;
                                            response.SetError(historialGuardado.Message);
                                            return response;
                                        }
                                        fechaExpiraRefreshToken = historialGuardado.Result.dtFechaExpiracion;
                                        //Termina guardado de historia


                                    }
                                    else
                                    {
                                        response = sRfreshTokenJWT;
                                        return response;
                                    }

                                    //Termina RefreshToke
                                    //Favoritos

                                    //Termina favoritos
                                    EntTokenCodigoVerificacion token = new EntTokenCodigoVerificacion
                                    {
                                        sAccessToken = tokenJWT,
                                        sRefreshToken = refreshToken,
                                        dtFechaExpiracion = fechaExpiraRefreshToken
                                    };

                                    entCodigoResponse.entToken = token;
                                    //Descarga de imagen del Azure Blob Storage
                                    entUsuario.sFotografia = "";//Se quita descargar imagen
                                    if (!string.IsNullOrEmpty(entUsuario.sFotografia))
                                    {
                                        var imageUsuario = await BDescargaImagenPerfil(entUsuario.uIdUsuario);
                                        if (imageUsuario.HasError != true)
                                        {
                                            entUsuario.sFotografia = imageUsuario.Result;
                                        }
                                        else
                                        {
                                            response.ErrorCode = imageUsuario.ErrorCode;
                                            response.SetError(imageUsuario.Message);
                                        }
                                    }
                                    //Termina Descarga de imagen del Azure Blob Storage
                                    entCodigoResponse.entUsuario = new EntUsuarioCodigoVerificacion
                                    {
                                        uIdUsuario = entUsuario.uIdUsuario,
                                        sNombre = entUsuario.sNombre,
                                        sApellidoPaterno = entUsuario.sApellidoPaterno,
                                        sApellidoMaterno = entUsuario.sApellidoMaterno,
                                        sTelefono = entUsuario.sTelefono,
                                        sCorreo = entUsuario.sCorreo,
                                        dtFechaNacimiento = entUsuario.dtFechaNacimiento,
                                        sCURP = entUsuario.sCURP,
                                        cGenero = entUsuario.cGenero,
                                        sFotografia = entUsuario.sFotografia,
                                        uIdMonedero = entUsuario.uIdMonedero
                                    };

                                    if (sTokenJWT.Result != null)
                                    {

                                        response.SetSuccess(entCodigoResponse);
                                        #region Enviar correo
                                        try
                                        {
                                            string plantilla = string.Empty;

                                            try
                                            {
                                                plantilla = _busLenguaje.BusSetLanguajeBienvenido();
                                                if (string.IsNullOrEmpty(plantilla))
                                                {
                                                    return response;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogError(IMDSerializer.Serialize(67823462144662, $"Error en {metodo}(EntCodigoVerificacion codigo): {ex.Message}", codigo, ex, response));
                                                return response;
                                            }

                                            string nombre = entUsuario.sNombre + " " + entUsuario.sApellidoPaterno + " " + entUsuario.sApellidoMaterno;
                                            plantilla = plantilla.Replace("{Nombre}", nombre.Trim());

                                            string badge_googleplay = _busParametros.BObtener("APP_BADGE_GOOGLE_PLAY").Result.Result.sValor ?? "";
                                            string badge_appstore = _busParametros.BObtener("APP_BADGE_APP_STORE").Result.Result.sValor ?? "";

                                            plantilla = plantilla.Replace("{badge_google_play}", badge_googleplay);
                                            plantilla = plantilla.Replace("{badge_app_store}", badge_appstore);

                                            EntBusMessCorreo busMessage = new EntBusMessCorreo
                                            {
                                                uIdUsuario = entUsuario.uIdUsuario,
                                                sMensaje = plantilla,
                                                sCorreoElectronico = entUsuario.sCorreo,
                                                bHtml = true
                                            };
                                            ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                                            await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                                            {
                                                Content = busMessage
                                            });
                                        }
                                        catch { }
                                        #endregion
                                    }
                                    else
                                    {
                                        response.SetError(Menssages.BusNoAddCorrectToken);
                                    }
                                }
                                else
                                {
                                    response.SetError(Menssages.BusUserNoVerify);
                                }
                            }
                            else
                            {
                                response.ErrorCode = usuarioActualizado.ErrorCode;
                                response.SetError(usuarioActualizado.Message);
                            }
                            //Termina codigo valida
                        }
                        else
                        {
                            response.SetNotFound(codigo);
                        }
                    }
                    else
                    {
                        response.ErrorCode = codigoValido.ErrorCode;
                        response.SetError(codigoValido.Message);
                    }
                }
                else
                {
                    response.SetNotFound(codigo);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462144662;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462144662, $"Error en {metodo}(EntCodigoVerificacion codigo): {ex.Message}", codigo, ex, response));
            }
            return response;
        }

        //Reenvia codigo de verificación        
        public async Task<IMDResponse<dynamic>> BReenviarCodigo(EntReenviaCodigo solicitud)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BReenviarCodigo);
            _logger.LogInformation(IMDSerializer.Serialize(67823462156317, $"Inicia {metodo}(EntReenviaCodigo solicitud)", solicitud));

            try
            {

                //Genera nuevo codigo
                IMDResponse<string> generaCodigo = await BGeneraCodigoVerificacion();

                var codigo = await BEncryptData(generaCodigo.Result, true);
                string codgioEncrypt = codigo.Result;

                //actualiza  codigo encriptado 
                var existeCuenta = await _datUsuario.DGetExisteCuenta(solicitud.sCorreo);

                if (existeCuenta.HasError != true)
                {
                    var codigoActualizado = await _datUsuario.DUpdateCodigoVerificacion(solicitud, codgioEncrypt);

                    if (codigoActualizado.Result == true)
                    {
                        try
                        {
                            bool sendCodeByEmail = bool.Parse(_busParametros.BObtener("APP_VERIFICACION_MAIL").Result.Result.sValor ?? "false");
                            if (sendCodeByEmail)
                            {
                                string nombre = $"{existeCuenta.Result.sNombre} {existeCuenta.Result.sApellidoPaterno} {existeCuenta.Result.sApellidoMaterno}";
                                await EnviarCodigoVerificacionEmail(existeCuenta.Result.uIdUsuario, existeCuenta.Result.sCorreo!, generaCodigo.Result, nombre.Trim());
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(IMDSerializer.Serialize(67823462157094, $"Error en {metodo}(EntReenviaCodigo solicitud): {ex.Message}", solicitud, ex, response));
                        }

                        //Envío de mensaje 
                        EntBusMessUsuarioCodigoVerificacion busMessage = new EntBusMessUsuarioCodigoVerificacion
                        {
                            uIdUsuario = existeCuenta.Result.uIdUsuario,
                            sNumeroTelefono = $"+52{existeCuenta.Result.sTelefono}",
                            sMensaje = $"{Menssages.BusCodeVerification}: {generaCodigo.Result}"
                        };
                        ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                        await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionSms.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessUsuarioCodigoVerificacion>
                        {
                            Content = busMessage
                        });

                        response.SetSuccess(Menssages.BusSMSSend, Menssages.BusCorrecto);
                    }
                    else
                    {
                        response.ErrorCode = codigoActualizado.ErrorCode;
                        response.SetError(codigoActualizado.Message);
                    }
                }
                else
                {
                    response.ErrorCode = existeCuenta.ErrorCode;
                    response.SetError(existeCuenta.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462157094;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462157094, $"Error en {metodo}(EntReenviaCodigo solicitud): {ex.Message}", solicitud, ex, response));
            }
            return response;
        }

        //Cambia/Actualiza contraseña
        public async Task<IMDResponse<dynamic>> BCambiaContrasena(Guid uIdUsuario, EntNuevaContrasena contrasena)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BCambiaContrasena);
            _logger.LogInformation(IMDSerializer.Serialize(67823462184289, $"Inicia {metodo}(Guid uIdUsuario, EntNuevaContrasena contrasena)", uIdUsuario, contrasena));

            try
            {
                if (contrasena.sContrasenia.Equals(contrasena.sConfirmaContrasenia) == true)
                {

                    var tempNuevaContrasena = await BEncryptData(contrasena.sContrasenia, true);
                    contrasena.sContrasenia = tempNuevaContrasena.Result;
                    var tempConfirmacionNuevaContrasena = await BEncryptData(contrasena.sConfirmaContrasenia, true);
                    contrasena.sConfirmaContrasenia = tempConfirmacionNuevaContrasena.Result;
                    var actualizaContrasena = await _datUsuario.DUpdateContrasena(uIdUsuario, contrasena);

                    if (actualizaContrasena.Result == true)
                    {
                        response.SetSuccess(Menssages.BusComplete, Menssages.BusCompleteCorrect);
                    }
                    else
                    {
                        response.ErrorCode = actualizaContrasena.ErrorCode;
                        response.SetError(actualizaContrasena.Message);
                    }
                }
                else
                {
                    response.SetError(Menssages.BusPasswordCoincide);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462185066;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462185066, $"Error en {metodo}(Guid uIdUsuario, EntNuevaContrasena contrasena): {ex.Message}", uIdUsuario, contrasena, ex, response));
            }
            return response;
        }

        
        
        //Guarda firebase Token       
        public async Task<IMDResponse<bool>> BGuardaFireBaseToken(Guid uIdUsuario, string sFcmToken, string sInfoAppOS, string? sIdAplicacion)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BGuardaFireBaseToken);
            _logger.LogInformation(IMDSerializer.Serialize(67823462537047, $"Inicia {metodo}(Guid uIdUsuario, string sFcmToken)", uIdUsuario, sFcmToken, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                EntFirebaseToken firebaseToken = new EntFirebaseToken
                {
                    uIdUsuario = uIdUsuario,
                    sFcmToken = sFcmToken,
                    dtFechaCreacion = DateTime.Now,
                    sInfoAppOS = sInfoAppOS,
                    sIdAplicacion = sIdAplicacion
                };

                var entUsuarios = await BGet(uIdUsuario);

                var resFirebaseToken = await _datUsuario.DSaveFirebaseToken(firebaseToken, entUsuarios.Result);

                if (resFirebaseToken.Result == true)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.ErrorCode = resFirebaseToken.ErrorCode;
                    response.SetError(resFirebaseToken.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462537824;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462537824, $"Error en {metodo}(Guid uIdUsuario, string sFcmToken): {ex.Message}", uIdUsuario, sFcmToken, ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823462537047, $"Termina {metodo}(Guid uIdUsuario, string sFcmToken)", uIdUsuario, sFcmToken, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUpdateUsuario entUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.BUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462585221, $"Inicia {metodo}(EntUpdateUsuario entUsuario)", entUsuario));

            try
            {
                var resUsuario = await _datUsuario.DGetUsuario(entUsuario.uIdUsuario);
                bool noUpdateImage = false;
                if (!resUsuario.HasError)
                {
                    if (entUsuario.sNombre == null)
                    {
                        entUsuario.sNombre = resUsuario.Result.sNombre;
                    }

                    if (entUsuario.sApellidoPaterno == null)
                    {
                        entUsuario.sApellidoPaterno = resUsuario.Result.sApellidoPaterno;
                    }

                    if (entUsuario.sApellidoMaterno == null)
                    {
                        entUsuario.sApellidoMaterno = resUsuario.Result.sApellidoMaterno;
                    }

                    if (entUsuario.cGenero == null)
                    {
                        entUsuario.cGenero = resUsuario.Result.cGenero;
                    }

                    if (entUsuario.sCodigoVerificacion == null)
                    {
                        entUsuario.sCodigoVerificacion = resUsuario.Result.sCodigoVerificacion;
                    }

                    if (entUsuario.uIdRedSocialGoogle == null)
                    {
                        entUsuario.uIdRedSocialGoogle = resUsuario.Result.uIdRedSocialGoogle;
                    }

                    if (entUsuario.sRedSocialGoogle == null)
                    {
                        entUsuario.sRedSocialGoogle = resUsuario.Result.sRedSocialGoogle;
                    }

                    if (entUsuario.uIdRedSocialFaceBook == null)
                    {
                        entUsuario.uIdRedSocialFaceBook = resUsuario.Result.uIdRedSocialFaceBook;
                    }

                    if (entUsuario.sRedSocialFaceBook == null)
                    {
                        entUsuario.sRedSocialFaceBook = resUsuario.Result.sRedSocialFaceBook;
                    }

                    if (entUsuario.uIdRedSocialApple == null)
                    {
                        entUsuario.uIdRedSocialApple = resUsuario.Result.uIdRedSocialApple;
                    }

                    if (entUsuario.sRedSocialApple == null)
                    {
                        entUsuario.sRedSocialApple = resUsuario.Result.sRedSocialApple;
                    }

                    if (entUsuario.sFotografia == null)
                    {
                        noUpdateImage = true;
                        entUsuario.sFotografia = resUsuario.Result.sFotografia;
                    }

                    if (entUsuario.sLada == null)
                    {
                        entUsuario.sLada = resUsuario.Result.sLada;
                    }

                    if (entUsuario.sTelefono == null)
                    {
                        entUsuario.sTelefono = resUsuario.Result.sTelefono;
                    }

                    if (entUsuario.sCURP == null)
                    {
                        entUsuario.sCURP = resUsuario.Result.sCURP;
                    }

                    if (resUsuario.Result.sCURP != entUsuario.sCURP)
                    {
                        //Validar que la curp no esté asociado a otro usuario.
                        var existeCURP = await BExisteCURP(entUsuario.sCURP!);
                        if (existeCURP.Result)
                        {
                            response.SetError(Menssages.BusCrupNoValid);
                            return response;
                        }
                    }

                    if (resUsuario.Result.sTelefono != entUsuario.sTelefono)
                    {
                        //Validar que el telefono no esté asociado a otro usuario.
                        var existeTelefono = await BExisteTelefono(entUsuario.sTelefono!);
                        if (existeTelefono.Result)
                        {
                            response.SetError(Menssages.BusPhoneNoValid);
                            return response;
                        }
                    }

                    entUsuario.bBaja = false;
                    entUsuario.dtFechaModificacion = DateTime.UtcNow;

                    var upd = await _datUsuario.DUpdateUsuario(entUsuario);
                    if (upd.Result == true)
                    {
                        //Carga de imagaen
                        entUsuario.sFotografia = null;//se quita la imagen
                        if (string.IsNullOrEmpty(entUsuario.sFotografia) == false && noUpdateImage == false)
                        {
                            EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                            {
                                uIdUsuario = entUsuario.uIdUsuario,
                                sImagen = entUsuario.sFotografia
                            };

                            var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                            if (guardaImagen.HasError)
                            {
                                response.ErrorCode = guardaImagen.ErrorCode;
                                response.SetError(guardaImagen.Message);
                                return response;
                            }

                        }
                        //Termina Carga Imagen
                        var user = await _datUsuario.DGetUsuario(entUsuario.uIdUsuario);
                        //Se actualiza datos de monedero
                        if (user.Result.uIdMonedero != null)
                        {
                            var monederoActualizado = await BActualizaDatosMonedero(user.Result);

                            if (monederoActualizado.HasError)
                            {
                                response.ErrorCode = monederoActualizado.ErrorCode;
                                response.SetError(monederoActualizado.Message);
                                return response;
                            }
                        }
                        //Termina actualización de monedero
                        //Descarga de imagen del Azure Blob Storage
                        user.Result.sFotografia = "";//Se quita descargar imagen
                        if (!string.IsNullOrEmpty(user.Result.sFotografia))
                        {
                            var imageUsuario = await BDescargaImagenPerfil(user.Result.uIdUsuario);
                            if (imageUsuario.HasError != true)
                            {
                                user.Result.sFotografia = imageUsuario.Result;
                            }
                            else
                            {
                                response.ErrorCode = imageUsuario.ErrorCode;
                                response.SetError(imageUsuario.Message);
                            }
                        }
                        //Termina Descarga de imagen del Azure Blob Storage

                        //Se actualiza datos de monedero
                        //await BActualizarDatosMonedero(user.Result.uIdUsuario);

                        response.SetSuccess(user.Result, Menssages.BusUserUpdateSuccess);

                    }
                    else
                    {
                        response.SetError(upd.Message);
                    }
                }
                else
                {
                    response.SetError(Menssages.BusUserUpdateNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462585998;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(67823462585998, $"Error en {metodo}(EntUpdateUsuario entUsuario): {ex.Message}", entUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUpdateUsuarioRedSocial creaUsarioRedSocialBaja)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.BUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462810551, $"Inicia {metodo}(EntUpdateUsuarioRedSocial creaUsarioRedSocialBaja)", creaUsarioRedSocialBaja));

            try
            {
                var resUsuario = await _datUsuario.DGetUsuario(creaUsarioRedSocialBaja.uIdUsuario);
                bool noUpdateImage = false;
                if (!resUsuario.HasError)
                {
                    if (creaUsarioRedSocialBaja.sNombre == null)
                    {
                        creaUsarioRedSocialBaja.sNombre = resUsuario.Result.sNombre;
                    }

                    if (creaUsarioRedSocialBaja.sApellidoPaterno == null)
                    {
                        creaUsarioRedSocialBaja.sApellidoPaterno = resUsuario.Result.sApellidoPaterno;
                    }

                    if (creaUsarioRedSocialBaja.sApellidoMaterno == null)
                    {
                        creaUsarioRedSocialBaja.sApellidoMaterno = resUsuario.Result.sApellidoMaterno;
                    }

                    if (creaUsarioRedSocialBaja.cGenero == null)
                    {
                        creaUsarioRedSocialBaja.cGenero = resUsuario.Result.cGenero;
                    }

                    if (creaUsarioRedSocialBaja.sCodigoVerificacion == null)
                    {
                        creaUsarioRedSocialBaja.sCodigoVerificacion = resUsuario.Result.sCodigoVerificacion;
                    }

                    if (creaUsarioRedSocialBaja.uIdRedSocialGoogle == null)
                    {
                        creaUsarioRedSocialBaja.uIdRedSocialGoogle = resUsuario.Result.uIdRedSocialGoogle;
                    }

                    if (creaUsarioRedSocialBaja.sRedSocialGoogle == null)
                    {
                        creaUsarioRedSocialBaja.sRedSocialGoogle = resUsuario.Result.sRedSocialGoogle;
                    }

                    if (creaUsarioRedSocialBaja.uIdRedSocialFaceBook == null)
                    {
                        creaUsarioRedSocialBaja.uIdRedSocialFaceBook = resUsuario.Result.uIdRedSocialFaceBook;
                    }

                    if (creaUsarioRedSocialBaja.sRedSocialFaceBook == null)
                    {
                        creaUsarioRedSocialBaja.sRedSocialFaceBook = resUsuario.Result.sRedSocialFaceBook;
                    }

                    if (creaUsarioRedSocialBaja.uIdRedSocialApple == null)
                    {
                        creaUsarioRedSocialBaja.uIdRedSocialApple = resUsuario.Result.uIdRedSocialApple;
                    }

                    if (creaUsarioRedSocialBaja.sRedSocialApple == null)
                    {
                        creaUsarioRedSocialBaja.sRedSocialApple = resUsuario.Result.sRedSocialApple;
                    }

                    if (creaUsarioRedSocialBaja.sFotografia == null)
                    {
                        noUpdateImage = true;
                        creaUsarioRedSocialBaja.sFotografia = resUsuario.Result.sFotografia;
                    }

                    if (creaUsarioRedSocialBaja.sLada == null)
                    {
                        creaUsarioRedSocialBaja.sLada = resUsuario.Result.sLada;
                    }

                    if (creaUsarioRedSocialBaja.sTelefono == null)
                    {
                        creaUsarioRedSocialBaja.sTelefono = resUsuario.Result.sTelefono;
                    }

                    if (creaUsarioRedSocialBaja.sCURP == null)
                    {
                        creaUsarioRedSocialBaja.sCURP = resUsuario.Result.sCURP;
                    }

                    if (resUsuario.Result.sCURP != creaUsarioRedSocialBaja.sCURP)
                    {
                        //Validar que la curp no esté asociado a otro usuario.
                        var existeCURP = await BExisteCURP(creaUsarioRedSocialBaja.sCURP!);
                        if (existeCURP.Result)
                        {
                            response.SetError(Menssages.BusCrupNoValid);
                            return response;
                        }
                    }

                    if (resUsuario.Result.sTelefono != creaUsarioRedSocialBaja.sTelefono)
                    {
                        //Validar que el telefono no esté asociado a otro usuario.
                        var existeTelefono = await BExisteTelefono(creaUsarioRedSocialBaja.sTelefono!);
                        if (existeTelefono.Result)
                        {
                            response.SetError(Menssages.BusPhoneNoValid);
                            return response;
                        }
                    }

                    creaUsarioRedSocialBaja.bBaja = false;
                    creaUsarioRedSocialBaja.dtFechaModificacion = DateTime.UtcNow;
                    creaUsarioRedSocialBaja.bCuentaVerificada = false;

                    var upd = await _datUsuario.DUpdateUsuario(creaUsarioRedSocialBaja);
                    if (upd.Result == true)
                    {
                        //Carga de imagaen
                        creaUsarioRedSocialBaja.sFotografia = null;//se quita la imagen
                        if (string.IsNullOrEmpty(creaUsarioRedSocialBaja.sFotografia) == false && noUpdateImage == false)
                        {
                            EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                            {
                                uIdUsuario = creaUsarioRedSocialBaja.uIdUsuario,
                                sImagen = creaUsarioRedSocialBaja.sFotografia
                            };

                            var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                            if (guardaImagen.HasError)
                            {
                                response.ErrorCode = guardaImagen.ErrorCode;
                                response.SetError(guardaImagen.Message);
                                return response;
                            }

                        }
                        //Termina Carga Imagen
                        var user = await _datUsuario.DGetUsuario(creaUsarioRedSocialBaja.uIdUsuario);
                        //Se actualiza datos de monedero
                        if (user.Result.uIdMonedero != null)
                        {
                            var monederoActualizado = await BActualizaDatosMonedero(user.Result);

                            if (monederoActualizado.HasError)
                            {
                                response.ErrorCode = monederoActualizado.ErrorCode;
                                response.SetError(monederoActualizado.Message);
                                return response;
                            }
                        }
                        //Termina actualización de monedero
                        //Descarga de imagen del Azure Blob Storage
                        user.Result.sFotografia = "";//Se quita descargar imagen
                        if (!string.IsNullOrEmpty(user.Result.sFotografia))
                        {
                            var imageUsuario = await BDescargaImagenPerfil(user.Result.uIdUsuario);
                            if (imageUsuario.HasError != true)
                            {
                                user.Result.sFotografia = imageUsuario.Result;
                            }
                            else
                            {
                                response.ErrorCode = imageUsuario.ErrorCode;
                                response.SetError(imageUsuario.Message);
                            }
                        }
                        //Termina Descarga de imagen del Azure Blob Storage

                        //Se actualiza datos de monedero
                        //await BActualizarDatosMonedero(user.Result.uIdUsuario);

                        response.SetSuccess(user.Result, Menssages.BusUserUpdateSuccess);

                    }
                    else
                    {
                        response.SetError(upd.Message);
                    }
                }
                else
                {
                    response.SetError(Menssages.BusUserUpdateNoExist);
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462811328;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462811328, $"Error en {metodo}(EntUpdateUsuarioRedSocial creaUsarioRedSocialBaja): {ex.Message}", creaUsarioRedSocialBaja, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUsuario nuevoUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.BUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462815213, $"Inicia {metodo}(EntUsuario nuevoUsuario)", nuevoUsuario));

            try
            {
                var resUsuario = await _datUsuario.DGetUsuario(nuevoUsuario.uIdUsuario);
                bool noUpdateImage = false;
                if (!resUsuario.HasError)
                {
                    if (nuevoUsuario.sNombre == null)
                    {
                        nuevoUsuario.sNombre = resUsuario.Result.sNombre;
                    }

                    if (nuevoUsuario.sApellidoPaterno == null)
                    {
                        nuevoUsuario.sApellidoPaterno = resUsuario.Result.sApellidoPaterno;
                    }

                    if (nuevoUsuario.sApellidoMaterno == null)
                    {
                        nuevoUsuario.sApellidoMaterno = resUsuario.Result.sApellidoMaterno;
                    }

                    if (nuevoUsuario.cGenero == null)
                    {
                        nuevoUsuario.cGenero = resUsuario.Result.cGenero;
                    }

                    if (nuevoUsuario.sCodigoVerificacion == null)
                    {
                        nuevoUsuario.sCodigoVerificacion = resUsuario.Result.sCodigoVerificacion;
                    }

                    if (nuevoUsuario.uIdRedSocialGoogle == null)
                    {
                        nuevoUsuario.uIdRedSocialGoogle = resUsuario.Result.uIdRedSocialGoogle;
                    }

                    if (nuevoUsuario.sRedSocialGoogle == null)
                    {
                        nuevoUsuario.sRedSocialGoogle = resUsuario.Result.sRedSocialGoogle;
                    }

                    if (nuevoUsuario.uIdRedSocialFaceBook == null)
                    {
                        nuevoUsuario.uIdRedSocialFaceBook = resUsuario.Result.uIdRedSocialFaceBook;
                    }

                    if (nuevoUsuario.sRedSocialFaceBook == null)
                    {
                        nuevoUsuario.sRedSocialFaceBook = resUsuario.Result.sRedSocialFaceBook;
                    }

                    if (nuevoUsuario.uIdRedSocialApple == null)
                    {
                        nuevoUsuario.uIdRedSocialApple = resUsuario.Result.uIdRedSocialApple;
                    }

                    if (nuevoUsuario.sRedSocialApple == null)
                    {
                        nuevoUsuario.sRedSocialApple = resUsuario.Result.sRedSocialApple;
                    }

                    if (nuevoUsuario.sFotografia == null)
                    {
                        noUpdateImage = true;
                        nuevoUsuario.sFotografia = resUsuario.Result.sFotografia;
                    }

                    if (nuevoUsuario.sLada == null)
                    {
                        nuevoUsuario.sLada = resUsuario.Result.sLada;
                    }

                    if (nuevoUsuario.sTelefono == null)
                    {
                        nuevoUsuario.sTelefono = resUsuario.Result.sTelefono;
                    }

                    if (nuevoUsuario.sCURP == null)
                    {
                        nuevoUsuario.sCURP = resUsuario.Result.sCURP;
                    }

                    if (resUsuario.Result.sCURP != nuevoUsuario.sCURP)
                    {
                        //Validar que la curp no esté asociado a otro usuario.
                        var existeCURP = await BExisteCURP(nuevoUsuario.sCURP!);
                        if (existeCURP.Result)
                        {
                            response.SetError(Menssages.BusCrupNoValid);
                            return response;
                        }
                    }

                    if (resUsuario.Result.sTelefono != nuevoUsuario.sTelefono)
                    {
                        //Validar que el telefono no esté asociado a otro usuario.
                        var existeTelefono = await BExisteTelefono(nuevoUsuario.sTelefono!);
                        if (existeTelefono.Result)
                        {
                            response.SetError(Menssages.BusPhoneNoValid);
                            return response;
                        }
                    }

                    nuevoUsuario.bBaja = false;
                    nuevoUsuario.dtFechaModificacion = DateTime.UtcNow;
                    nuevoUsuario.bCuentaVerificada = false;
                    var upd = await _datUsuario.DUpdateUsuario(nuevoUsuario);
                    if (upd.Result == true)
                    {
                        //Carga de imagaen
                        nuevoUsuario.sFotografia = null;//se quita la imagen
                        if (string.IsNullOrEmpty(nuevoUsuario.sFotografia) == false && noUpdateImage == false)
                        {
                            EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                            {
                                uIdUsuario = nuevoUsuario.uIdUsuario,
                                sImagen = nuevoUsuario.sFotografia
                            };

                            var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                            if (guardaImagen.HasError)
                            {
                                response.ErrorCode = guardaImagen.ErrorCode;
                                response.SetError(guardaImagen.Message);
                                return response;
                            }

                        }
                        //Termina Carga Imagen
                        var user = await _datUsuario.DGetUsuario(nuevoUsuario.uIdUsuario);
                        //Se actualiza datos de monedero
                        if (user.Result.uIdMonedero != null)
                        {
                            var monederoActualizado = await BActualizaDatosMonedero(user.Result);

                            if (monederoActualizado.HasError)
                            {
                                response.ErrorCode = monederoActualizado.ErrorCode;
                                response.SetError(monederoActualizado.Message);
                                return response;
                            }
                        }
                        //Termina actualización de monedero
                        //Descarga de imagen del Azure Blob Storage
                        user.Result.sFotografia = "";//Se quita descargar imagen
                        if (!string.IsNullOrEmpty(user.Result.sFotografia))
                        {
                            var imageUsuario = await BDescargaImagenPerfil(user.Result.uIdUsuario);
                            if (imageUsuario.HasError != true)
                            {
                                user.Result.sFotografia = imageUsuario.Result;
                            }
                            else
                            {
                                response.ErrorCode = imageUsuario.ErrorCode;
                                response.SetError(imageUsuario.Message);
                            }
                        }
                        //Termina Descarga de imagen del Azure Blob Storage

                        //Se actualiza datos de monedero
                        //await BActualizarDatosMonedero(user.Result.uIdUsuario);

                        response.SetSuccess(user.Result, Menssages.BusUserUpdateSuccess);

                    }
                    else
                    {
                        response.SetError(upd.Message);
                    }
                }
                else
                {
                    response.SetError(Menssages.BusUserUpdateNoExist);
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462815990;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462815990, $"Error en {metodo}(EntUsuario nuevoUsuario): {ex.Message}", nuevoUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUpdateUsarioActivo usuarioActivo)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.BUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462821429, $"Inicia {metodo}(EntUpdateUsarioActivo usuarioActivo)", usuarioActivo));

            try
            {

                var resUsuario = await _datUsuario.DGetUsuario(usuarioActivo.uIdUsuario);
                bool noUpdateImage = false;
                if (!resUsuario.HasError)
                {
                    if (usuarioActivo.sNombre == null)
                    {
                        usuarioActivo.sNombre = resUsuario.Result.sNombre;
                    }

                    if (usuarioActivo.sApellidoPaterno == null)
                    {
                        usuarioActivo.sApellidoPaterno = resUsuario.Result.sApellidoPaterno;
                    }

                    if (usuarioActivo.sApellidoMaterno == null)
                    {
                        usuarioActivo.sApellidoMaterno = resUsuario.Result.sApellidoMaterno;
                    }

                    if (usuarioActivo.cGenero == null)
                    {
                        usuarioActivo.cGenero = resUsuario.Result.cGenero;
                    }

                    if (usuarioActivo.sCodigoVerificacion == null)
                    {
                        usuarioActivo.sCodigoVerificacion = resUsuario.Result.sCodigoVerificacion;
                    }

                    if (usuarioActivo.sFotografia == null)
                    {
                        noUpdateImage = true;
                        usuarioActivo.sFotografia = resUsuario.Result.sFotografia;
                    }

                    if (usuarioActivo.sLada == null)
                    {
                        usuarioActivo.sLada = resUsuario.Result.sLada;
                    }

                    if (usuarioActivo.sTelefono == null)
                    {
                        usuarioActivo.sTelefono = resUsuario.Result.sTelefono;
                    }

                    if (usuarioActivo.sCURP == null)
                    {
                        usuarioActivo.sCURP = resUsuario.Result.sCURP;
                    }

                    if (resUsuario.Result.sCURP != usuarioActivo.sCURP)
                    {
                        //Validar que la curp no esté asociado a otro usuario.
                        var existeCURP = await BExisteCURP(usuarioActivo.sCURP!);
                        if (existeCURP.Result)
                        {
                            response.SetError(Menssages.BusCrupNoValid);
                            return response;
                        }
                    }

                    if (resUsuario.Result.sTelefono != usuarioActivo.sTelefono)
                    {
                        //Validar que el telefono no esté asociado a otro usuario.
                        var existeTelefono = await BExisteTelefono(usuarioActivo.sTelefono!);
                        if (existeTelefono.Result)
                        {
                            response.SetError(Menssages.BusPhoneNoValid);
                            return response;
                        }
                    }

                    usuarioActivo.bBaja = false;
                    usuarioActivo.dtFechaModificacion = DateTime.UtcNow;
                    usuarioActivo.bCuentaVerificada = false;
                    var upd = await _datUsuario.DUpdateUsuario(usuarioActivo);
                    if (upd.Result == true)
                    {
                        //Carga de imagaen
                        usuarioActivo.sFotografia = null;//se quita la imagen
                        if (string.IsNullOrEmpty(usuarioActivo.sFotografia) == false && noUpdateImage == false)
                        {
                            EntRequestBlobStorage requestImgaenPerfil = new EntRequestBlobStorage()
                            {
                                uIdUsuario = usuarioActivo.uIdUsuario,
                                sImagen = usuarioActivo.sFotografia
                            };

                            var guardaImagen = await BGuardaImagenPerfil(requestImgaenPerfil);

                            if (guardaImagen.HasError)
                            {
                                response.ErrorCode = guardaImagen.ErrorCode;
                                response.SetError(guardaImagen.Message);
                                return response;
                            }

                        }
                        //Termina Carga Imagen
                        var user = await _datUsuario.DGetUsuario(usuarioActivo.uIdUsuario);
                        //Se actualiza datos de monedero
                        if (user.Result.uIdMonedero != null)
                        {
                            var monederoActualizado = await BActualizaDatosMonedero(user.Result);

                            if (monederoActualizado.HasError)
                            {
                                response.ErrorCode = monederoActualizado.ErrorCode;
                                response.SetError(monederoActualizado.Message);
                                return response;
                            }
                        }
                        //Termina actualización de monedero
                        //Descarga de imagen del Azure Blob Storage
                        user.Result.sFotografia = "";//Se quita descargar imagen
                        if (!string.IsNullOrEmpty(user.Result.sFotografia))
                        {
                            var imageUsuario = await BDescargaImagenPerfil(user.Result.uIdUsuario);
                            if (imageUsuario.HasError != true)
                            {
                                user.Result.sFotografia = imageUsuario.Result;
                            }
                            else
                            {
                                response.ErrorCode = imageUsuario.ErrorCode;
                                response.SetError(imageUsuario.Message);
                            }
                        }
                        //Termina Descarga de imagen del Azure Blob Storage

                        //Se actualiza datos de monedero
                        //await BActualizarDatosMonedero(user.Result.uIdUsuario);

                        response.SetSuccess(user.Result, Menssages.BusUserUpdateSuccess);

                    }
                    else
                    {
                        response.SetError(upd.Message);
                    }
                }
                else
                {
                    response.SetError(Menssages.BusUserUpdateNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462822206;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462822206, $"Error en {metodo}(EntUpdateUsarioActivo usuarioActivo): {ex.Message}", usuarioActivo, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> BEliminarUsuario(Guid uIdUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BEliminarUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462597653, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var eliminaUsuario = await _datUsuario.DEliminarUsuario(new EntEliminarUsuario()
                {
                    uIdUsuario = uIdUsuario,
                    bCuentaVerificada = false,
                    sTelefono = null,
                    sContrasena = null,
                    sCURP = null,
                    sFotografia = null,
                    bActivo = true,
                    bBaja = true,
                    dtFechaBaja = DateTime.UtcNow,
                    uIdUsuarioBaja = uIdUsuario
                });

                if (eliminaUsuario.HasError != true)
                {
                    if (eliminaUsuario.Result == true)
                    {
                        var usuarioEliminado = await _datUsuario.DGetUsuario(uIdUsuario);

                        if (usuarioEliminado.HasError != true)
                        {
                            var eliminaFotoAzure = await BEliminaImagenPerfil(usuarioEliminado.Result.uIdUsuario);

                            if (eliminaFotoAzure.HasError == true)
                            {
                                response.ErrorCode = eliminaFotoAzure.ErrorCode;
                                response.SetError(eliminaFotoAzure.Message);
                                return response;
                            }

                            string nombreUsuario = usuarioEliminado.Result.sNombre + " " + usuarioEliminado.Result.sApellidoPaterno + " " + usuarioEliminado.Result.sApellidoMaterno;

                            string plantillaEliminaCuenta = string.Empty;
                            string sCorreo = usuarioEliminado.Result.sCorreo!;

                            try
                            {
                                plantillaEliminaCuenta = _busLenguaje.BusSetLanguajeDelCuenta();
                            }
                            catch (Exception ex)
                            {
                                response.ErrorCode = 67823462598430;
                                response.SetError($"{Menssages.BusPlantillaError}: {ex}");
                                _logger.LogError(IMDSerializer.Serialize(67823462598430, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
                                return response;
                            }
                            var correoRecuperaCuenta = plantillaEliminaCuenta.Replace("{Nombre}", nombreUsuario.Trim());
                            correoRecuperaCuenta = correoRecuperaCuenta.Replace("{UUID}", usuarioEliminado.Result.uIdUsuario.ToString());
                            correoRecuperaCuenta = correoRecuperaCuenta.Replace("{Fecha}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));


                            EntBusMessCorreo busMessage = new EntBusMessCorreo
                            {
                                uIdUsuario = uIdUsuario,
                                sMensaje = correoRecuperaCuenta,
                                sCorreoElectronico = sCorreo,
                                bHtml = true
                            };
                            ///Envío a bus evento Nueva Cuenta Código de Verificacion
                            await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                            {
                                Content = busMessage
                            });

                            response.SetSuccess(default, Menssages.DatCompleteSucces);
                        }
                        else
                        {
                            response.ErrorCode = usuarioEliminado.ErrorCode;
                            response.SetError(usuarioEliminado.Message);
                        }
                    }

                    if (eliminaUsuario.Result == false)
                    {
                        response.SetError(eliminaUsuario.Message);
                    }
                }
                else
                {
                    response = eliminaUsuario;
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462598430;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(67823462598430, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

       
       
        public async Task<IMDResponse<dynamic>> BGuardaImagenPerfil(EntRequestBlobStorage requestImagenPerfil)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BGuardaImagenPerfil);
            _logger.LogInformation(IMDSerializer.Serialize(67823463536269, $"Inicia {metodo}(EntRequestBlobStorage requestImagenPerfil)", requestImagenPerfil, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                requestImagenPerfil.sImagen = "";

                if (!string.IsNullOrEmpty(requestImagenPerfil.sImagen))
                {
                    var existeUsuario = await _datUsuario.DGetUsuario(requestImagenPerfil.uIdUsuario);

                    if (existeUsuario.HasError != true)
                    {
                        var usuario = existeUsuario.Result;

                        if (usuario.bActivo == true && usuario.bBaja == false)
                        {
                            var guardaImagen = await _servAzureBlobStorage.UploadAsync(requestImagenPerfil.sImagen, ContainerEnum.PERFILESAPP, requestImagenPerfil.uIdUsuario.ToString());
                            var dowloadImage = await _servAzureBlobStorage.DownloadAsync(ContainerEnum.PERFILESAPP, requestImagenPerfil.uIdUsuario.ToString());
                            if (guardaImagen.HasError != true)
                            {
                                //Actualiza el campo  fotografia 
                                var actualizaFotografia = await _datUsuario.DUpdateFotografia(requestImagenPerfil.uIdUsuario, guardaImagen.Result);


                                if (actualizaFotografia.HasError != true)
                                {
                                    //Regresa el nombre guardado
                                    response.SetSuccess(guardaImagen.Result);
                                }
                                else
                                {
                                    response.ErrorCode = actualizaFotografia.ErrorCode;
                                    response.SetError(actualizaFotografia.Message);
                                }


                            }
                            else
                            {
                                response.ErrorCode = guardaImagen.ErrorCode;
                                response.SetError(guardaImagen.Message);
                            }

                        }
                        else
                        {
                            response.SetError(Menssages.DatUserSolicitadeNoAviable);
                        }
                    }
                    else
                    {
                        response.ErrorCode = existeUsuario.ErrorCode;
                        response.SetError(existeUsuario.Message);
                    }

                }
                else
                {
                    response.SetError(Menssages.BusRequiredImage);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463537046;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463537046, $"Error en {metodo}(EntRequestBlobStorage requestImagenPerfil): {ex.Message}", requestImagenPerfil, ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823463536269, $"Termina {metodo}(EntRequestBlobStorage requestImagenPerfil)", requestImagenPerfil, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        public async Task<IMDResponse<dynamic>> BDescargaImagenPerfil(Guid uIdUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BDescargaImagenPerfil);
            _logger.LogInformation(IMDSerializer.Serialize(67823463547147, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var existeUsuario = await _datUsuario.DGetUsuario(uIdUsuario);

                if (existeUsuario.HasError != true)
                {
                    var usuario = existeUsuario.Result;

                    if (usuario.bActivo == true && usuario.bBaja == false)
                    {
                        var dowloadImage = await _servAzureBlobStorage.DownloadAsync(ContainerEnum.PERFILESAPP, uIdUsuario.ToString());
                        if (dowloadImage.HasError != true)
                        {
                            //Regresa la imagen en base64
                            response.SetSuccess(dowloadImage.Result);

                        }
                        else
                        {
                            response.ErrorCode = dowloadImage.ErrorCode;
                            response.SetError(dowloadImage.Message);
                        }

                    }
                    else
                    {
                        response.SetError(Menssages.DatUserSolicitadeNoAviable);
                    }
                }
                else
                {
                    response.ErrorCode = existeUsuario.ErrorCode;
                    response.SetError(existeUsuario.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463547924;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463547924, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<dynamic>> BEliminaImagenPerfil(Guid uIdUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BEliminaImagenPerfil);
            _logger.LogInformation(IMDSerializer.Serialize(67823463582889, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var existeUsuario = await _datUsuario.DGetUsuario(uIdUsuario);

                if (existeUsuario.HasError != true)
                {
                    var usuario = existeUsuario.Result;

                    if (usuario.bActivo == true && usuario.bBaja == true)
                    {
                        var deleteImage = await _servAzureBlobStorage.DeleteAsync(ContainerEnum.PERFILESAPP, uIdUsuario.ToString());
                        if (deleteImage.HasError != true)
                        {
                            //Se elino correctamente
                            response.SetNoContent();

                        }
                        else
                        {
                            response.ErrorCode = deleteImage.ErrorCode;
                            response.SetError(deleteImage.Message);
                        }

                    }
                    else
                    {
                        response.SetError(Menssages.DatUserSolicitadeNoAviable);
                    }
                }
                else
                {
                    response.ErrorCode = existeUsuario.ErrorCode;
                    response.SetError(existeUsuario.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463583666;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463583666, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

       

        [IMDMetodo(67823465411170, 67823465410393)]
        public async Task<IMDResponse<EntUsuario>> BValidaMonederoUsuario(Guid uIdUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var usuario = await BGet(uIdUsuario);
                if (usuario != null)
                {
                    if (usuario.Result.uIdMonedero == null)
                    {
                        //Se crea el monedero...
                        var httpMonederoC = await BHttpMonederoC(usuario.Result);
                        if (httpMonederoC.HasError != true)
                        {
                            foreach (var monedero in httpMonederoC.Result)
                            {
                                usuario.Result.uIdMonedero = monedero.IdMonedero;
                                usuario.Result.sNoMonedero = monedero.NumMonedero.ToString();

                                var monederoAsigando = await _datUsuario.DUpdateMonederoUsuario(usuario.Result.uIdUsuario, monedero.IdMonedero);
                                if (monederoAsigando.HasError == true)
                                {
                                    response.ErrorCode = monederoAsigando.ErrorCode;
                                    response.SetError(monederoAsigando.Message);
                                    return response;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Obtener los datos del monedero...
                        //var token = await _auth.BIniciarSesion();
                        var datosMonedero = await _busMonedero.BDatosMonedero(Guid.Parse(usuario.Result.uIdMonedero.ToString()));
                        if (!datosMonedero.HasError)
                        {
                            usuario.Result.sNoMonedero = datosMonedero.Result.numMonedero.ToString();
                        }
                    }

                    response.SetSuccess(usuario.Result);
                }
                else { response.SetError(usuario!.Message); }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

       

        [IMDMetodo(67823465566570, 67823465565793)]
        public async Task<IMDResponse<dynamic>> BDescargaImagenUsuarioApp(Guid uIdUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var dowloadImage = await _servAzureBlobStorage.DownloadAsync(ContainerEnum.PERFILESAPP, uIdUsuario.ToString());
                if (dowloadImage.HasError != true)
                {
                    //Regresa la imagen en base64
                    response.SetSuccess(dowloadImage.Result);
                }
                else
                {
                    response.ErrorCode = dowloadImage.ErrorCode;
                    response.SetError(dowloadImage.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465594542, 67823465593765)]
        public async Task<IMDResponse<List<EntUsuarioAppInfo>>> BGetUsuariosAppInfo(Guid uIdUsuario)
        {
            IMDResponse<List<EntUsuarioAppInfo>> response = new IMDResponse<List<EntUsuarioAppInfo>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));
            try
            {
                response = await _datUsuario.DGetUsuarioAppInfo(uIdUsuario);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        
        
        [IMDMetodo(67823466299281, 67823466300058)]
        public async Task<IMDResponse<bool>> BActualizaDispositivoCuenta(EntDispositivoCuentaUpdate entDispositivoCuentaUpdate)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entDispositivoCuentaUpdate));
            try
            {
                response = await _datUsuario.DActualizaDispositivoCuenta(entDispositivoCuentaUpdate);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entDispositivoCuentaUpdate, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466358333, 67823466359110)]
        public async Task<IMDResponse<bool>> BCerrarSesionDispositivos(EntDispositivoCuentaUpdate entDispositivoCuentaUpdate)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entDispositivoCuentaUpdate));
            try
            {
                string sMessage = entDispositivoCuentaUpdate.iEstatusCuenta == (int)eEstatusCuenta.REPORTADO ? Menssages.BusBlockedAccountApp : Menssages.BusLogoutOtherDevice;
                #region Notificacion PUSH
                EntTopicUsuario entTopicUsuario = new EntTopicUsuario()
                {
                    sTopic = entDispositivoCuentaUpdate.uIdUsuario.ToString(),
                    sActionCode = _errorCodeSesion,
                    sData = entDispositivoCuentaUpdate.sIdAplicacion
                };

                EntNotificacionPushTopic entTopicNotificacionPush = new EntNotificacionPushTopic
                {
                    entTopicUsuario = entTopicUsuario,
                    uIdUsuario = entDispositivoCuentaUpdate.uIdUsuario,
                    sTitulo = Menssages.BusTitleLogout,
                    sMensaje = sMessage
                };

                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionPushTopic.GetDescription(), _exchangeConfig, new QueueMessage<EntNotificacionPushTopic>
                {
                    Content = entTopicNotificacionPush
                });
                #endregion
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entDispositivoCuentaUpdate, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466359887, 67823466360664)]
        private async Task<IMDResponse<dynamic>> BValidarCuenta(EntValidaCuenta entValidaCuenta)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entValidaCuenta));
            try
            {
                string sUrlMapa = _busParametros.BObtener("APP_URL_MAPA_ATY").Result.Result.sValor;
                EntLoginResponse entLoginResponse = new EntLoginResponse();
                EntDispositivoCuentaUpdate entDispositivoCuentaUpdate = new EntDispositivoCuentaUpdate()
                {
                    uIdUsuario = entValidaCuenta.uIdUsuario,
                    sIdAplicacion = entValidaCuenta.sIdAplicacionRequest
                };

                switch (entValidaCuenta.iEstatusCuenta)
                {
                    case (int)eEstatusCuenta.DESBLOQUEADO:
                        if (entValidaCuenta.sIdAplicacionRequest != entValidaCuenta.sIdAplicacionBD)
                        {
                            #region Generar y Enviar Código
                            if (!entValidaCuenta.bTipoLogin || (entValidaCuenta.bTipoLogin && entValidaCuenta.bRedSocialGoogle))
                            {
                                //Genera nuevo codigo
                                IMDResponse<string> generaCodigo = await BGeneraCodigoVerificacion();

                                var codigo = await BEncryptData(generaCodigo.Result, true);
                                string codgioEncrypt = codigo.Result;

                                EntReenviaCodigo solicitud = new EntReenviaCodigo()
                                {
                                    sCorreo = entValidaCuenta.sCorreo
                                };

                                var codigoActualizado = await _datUsuario.DUpdateCodigoVerificacion(solicitud, codgioEncrypt);

                                if (codigoActualizado.Result == true)
                                {
                                    try
                                    {
                                        await BEnviarCodigoVerificacionEmailVigencia(entValidaCuenta.uIdUsuario, entValidaCuenta.sCorreo, generaCodigo.Result);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entValidaCuenta, ex, response));
                                    }
                                }

                                entLoginResponse.bRequiereCode = true;
                                response.HasError = true;
                                response.Result = entLoginResponse;
                                response.HttpCode = HttpStatusCode.PreconditionFailed;
                                response.ErrorCode = int.Parse(_errorCodeSesion);
                            }
                            #endregion
                        }
                        break;
                    case (int)eEstatusCuenta.BLOQUEADO:
                        if (entValidaCuenta.sIdAplicacionRequest != entValidaCuenta.sIdAplicacionBD)
                        {
                            entLoginResponse.sUrlMapa = sUrlMapa;
                            entLoginResponse.bRequiereCode = false;
                            response.SetError(Menssages.BusNoLogin);
                            response.Result = entLoginResponse;
                            response.HttpCode = HttpStatusCode.PreconditionFailed;
                            response.ErrorCode = int.Parse(_errorCodeSesion);
                        }
                        break;
                    case (int)eEstatusCuenta.ENCAMBIO:
                        entDispositivoCuentaUpdate.iEstatusCuenta = (int)eEstatusCuenta.BLOQUEADO;
                        BActualizaDispositivoCuenta(entDispositivoCuentaUpdate);
                        BCerrarSesionDispositivos(entDispositivoCuentaUpdate);
                        break;
                    case (int)eEstatusCuenta.REPORTADO:
                        entLoginResponse.sUrlMapa = sUrlMapa;
                        entLoginResponse.bRequiereCode = false;
                        response.SetError(Menssages.BusReportedAccount);
                        response.Result = entLoginResponse;
                        response.HttpCode = HttpStatusCode.PreconditionFailed;
                        response.ErrorCode = int.Parse(_errorCodeSesion);
                        break;
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entValidaCuenta, ex, response));
            }
            return response;
        }

        
        
        #endregion

        #region Validaciones
        /*Validaciones*/
        public async Task<IMDResponse<bool>> BValidaEmail(string pEmail)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            string metodo = nameof(this.BValidaEmail);
            _logger.LogInformation(IMDSerializer.Serialize(67823461742953, $"Inicia {metodo}(string pEmail)", pEmail, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                if (string.IsNullOrWhiteSpace(pEmail))
                {
                    response.SetSuccess(false);
                    return response;
                }

                try
                {
                    // Normalize the domain
                    pEmail = Regex.Replace(pEmail, @"(@)(.+)$", DomainMapper,
                                          RegexOptions.None, TimeSpan.FromMilliseconds(200));

                    // Examines the domain part of the email and normalizes it.
                    string DomainMapper(Match match)
                    {
                        // Use IdnMapping class to convert Unicode domain names.
                        var idn = new IdnMapping();

                        // Pull out and process domain name (throws ArgumentException on invalid)
                        string domainName = idn.GetAscii(match.Groups[2].Value);

                        return match.Groups[1].Value + domainName;
                    }
                }
                catch (RegexMatchTimeoutException)
                {
                    response.SetSuccess(false);
                }
                catch (ArgumentException)
                {
                    response.SetSuccess(false);
                }

                try
                {
                    response.SetSuccess(Regex.IsMatch(pEmail,

                       @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                       RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
                }
                catch (RegexMatchTimeoutException)
                {
                    return response.GetSuccess(false);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461743730;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461743730, $"Error en {metodo}(string pEmail): {ex.Message}", pEmail, ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823461742953, $"Termina {metodo}(string pEmail)", pEmail, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        //Validar  que no haya otro correo 
        public async Task<IMDResponse<bool>> BExisteCorreo(string pEmail)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BExisteCorreo);
            _logger.LogInformation(IMDSerializer.Serialize(67823461756939, $"Inicia {metodo}(string pEmail)", pEmail));

            try
            {

                IMDResponse<EntUsuario> existeCorreo = await _datUsuario.DGetCorreo(pEmail);
                if (existeCorreo.Result != null)
                {
                    EntUsuario usuarioValido = existeCorreo.Result;

                    if (usuarioValido.bMigrado == false)
                    {
                        response.SetError(Menssages.BusUserAlreadyRegister);
                    }
                    else
                    {
                        if (usuarioValido.bCuentaVerificada == true)
                        {
                            response.SetError(Menssages.BusEmailNoValidAsociate);
                        }
                        else
                        {
                            response.SetNoContent();//Indica que la cuenta no esta validada por lo cual se actualiza los datos --> HttpStatusCode.NoContent
                        }
                    }
                }
                else
                {
                    response.SetSuccess(false);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461757716;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461757716, $"Error en {metodo}(string pEmail): {ex.Message}", pEmail, ex, response));
            }
            return response;
        }

        //Validar que no haya otro telefono
        public async Task<IMDResponse<bool>> BExisteTelefono(string pTelefono)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BExisteTelefono);
            _logger.LogInformation(IMDSerializer.Serialize(67823461769371, $"Inicia {metodo}(string pTelefono)", pTelefono));

            try
            {
                IMDResponse<EntUsuario> existeCorreo = await _datUsuario.DGetTelefono(pTelefono);
                if (existeCorreo.Result != null)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetSuccess(false);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461770148;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461770148, $"Error en {metodo}(string pTelefono): {ex.Message}", pTelefono, ex, response));
            }
            return response;
        }

        //Validar que no haya otro CURP
        public async Task<IMDResponse<bool>> BExisteCURP(string pCURP)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BExisteCURP);
            _logger.LogInformation(IMDSerializer.Serialize(67823461772479, $"Inicia {metodo}(string pCURP)", pCURP));

            try
            {

                IMDResponse<EntUsuario> existeCURP = await _datUsuario.DGetCURP(pCURP);
                if (existeCURP.Result != null)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetSuccess(false);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461773256;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461773256, $"Error en {metodo}(string pCURP): {ex.Message}", pCURP, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823462784910, 67823462784133)]
        public async Task<IMDResponse<EntUsuario>> BGetByCorreo(string Email)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string Email)", Email, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                response = await _datUsuario.DGetCorreo(Email);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string Email): {ex.Message}", Email, ex, response));
            }
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Termina {metodo}(string Email)", Email, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        [IMDMetodo(67823462786464, 67823462785687)]
        public async Task<IMDResponse<EntUsuario>> BGetByAppleId(string sAppleId)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sAppleId)", sAppleId));

            try
            {
                response = await _datUsuario.DGetByAppleId(sAppleId);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sAppleId): {ex.Message}", sAppleId, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463460900, 67823463460123)]
        private async Task<IMDResponse<string>> BValidaDatosUsuarioActualizaTelefono(EntUsuarioActualizaTelefonoRequest model)
        {
            IMDResponse<string> response = new IMDResponse<string>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntUsuarioActualizaTelefonoRequest model)", model));

            try
            {

                List<string> datosRequeridos = new List<string>();

                string mensaje = string.Empty;
                bool error = false;
                if (String.IsNullOrEmpty(model.sTelefono))
                {
                    datosRequeridos.Add("Teléfono");
                    error = true;
                }

                if (String.IsNullOrEmpty(model.sCorreo))
                {
                    datosRequeridos.Add("Correo");
                    error = true;
                }

                if (error == true)
                {
                    if (datosRequeridos.Count == 1)
                    {
                        foreach (string adatosRequeridos in datosRequeridos)
                        {
                            response.SetError($"{Menssages.BusDataRequired} : " + adatosRequeridos);
                        }
                    }

                    if (datosRequeridos.Count > 1)
                    {

                        foreach (string adatosRequeridos in datosRequeridos)
                        {
                            mensaje = mensaje + " : " + adatosRequeridos;
                        }
                        response.SetError($"{Menssages.BusDatasRequired} " + mensaje);
                    }


                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntUsuarioActualizaTelefonoRequest model): {ex.Message}", model, ex, response));
            }
            return response;
        }
        #endregion

        #region Utilidades
        public async Task<IMDResponse<dynamic>> BHttpMonederoC(EntUsuario pUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BHttpMonederoC);
            _logger.LogInformation(IMDSerializer.Serialize(67823463360667, $"Inicia {metodo}(EntUsuario pUsuario)", pUsuario, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                string ruta = "genera-monedero";

                var iniciaSesionTokenKong = await _auth.BIniciarSesion();

                if (iniciaSesionTokenKong.HasError != true)
                {
                    string tokenKong = iniciaSesionTokenKong.Result.sToken;
                    EntRequestHttpMonederoC reqMonederoC = new EntRequestHttpMonederoC()
                    {
                        Cantidad = 1,
                        uIdTipo = Guid.Parse(_busParametros.BObtener("APP_TIPOBOLETOAPP_GUID").Result.Result.sValor ?? string.Empty),
                        uIdTarifa = Guid.Parse(_busParametros.BObtener("APP_TARIFAGENERAL_GUID").Result.Result.sValor ?? string.Empty),
                        sTelefono = pUsuario.sTelefono,
                        sNombre = pUsuario.sNombre,
                        sApellidoPaterno = pUsuario.sApellidoPaterno,
                        sApellidoMaterno = pUsuario.sApellidoMaterno,
                        sCorreo = pUsuario.sCorreo,
                        dtFechaNacimiento = pUsuario.dtFechaNacimiento
                    };

                    var httpResponse = await _servGenerico.SPostBody(_urlHttpClientMonederoC, ruta, reqMonederoC, tokenKong);

                    if (httpResponse.HasError != true)
                    {

                        var result = httpResponse.Result;
                        var resultJson = result.ToString();

                        List<EntResponseHttpMonederoC>? listaMonederoC = JsonConvert.DeserializeObject<List<EntResponseHttpMonederoC>>(resultJson);
                        response.SetSuccess(listaMonederoC);
                    }
                    else
                    {
                        response.ErrorCode = httpResponse.ErrorCode;
                        response.SetError(httpResponse.Message);
                    }
                }
                else
                {
                    response.ErrorCode = iniciaSesionTokenKong.ErrorCode;
                    response.SetError(iniciaSesionTokenKong.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463361444;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463361444, $"Error en {metodo}(EntUsuario pUsuario): {ex.Message}", pUsuario, ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823463360667, $"Termina {metodo}(EntUsuario pUsuario)", pUsuario, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        public async Task<IMDResponse<string>> BGeneraCodigoVerificacion()
        {
            IMDResponse<string> response = new IMDResponse<string>();

            string metodo = nameof(this.BGeneraCodigoVerificacion);
            _logger.LogInformation(IMDSerializer.Serialize(67823462036659, $"Inicia {metodo}()", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                Random random = new Random();
                string codigo = random.Next(0, 9).ToString() + random.Next(0, 9).ToString() + random.Next(0, 9).ToString() + random.Next(0, 9).ToString();
                response.SetSuccess(codigo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462037436;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462037436, $"Error en {metodo}(): {ex.Message}", ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823462036659, $"Termina {metodo}()", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        public async Task<IMDResponse<string>> BEncryptData(string pData, bool tipo)
        {

            IMDResponse<string> response = new IMDResponse<string>();
            string PCKEY = Environment.GetEnvironmentVariable("PCKEY") ?? "";
            string PCIV = Environment.GetEnvironmentVariable("PCIV") ?? "";

            if (tipo == true)
            {
                response.SetSuccess(IMDSecurity.BEncrypt(pData, PCKEY, PCIV));
            }

            if (tipo == false)
            {
                response.SetSuccess(IMDSecurity.BDecrypt(pData, PCKEY, PCIV));
            }

            return response;
        }

        public async Task<IMDResponse<bool>> BVerificaContrasena(string contrasena, string confirmaContrasena)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BVerificaContrasena);
            _logger.LogInformation(IMDSerializer.Serialize(67823462538601, $"Inicia {metodo}(string contrasena, string confirmaContrasena)", contrasena, confirmaContrasena));

            try
            {
                if (String.Equals(contrasena, confirmaContrasena))
                {
                    //Validación con expresión regular
                    string patron = "(?=.*\\d)(?=.*[a-zA-ZñÑ\\s])(?!.*\\s){8,250}";

                    if (Regex.IsMatch(contrasena, patron))
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.BusValidatePassword);
                    }
                }
                else
                {
                    response.SetError(Menssages.BusPasswordNoMacthes);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462539378;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462539378, $"Error en {metodo}(string contrasena, string confirmaContrasena): {ex.Message}", contrasena, confirmaContrasena, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<string>> BValidaDatosCompletos(EntCreateUsuario pCreateModel)
        {
            IMDResponse<string> response = new IMDResponse<string>();

            string metodo = nameof(this.BValidaDatosCompletos);
            _logger.LogInformation(IMDSerializer.Serialize(67823462589883, $"Inicia {metodo}(EntCreateUsuario pCreateModel)", pCreateModel, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                List<string> datosRequeridos = new List<string>();

                string mensaje = string.Empty;
                bool error = false;
                if (String.IsNullOrEmpty(pCreateModel.sNombre))
                {
                    datosRequeridos.Add(Menssages.BusName);
                    error = true;
                }

                if (String.IsNullOrEmpty(pCreateModel.sApellidoPaterno))
                {
                    datosRequeridos.Add(Menssages.BusLastNameP);
                    error = true;
                }

                if (String.IsNullOrEmpty(pCreateModel.sCorreo))
                {
                    datosRequeridos.Add(Menssages.BusEmail);
                    error = true;
                }

                if (String.IsNullOrEmpty(pCreateModel.sContrasena))
                {
                    datosRequeridos.Add(Menssages.BusPassword);
                    error = true;
                }

                if (String.IsNullOrEmpty(pCreateModel.sConfirmaContrasenia))
                {
                    datosRequeridos.Add(Menssages.BusConfirmPass);
                    error = true;
                }

                if (!pCreateModel.sContrasena.Equals(pCreateModel.sConfirmaContrasenia))
                {
                    datosRequeridos.Add(Menssages.BusPasswordNoMacthes);
                    error = true;
                }

                if (String.IsNullOrEmpty(pCreateModel.dtFechaNacimiento.ToString()))
                {
                    datosRequeridos.Add(Menssages.BusDateBorn);
                    error = true;
                }

                if (String.IsNullOrEmpty(pCreateModel.cGenero))
                {
                    datosRequeridos.Add(Menssages.BusGenero);
                    error = true;
                }

                if (error == true)
                {
                    if (datosRequeridos.Count == 1)
                    {
                        foreach (string adatosRequeridos in datosRequeridos)
                        {
                            response.SetError($"{Menssages.BusDataRequired} : " + adatosRequeridos);
                        }
                    }

                    if (datosRequeridos.Count > 1)
                    {

                        foreach (string adatosRequeridos in datosRequeridos)
                        {
                            mensaje = mensaje + " : " + adatosRequeridos;
                        }
                        response.SetError($"{Menssages.BusDatasRequired} " + mensaje);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462590660;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462590660, $"Error en {metodo}(EntCreateUsuario pCreateModel): {ex.Message}", pCreateModel, ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823462589883, $"Termina {metodo}(EntCreateUsuario pCreateModel)", pCreateModel, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        public async Task<IMDResponse<dynamic>> BGeneraTokenRecuperacion()
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BGeneraTokenRecuperacion);
            _logger.LogInformation(IMDSerializer.Serialize(67823462672245, $"Inicia {metodo}()"));

            try
            {
                var byteArray = new byte[64];
                var refreshToken = "";
                using (var mg = RandomNumberGenerator.Create())
                {
                    mg.GetBytes(byteArray);
                    refreshToken = Convert.ToBase64String(byteArray);
                }
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[16];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                response.SetSuccess(new String(stringChars));
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462673022;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462673022, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<string>> BGeneraContrasenaAleatoria()
        {
            IMDResponse<string> response = new IMDResponse<string>();

            string metodo = nameof(this.BGeneraContrasenaAleatoria);
            _logger.LogInformation(IMDSerializer.Serialize(67823462777917, $"Inicia {metodo}()"));

            try
            {
                Random rdn = new Random();
                Random rnd = new Random();

                int min = 8;
                int max = 15;

                string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                int longitud = caracteres.Length;
                char letra;
                int longitudContrasenia = rnd.Next(min, max + 1);
                string contraseniaAleatoria = string.Empty;

                for (int i = 0; i < longitudContrasenia; i++)
                {
                    letra = caracteres[rdn.Next(longitud)];
                    contraseniaAleatoria += letra.ToString();
                }

                response.SetSuccess(contraseniaAleatoria);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462778694;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462778694, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        private async Task<IMDResponse<bool>> EnviarCodigoVerificacionEmail(Guid uIdUsuario, string sCorreo, string codigoVerificacion, string sNombre)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BGeneraContrasenaAleatoria);
            _logger.LogInformation(IMDSerializer.Serialize(67823462777917, $"Inicia {metodo}()", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                string plantilla = _busLenguaje.BusSetLanguajeVerificationCode();
                plantilla = plantilla.Replace("{Codigo}", codigoVerificacion);
                plantilla = plantilla.Replace("{Nombre}", sNombre);

                EntBusMessCorreo busMessage = new EntBusMessCorreo
                {
                    uIdUsuario = uIdUsuario,
                    sMensaje = plantilla,
                    sCorreoElectronico = sCorreo,
                    bHtml = true
                };
                ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                {
                    Content = busMessage
                });

                response.SetSuccess(true, Menssages.BusCompleteCorrect);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462778694;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462778694, $"Error en {metodo}(): {ex.Message}", ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823462777917, $"Termina {metodo}()", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        public async Task<IMDResponse<dynamic>> BActualizaDatosMonedero(EntUsuario datosUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BActualizaDatosMonedero);
            _logger.LogInformation(IMDSerializer.Serialize(67823463668359, $"Inicia {metodo}(EntUsuario datosUsuario)", datosUsuario));

            try
            {
                var iniciaSesionTokenKong = await _auth.BIniciarSesion();

                if (iniciaSesionTokenKong.HasError != true)
                {
                    string tokenKong = iniciaSesionTokenKong.Result.sToken;
                    EntRequestHTTPActualizaMonedero datosMonedero = new EntRequestHTTPActualizaMonedero()
                    {
                        uIdMonedero = (Guid)datosUsuario.uIdMonedero,
                        sNumTelefono = datosUsuario.sTelefono,
                        sNombre = datosUsuario.sNombre,
                        sApellidoPaterno = datosUsuario.sApellidoPaterno,
                        sApellidoMaterno = datosUsuario.sApellidoMaterno,
                        sCorreo = datosUsuario.sCorreo,
                        dtFechaNacimiento = datosUsuario.dtFechaNacimiento

                    };

                    var monederoUpdate = await _busMonedero.BActualizaMonedero(datosMonedero, tokenKong);

                    if (monederoUpdate.HasError != true)
                    {
                        response.SetSuccess(monederoUpdate.Result);
                    }
                    else
                    {
                        response.ErrorCode = monederoUpdate.ErrorCode;
                        response.SetError(monederoUpdate.Message);
                    }

                    return response;
                }
                else
                {
                    response.ErrorCode = iniciaSesionTokenKong.ErrorCode;
                    response.SetError(iniciaSesionTokenKong.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463669136;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463669136, $"Error en {metodo}(EntUsuario datosUsuario): {ex.Message}", datosUsuario, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463653596, 67823463652819)]
        public async Task<IMDResponse<bool>> EnviarCorreoValidacionAbono(EntBusMessCorreoValidacionAbono entBusMessCorreoValidacionAbono)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntBusMessCorreoValidacionAbono entBusMessCorreoValidacionAbono)", entBusMessCorreoValidacionAbono));

            try
            {
                string msj = $"{Menssages.BusAbonoMonedero}";
                msj = msj
                    .Replace("{TD}", entBusMessCorreoValidacionAbono.sTipoTarjeta)
                    .Replace("{0}", entBusMessCorreoValidacionAbono.sNumeroTarjeta)
                    .Replace("{1}", entBusMessCorreoValidacionAbono.sMonto);

                bool _isHtml = false;

                string plantilla = null;
                try
                {
                    plantilla = _busLenguaje.BusSetLanguajeVentaSaldo();
                    plantilla = plantilla.Replace("{Nombre}", entBusMessCorreoValidacionAbono.sNombre);
                    plantilla = plantilla.Replace("{Mensaje}", msj);

                    plantilla = plantilla.Replace("{LabelOrigen}", Char.ToUpper(entBusMessCorreoValidacionAbono.sTipoTarjeta[0]) + entBusMessCorreoValidacionAbono.sTipoTarjeta.Substring(1));

                    plantilla = plantilla.Replace("{TO}", entBusMessCorreoValidacionAbono.sNumeroTarjeta);

                    plantilla = plantilla.Replace("{Monto}", "$" + entBusMessCorreoValidacionAbono.sMonto);
                    string serie = _busParametros.BObtener("APP_SERIE").Result.Result.sValor;
                    plantilla = plantilla.Replace("{Folio}", serie + "-" + entBusMessCorreoValidacionAbono.sFolio);
                    plantilla = plantilla.Replace("{Fecha}", entBusMessCorreoValidacionAbono.dtFecha.ToString("dd/MM/yyyy HH:mm"));
                    plantilla = plantilla.Replace("{Concepto}", entBusMessCorreoValidacionAbono.sConcepto);

                    string telefonoAtencionClientes = _busParametros.BObtener("ATI_ATENCION_CLIENTES_TELEFONO").Result.Result.sValor;
                    string emailAtencionClientes = _busParametros.BObtener("ATI_ATENCION_CLIENTES_EMAIL").Result.Result.sValor;

                    plantilla = plantilla.Replace("{Importante}", Menssages.HtmlImportantLabel.Replace("{telefono}", telefonoAtencionClientes).Replace("{email}", emailAtencionClientes));

                    _isHtml = true;
                }
                catch (Exception)
                {
                    plantilla = null;
                }

                EntBusMessCorreo busMessage = new EntBusMessCorreo
                {
                    uIdUsuario = entBusMessCorreoValidacionAbono.uIdUsuario,
                    sMensaje = _isHtml ? plantilla : msj,
                    sCorreoElectronico = entBusMessCorreoValidacionAbono.sCorreo,
                    bHtml = _isHtml
                };
                ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                {
                    Content = busMessage
                });

                response.SetSuccess(true, Menssages.BusCompleteCorrect);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntBusMessCorreoValidacionAbono entBusMessCorreoValidacionAbono): {ex.Message}", entBusMessCorreoValidacionAbono, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463655150, 67823463654373)]
        public async Task<IMDResponse<bool>> EnviarSmsValidacionAbono(EntBusMessSmsValidacionAbono entBusMessSmsValidacionAbono)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntBusMessSmsValidacionAbono entBusMessSmsValidacionAbono)", entBusMessSmsValidacionAbono));

            try
            {
                string mensaje = $"{Menssages.BusAbonoMonedero}";
                mensaje = mensaje
                    .Replace("{TD}", entBusMessSmsValidacionAbono.sTipoTarjeta)
                    .Replace("{0}", entBusMessSmsValidacionAbono.sNumeroTarjeta)
                    .Replace("{1}", entBusMessSmsValidacionAbono.sMonto);

                EntBusMessUsuarioCodigoVerificacion busMessage = new EntBusMessUsuarioCodigoVerificacion
                {
                    uIdUsuario = entBusMessSmsValidacionAbono.uIdUsuario,
                    sNumeroTelefono = $"+52{entBusMessSmsValidacionAbono.sNumeroTelefono}",
                    sMensaje = mensaje
                };
                ///Envío a bus evento notificación Sms 
                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionSms.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessUsuarioCodigoVerificacion>
                {
                    Content = busMessage
                });

                response.SetSuccess(true, Menssages.BusCompleteCorrect);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntBusMessSmsValidacionAbono entBusMessSmsValidacionAbono): {ex.Message}", entBusMessSmsValidacionAbono, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463656704, 67823463655927)]
        public async Task<IMDResponse<bool>> EnviarPushValidacionAbono(EntBusMessPushValidacionAbono entBusMessPushValidacionAbono)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntBusMessPushValidacionAbono entBusMessPushValidacionAbono)", entBusMessPushValidacionAbono));

            try
            {
                List<EntToken> lstEntToken = new List<EntToken>();

                foreach (var item in entBusMessPushValidacionAbono.lstTokens)
                {
                    EntToken entToken = new EntToken();
                    entToken.sToken = item;
                    entToken.uIdUsuario = entBusMessPushValidacionAbono.uIdUsuario;

                    lstEntToken.Add(entToken);
                }

                string mensaje = $"{Menssages.BusAbonoMonedero}";
                mensaje = mensaje
                    .Replace("{TD}", entBusMessPushValidacionAbono.sTipoTarjeta)
                    .Replace("{0}", entBusMessPushValidacionAbono.sNumeroTarjeta)
                    .Replace("{1}", entBusMessPushValidacionAbono.sMonto);

                EntNotificacionMultiplePush entNotificacionMultiPush = new EntNotificacionMultiplePush
                {
                    lstTokens = lstEntToken,
                    sTitulo = "Notificación",
                    sMensaje = mensaje
                };

                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionMultiplePush.GetDescription(), _exchangeConfig, new QueueMessage<EntNotificacionMultiplePush>
                {
                    Content = entNotificacionMultiPush
                });

                response.SetSuccess(true, Menssages.BusCompleteCorrect);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntBusMessPushValidacionAbono entBusMessPushValidacionAbono): {ex.Message}", entBusMessPushValidacionAbono, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463683122, 67823463682345)]
        public async Task<IMDResponse<List<EntFirebaseToken>>> BObtenerFireBaseToken(Guid uIdUsuario, int iTop = 0, string? sIdAplicacion = null)
        {
            IMDResponse<List<EntFirebaseToken>> response = new IMDResponse<List<EntFirebaseToken>>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                response = await _datUsuario.DGetFirebaseToken(uIdUsuario, iTop, sIdAplicacion);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463944194, 67823463943417)]
        public async Task<IMDResponse<bool>> EnviarMultipleCorreo(EntBusMessCorreoMultiples entBusMessMultipleCorreo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntBusMessCorreoMultiples entBusMessMultipleCorreo)", entBusMessMultipleCorreo));

            try
            {
                ///Envío a bus evento Nueva Sugerencias  
                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionMultipleEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreoMultiples>
                {
                    Content = entBusMessMultipleCorreo
                });

                response.SetSuccess(true, Menssages.BusCompleteCorrect);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntBusMessCorreoMultiples entBusMessMultipleCorreo): {ex.Message}", entBusMessMultipleCorreo, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823464912336, 67823464911559)]
        public async Task<IMDResponse<bool>> BNotificarTraspaso(string sMonederoOrigen, string sMonederoDestino, Guid uIdUsuario, TraspasoSaldoRequestModel traspasoSaldoRequestModel)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sMonederoOrigen, string sMonederoDestino, Guid uIdUsuario)", sMonederoOrigen, sMonederoDestino, uIdUsuario));

            try
            {
                string tipoOrigen = sMonederoOrigen.Split("-")[1] == "T" ? Menssages.BusCardTransfer : Menssages.BusWalledTransfer;
                string tipoDestino = sMonederoDestino.Split("-")[1] == "T" ? Menssages.BusCardTransfer : Menssages.BusWalledTransfer;

                string mensaje = Menssages.BusUsuarioMensajeNotificacionTraspaso;
                var usuario = await BGet(uIdUsuario);
                if (!usuario.HasError)
                {
                    mensaje = mensaje
                        .Replace("{0}", String.Format("{0:0.00}", traspasoSaldoRequestModel.MontoTransferencia))
                        .Replace("{1}", sMonederoOrigen.Split("-")[0])
                        .Replace("{2}", sMonederoDestino.Split("-")[0])
                        .Replace("{TO}", tipoOrigen)
                        .Replace("{TD}", tipoDestino);


                    bool enviar = _IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_MAIL;
                    if (enviar)
                    {
                        #region Notificacion email
                        string plantilla = null;
                        bool _isHtml = false;
                        try
                        {
                            plantilla = _busLenguaje.BusSetLanguajeTraspasoSaldo();
                            plantilla = plantilla.Replace("{Nombre}", (usuario.Result.sNombre + " " + usuario.Result.sApellidoPaterno + " " + usuario.Result.sApellidoMaterno).Trim());
                            plantilla = plantilla.Replace("{Mensaje}", mensaje);

                            plantilla = plantilla.Replace("{LabelOrigen}", Char.ToUpper(tipoOrigen[0]) + tipoOrigen.Substring(1));
                            plantilla = plantilla.Replace("{LabelDestino}", Char.ToUpper(tipoDestino[0]) + tipoDestino.Substring(1));

                            plantilla = plantilla.Replace("{TO}", sMonederoOrigen.Split("-")[0]);
                            plantilla = plantilla.Replace("{TD}", sMonederoDestino.Split("-")[0]);

                            plantilla = plantilla.Replace("{Monto}", "$" + String.Format("{0:0.00}", traspasoSaldoRequestModel.MontoTransferencia));
                            string serie = _busParametros.BObtener("APP_SERIE").Result.Result.sValor;
                            plantilla = plantilla.Replace("{Folio}", serie + "-" + traspasoSaldoRequestModel.folioMov.ToString());
                            plantilla = plantilla.Replace("{Fecha}", traspasoSaldoRequestModel.FechaOperacion.ToString("dd/MM/yyyy HH:mm"));

                            string telefonoAtencionClientes = _IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_TELEFONO;
                            string emailAtencionClientes = _IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_EMAIL;

                            plantilla = plantilla.Replace("{Importante}", Menssages.HtmlImportantLabel.Replace("{telefono}", telefonoAtencionClientes).Replace("{email}", emailAtencionClientes));

                            _isHtml = true;
                        }
                        catch (Exception)
                        {
                            plantilla = null;
                        }

                        EntBusMessCorreo busMessage = new EntBusMessCorreo
                        {
                            uIdUsuario = usuario.Result.uIdUsuario,
                            sMensaje = _isHtml ? plantilla : mensaje,
                            sCorreoElectronico = usuario.Result.sCorreo,
                            bHtml = _isHtml
                        };
                        ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                        await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                        {
                            Content = busMessage
                        });
                        #endregion
                    }

                    enviar = _IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_SMS;
                    if (enviar)
                    {
                        #region Notificacion SMS
                        EntBusMessUsuarioCodigoVerificacion busMessage = new EntBusMessUsuarioCodigoVerificacion
                        {
                            uIdUsuario = usuario.Result.uIdUsuario,
                            sNumeroTelefono = $"+52{usuario.Result.sTelefono}",
                            sMensaje = mensaje
                        };
                        ///Envío a bus evento notificación Sms 
                        await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionSms.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessUsuarioCodigoVerificacion>
                        {
                            Content = busMessage
                        });
                        #endregion
                    }

                    enviar = _IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_PUSH;
                    if (enviar)
                    {
                        int iDispositivos = _IMDParametroConfig.PARAMETRO_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH;
                        #region Notificacion PUSH
                        var resFirebaseToken = await BObtenerFireBaseToken(usuario.Result.uIdUsuario, iDispositivos);
                        if (resFirebaseToken.Result != null)
                        {
                            var lstEntToken = resFirebaseToken.Result.Select(item => new EntToken
                            {
                                sToken = item.sFcmToken,
                                uIdUsuario = usuario.Result.uIdUsuario
                            }).ToList();

                            EntNotificacionMultiplePush entNotificacionMultiPush = new EntNotificacionMultiplePush
                            {
                                lstTokens = lstEntToken,
                                sTitulo = Menssages.BusUsuarioMensajeNotificacionTraspasoTitulo,
                                sMensaje = mensaje
                            };

                            await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionMultiplePush.GetDescription(), _exchangeConfig, new QueueMessage<EntNotificacionMultiplePush>
                            {
                                Content = entNotificacionMultiPush
                            });
                        }
                        #endregion
                    }

                    response.SetSuccess(true, mensaje);
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sMonederoOrigen, string sMonederoDestino, Guid uIdUsuario): {ex.Message}", sMonederoOrigen, sMonederoDestino, uIdUsuario, ex, response));
            }
            return response;
        }

       

        [IMDMetodo(67823466282187, 67823466282964)]
        public async Task<IMDResponse<string>> BGeneraCodigoAlfanumerico(int iLength)
        {
            IMDResponse<string> response = new IMDResponse<string>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", iLength));
            try
            {
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Letras y números

                char[] code = new char[iLength];

                for (int i = 0; i < iLength; i++)
                {
                    code[i] = chars[random.Next(chars.Length)];
                }

                string codigo = new string(code);

                if (codigo.IsNullOrEmpty())
                    response.SetError(Menssages.ErrorGeneratedCode);

                response.SetSuccess(codigo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", iLength, ex, response));
            }

            return response;
        }

        [IMDMetodo(67823466283741, 67823466284518)]
        private async Task<IMDResponse<bool>> EnviarCodigoEliminacionCuenta(Guid uIdUsuario, string sCorreo, string sCode)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", uIdUsuario, sCorreo, sCode));
            try
            {
                string plantilla = _busLenguaje.BusSetLanguajeEliminaCuentaCode();
                plantilla = plantilla.Replace("{Code}", sCode);

                EntBusMessCorreo busMessage = new EntBusMessCorreo
                {
                    uIdUsuario = uIdUsuario,
                    sMensaje = plantilla,
                    sCorreoElectronico = sCorreo,
                    bHtml = true
                };
                ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                {
                    Content = busMessage
                });

                response.SetSuccess(true, Menssages.BusCompleteCorrect);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdUsuario, sCorreo, sCode, ex, response));
            }
            return response;
        }


        [IMDMetodo(67823466364549, 67823466365326)]
        private async Task<IMDResponse<bool>> BEnviarCodigoVerificacionEmailVigencia(Guid uIdUsuario, string sCorreo, string sCodigoVerificacion)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", uIdUsuario, sCorreo, sCodigoVerificacion));
            try
            {
                string plantilla = _busLenguaje.BusSetLanguajeVerificationCodeVigencia();
                plantilla = plantilla.Replace("{Code}", sCodigoVerificacion);

                EntBusMessCorreo busMessage = new EntBusMessCorreo
                {
                    uIdUsuario = uIdUsuario,
                    sMensaje = plantilla,
                    sCorreoElectronico = sCorreo,
                    bHtml = true
                };
                ///Envío a bus evento Nueva Cuenta Código de Verificacion  
                await _rabbitNotifications.SendAsync(RoutingKeys.NotificacionEmail.GetDescription(), _exchangeConfig, new QueueMessage<EntBusMessCorreo>
                {
                    Content = busMessage
                });

                response.SetSuccess(true, Menssages.BusCompleteCorrect);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdUsuario, sCorreo, sCodigoVerificacion, ex, response));
            }

            return response;
        }
        #endregion
    }
}
