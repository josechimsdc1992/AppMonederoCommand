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
        private readonly IMDParametroConfig _IMDParametroConfig;

        public BusUsuario(ILogger<BusUsuario> logger, IDatUsuario datUusario,
            IServiceProvider serviceProvider, ExchangeConfig exchangeConfig, //Se agrega para crear poder enviar publicación al bus
            IBusParametros busParametros,
            
            IBusMonedero busMonedero, IBusLenguaje busLenguaje,
            IMDParametroConfig iMDParametroConfig)
        {
            this._logger = logger;
            this._datUsuario = datUusario;
            //Se rellena la variable para poder publicar en el bus
            this._rabbitNotifications = serviceProvider.GetRequiredService<IMDRabbitNotifications>();
            this._exchangeConfig = exchangeConfig;
            this._busParametros = busParametros;
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
        
        
        
        #endregion

        #region Utilidades
        
       
        
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

       

       
        #endregion
    }
}
