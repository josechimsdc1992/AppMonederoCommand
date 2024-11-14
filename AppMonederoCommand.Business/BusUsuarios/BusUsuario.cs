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
            IBusParametros busParametros,
            IAuthService auth, IServGenerico servGenerico, IServAzureBlobStorage servAzureBlobStorage,
            IBusMonedero busMonedero, IBusLenguaje busLenguaje,
            IMDParametroConfig iMDParametroConfig)
        {
            this._logger = logger;
            this._datUsuario = datUusario;
            //Se rellena la variable para poder publicar en el bus
            this._rabbitNotifications = serviceProvider.GetRequiredService<IMDRabbitNotifications>();
            this._exchangeConfig = exchangeConfig;
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
