using AppMonederoCommand.Entities.Config;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AppMonederoCommand.Business.Monedero;
/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 07/09/2023 | César Cárdenas         | Creación
* ---------------------------------------------------------------------------------------
*      2        | 11/12/2023 | Oscar Luna             | Actualización
* ---------------------------------------------------------------------------------------
*/
public class BusMonedero : IBusMonedero
{
    private readonly ILogger<BusMonedero> _logger;
    private readonly IServGenerico _servGenerico;
    private readonly IDatFolio _datFolio;
    private readonly IBusParametros _busParametros;
    private readonly IMDRabbitNotifications _notificationsServiceIMD;
    private readonly ExchangeConfig _exchangeConfig;
    private readonly IDatMonedero _datMonedero;
    private readonly IBusMotivos _busMotivos;
    private readonly IBusTipoTarifa _busTipoTarifa;
    private readonly IBusTipoOperaciones _busTipoOperaciones;
    private readonly IDatUsuario _datUsuario;
    private readonly IMDServiceConfig _iMDServiceConfig;
    private readonly IMDParametroConfig _IMDParametroConfig;
    private readonly IBusTarjetas _busTarjetas;

    public BusMonedero(ILogger<BusMonedero> logger, IAuthService auth, IServGenerico servGenerico,
        IDatFolio datFolio,
        IBusParametros busParametros,
        IMDRabbitNotifications notificationsServiceIMD,
        IBusTipoOperaciones busTipoOperaciones,
        ExchangeConfig exchangeConfig, IDatMonedero datMonedero, IBusMotivos busMotivos, IBusTipoTarifa busTipoTarifa, IDatUsuario datUsuario,
        IMDParametroConfig iMDParametroConfig, IMDServiceConfig iMDServiceConfig, IBusTarjetas busTarjetas)
    {
        _logger = logger;
        _servGenerico = servGenerico;
        _datFolio = datFolio;
        _busParametros = busParametros;
        _notificationsServiceIMD = notificationsServiceIMD;
        _exchangeConfig = exchangeConfig;
        _datMonedero = datMonedero;
        _busMotivos = busMotivos;
        _busTipoTarifa = busTipoTarifa;
        _busTipoOperaciones = busTipoOperaciones;
        _datUsuario = datUsuario;
        _IMDParametroConfig = iMDParametroConfig;
        _iMDServiceConfig = iMDServiceConfig;
        _busTarjetas = busTarjetas;
    }

    [IMDMetodo(67823463373876, 67823463373099)]
    public async Task<(IMDResponse<bool>, IMDResponse<TraspasoSaldoRequestModel>)> BTransferirSaldo(EntTransferirSaldo entTransferir, string token)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();
        IMDResponse<TraspasoSaldoRequestModel> response02 = new IMDResponse<TraspasoSaldoRequestModel>();

        string mensajeErrorEstatus = string.Empty;
        long errorCodeEstatus = 0;

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntTransferirSaldo entTransferir, string token)", entTransferir, token));
        try
        {
            var _Operacion = _IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_DESCRIPCION;
            Guid _IdTipoOperacion = new Guid(_IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_GUID);
            //Obtiene la operacion
            List<EntTipoOperaciones> entTipoOperaciones = _IMDParametroConfig.TipoOperaciones;
            if (entTipoOperaciones != null && entTipoOperaciones.Count > 0)
            {
                var entOperacion = entTipoOperaciones.SingleOrDefault(x => x.sClave == OperacionesMovimientosMonedero.Traspaso.GetDescription());
                if (entOperacion != null)
                {
                    _Operacion = entOperacion.sNombre;
                    _IdTipoOperacion = entOperacion.uIdTipoOperacion;
                }
            }


            int? iTipoTarjetaDestino = null;
            int? iTipoTarjetaOrigen = null;
            //Valida tarjeta origen
            if (!string.IsNullOrEmpty(entTransferir.sNumeroTarjetaOrigen))
            {
                int numeroTarjetaOrigen = 0;
                int.TryParse(entTransferir.sNumeroTarjetaOrigen, out numeroTarjetaOrigen);

                IMDResponse<EntReadTarjetas> resTarjetaOrigen =await _busTarjetas.BGetByNumTarjeta(numeroTarjetaOrigen);
                
                if (!resTarjetaOrigen.HasError)
                {
                    iTipoTarjetaOrigen = resTarjetaOrigen.Result.entTipoTarifa.tipoTarjeta;
                }
                

                //var infoTarjeta = await BGetDatosByNumTarjeta(entTransferir.sNumeroTarjetaOrigen, token);
                //if (infoTarjeta != null && !infoTarjeta.HasError)
                //{
                //    iTipoTarjetaOrigen = infoTarjeta.Result.iTipoTarjeta;
                //}
                var estatusTarjeta = await BValidaEstatusTarjeta(entTransferir.sNumeroTarjetaOrigen, token, iTipoTarjetaOrigen);
                string mensajeTarjetaOrigen = Menssages.BusCardOrigin;
                if (estatusTarjeta.HasError == true)
                {
                    response.ErrorCode = estatusTarjeta.ErrorCode;

                    if (estatusTarjeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                    {
                        response.SetError(mensajeTarjetaOrigen + estatusTarjeta.Message);
                    }
                    else
                    {
                        response.SetError(estatusTarjeta.Message);
                    }

                    return (response, response02);
                }
                else
                {
                    //Se cargan las operaciones
                    var operacionesPermitidasTarjeta = estatusTarjeta.Result;

                    //Determina si puede traspasar la tarjeta
                    if (operacionesPermitidasTarjeta.sTodasOperaciones != OperacionesTarjeta.TodasOperaciones.GetDescription() || operacionesPermitidasTarjeta.sTodasOperaciones == null)
                    {
                        if (operacionesPermitidasTarjeta.sTraspasos != null)
                        {
                            if (operacionesPermitidasTarjeta.sTraspasos != OperacionesTarjeta.Traspasos.GetDescription())
                            {
                                response.ErrorCode = estatusTarjeta.ErrorCode;
                                response.SetError(mensajeTarjetaOrigen + " " + Menssages.NotAllowOperation);
                                return (response, response02);
                            }

                        }
                        else
                        {
                            response.ErrorCode = estatusTarjeta.ErrorCode;
                            response.SetError(mensajeTarjetaOrigen + " " + Menssages.NotAllowOperation);
                            return (response, response02);
                        }
                    }
                    //Termina la validacion de la tarjeta
                }

            }

            if (!string.IsNullOrEmpty(entTransferir.sNumeroTarjetaDestino))
            {
                int numeroTarjetaDestino = 0;
                int.TryParse(entTransferir.sNumeroTarjetaDestino, out numeroTarjetaDestino);

                IMDResponse<EntReadTarjetas> resTarjetaDestino = await _busTarjetas.BGetByNumTarjeta(numeroTarjetaDestino);

                if (!resTarjetaDestino.HasError)
                {
                    iTipoTarjetaDestino = resTarjetaDestino.Result.entTipoTarifa.tipoTarjeta;
                }

                var estatusTarjeta = await BValidaEstatusTarjeta(entTransferir.sNumeroTarjetaDestino, token, iTipoTarjetaDestino);

                string mensajeTarjetaDestino = Menssages.BusCardDestiny;
                if (estatusTarjeta.HasError == true)
                {
                    response.ErrorCode = estatusTarjeta.ErrorCode;

                    if (estatusTarjeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                    {
                        response.SetError(mensajeTarjetaDestino + estatusTarjeta.Message);
                    }
                    else
                    {
                        response.SetError(estatusTarjeta.Message);
                    }

                    return (response, response02);
                }
                else
                {
                    //Se cargan las operaciones
                    var operacionesPermitidasTarjeta = estatusTarjeta.Result;

                    //Determina si puede traspasar la tarjeta
                    if (operacionesPermitidasTarjeta.sTodasOperaciones != OperacionesTarjeta.TodasOperaciones.GetDescription() || operacionesPermitidasTarjeta.sTodasOperaciones == null)
                    {
                        if (operacionesPermitidasTarjeta.sTraspasos != null)
                        {
                            if (operacionesPermitidasTarjeta.sTraspasos != OperacionesTarjeta.Traspasos.GetDescription())
                            {
                                response.ErrorCode = estatusTarjeta.ErrorCode;
                                response.SetError(mensajeTarjetaDestino + " " + Menssages.NotAllowOperation);
                                return (response, response02);
                            }

                        }
                        else
                        {
                            response.ErrorCode = estatusTarjeta.ErrorCode;
                            response.SetError(mensajeTarjetaDestino + " " + Menssages.NotAllowOperation);
                            return (response, response02);
                        }
                    }
                    //Termina la validacion de la tarjeta
                }

            }
            //Termina valida tarjeta

            //Valida que estatus tiene el monedero origen
            if (iTipoTarjetaOrigen == null)
            {
                var infoMonederoOrigen = await BDatosMonedero(entTransferir.uIdMonederoOrigen);
                if (!infoMonederoOrigen.HasError)
                {
                    iTipoTarjetaOrigen=_IMDParametroConfig.TipoTarifas.Where(x => x.uIdTipoTarifa == infoMonederoOrigen.Result.idTipoTarifa).FirstOrDefault().iTipoTarjeta;
                    
                }
            }
            var estatusMonederoOrigen = await BEstatusMonedero(entTransferir.uIdMonederoOrigen, iTipoTarjetaOrigen);
            string mensajeMonederoOrigen = Menssages.BusMonederoOrigin;
            if (estatusMonederoOrigen.HasError == true)
            {
                response.ErrorCode = estatusMonederoOrigen.ErrorCode;
                response.SetError(mensajeMonederoOrigen + estatusMonederoOrigen.Message);

                if (estatusMonederoOrigen.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                {
                    response.SetError(mensajeMonederoOrigen + estatusMonederoOrigen.Message);
                }
                else
                {
                    response.SetError(estatusMonederoOrigen.Message);
                }

                return (response, response02);
            }

            var operacionesValidasOrigen = estatusMonederoOrigen.Result;

            if (operacionesValidasOrigen.sTodasOperaciones != OperacionesMonedero.TodasOperaciones.GetDescription() || operacionesValidasOrigen.sTodasOperaciones == null)
            {
                if (operacionesValidasOrigen.sTraspasos != OperacionesMonedero.Traspasos.GetDescription() || operacionesValidasOrigen.sTraspasos == null)
                {
                    response.ErrorCode = estatusMonederoOrigen.ErrorCode;
                    response.SetError(mensajeMonederoOrigen + " " + Menssages.NotAllowOperation);
                    return (response, response02);
                }
                else if (operacionesValidasOrigen.sTraspasos == OperacionesMonedero.Traspasos.GetDescription())
                {
                    mensajeErrorEstatus = mensajeMonederoOrigen + estatusMonederoOrigen.Message;
                    errorCodeEstatus = estatusMonederoOrigen.ErrorCode;
                }
            }

            //Valida que estatus tiene el monedero destino
            if (iTipoTarjetaDestino == null)
            {
                var infoMonederoDestino = await BDatosMonedero(entTransferir.uIdMonederoDestino);
                if (!infoMonederoDestino.HasError)
                {
                    iTipoTarjetaDestino = _IMDParametroConfig.TipoTarifas.Where(x => x.uIdTipoTarifa == infoMonederoDestino.Result.idTipoTarifa).FirstOrDefault().iTipoTarjeta;

                }
            }
            var estatusMonederoDestino = await BEstatusMonedero(entTransferir.uIdMonederoDestino, iTipoTarjetaDestino);
            string mensajeMonederoDestino = Menssages.BusMonederoDestiny;
            if (estatusMonederoOrigen.HasError == true)
            {
                response.ErrorCode = estatusMonederoDestino.ErrorCode;
                response.SetError(mensajeMonederoDestino + estatusMonederoDestino.Message);
                return (response, response02);
            }

            var operacionesValidasDestino = estatusMonederoDestino.Result;

            if (operacionesValidasDestino.sTodasOperaciones != OperacionesMonedero.TodasOperaciones.GetDescription() || operacionesValidasDestino.sTodasOperaciones == null)
            {
                if (operacionesValidasDestino.sTraspasos != OperacionesMonedero.Traspasos.GetDescription() || operacionesValidasDestino.sTraspasos == null)
                {
                    response.ErrorCode = estatusMonederoDestino.ErrorCode;
                    response.SetError(mensajeMonederoDestino + " " + Menssages.NotAllowOperation);
                    return (response, response02);
                }
                else if (operacionesValidasDestino.sTraspasos == OperacionesMonedero.Traspasos.GetDescription())
                {
                    if (errorCodeEstatus == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                    {
                        mensajeErrorEstatus = mensajeErrorEstatus + " & " + mensajeMonederoDestino + estatusMonederoDestino.Message;
                        errorCodeEstatus = estatusMonederoDestino.ErrorCode;
                    }
                    else
                    {
                        mensajeErrorEstatus = mensajeMonederoDestino + estatusMonederoDestino.Message;
                        errorCodeEstatus = estatusMonederoDestino.ErrorCode;
                    }


                }
            }
            //Termina validación del estatus del monedero
            var obtenerFolio = await BGeneraFolio(OperacionesMovimientosMonedero.Traspaso);

            if (obtenerFolio.HasError == true)
            {
                response.ErrorCode = obtenerFolio.ErrorCode;
                response.SetError(obtenerFolio.Message);
                return (response, response02);
            }

            TraspasoSaldoRequestModel body = new TraspasoSaldoRequestModel
            {
                MontoTransferencia = entTransferir.dImporte,
                IdMonederoOrigen = entTransferir.uIdMonederoOrigen,
                IdMonederoDestino = entTransferir.uIdMonederoDestino,
                Operacion = _Operacion,
                IdOperacion = Guid.NewGuid(),
                FechaOperacion = DateTime.Now,
                IdTipoOperacion = _IdTipoOperacion,
                Observaciones = "",
                folioMov = obtenerFolio.Result

            };
            response02.Result = body;
            string s = JsonConvert.SerializeObject(body);

            HttpClient httpClient = new HttpClient();

            var httpResponse = await _servGenerico.SPostBody(_iMDServiceConfig.MonederoC_Host, _iMDServiceConfig.MonederoC_Traspaso, body, token);

            if (httpResponse.HasError != true)
            {
                if (errorCodeEstatus != 0)
                {
                    response.ErrorCode = errorCodeEstatus;
                    response.SetSuccess(true, mensajeErrorEstatus);
                }
                else
                {
                    response.SetSuccess(true, Menssages.BusMessageTraspaso + entTransferir.sNumeroTarjetaDestino);
                }

            }
            else
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.SetError(httpResponse.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntTransferirSaldo entTransferir, string token): {ex.Message}", entTransferir, token, ex, response));
        }
        return (response, response02);
    }

    [IMDMetodo(67823463375430, 67823463374653)]
    public async Task<(IMDResponse<bool>, IMDResponse<AbonarSaldo>)> BAbonar(EntAbonar entAbonar, string token)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();
        IMDResponse<AbonarSaldo> response02 = new IMDResponse<AbonarSaldo>();
        string mensajeErrorEstatus = string.Empty;
        long errorCodeEstatus = 0;

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntAbonar entAbonar, string token)", entAbonar, token));
        try
        {
            string? _Operacion = _IMDParametroConfig.PARAMETRO_APP_ABONAR_DESCRIPCION;
            Guid _IdTipoOperacion = new Guid(_IMDParametroConfig.PARAMETRO_APP_ABONAR_GUID);

            //Obtiene la operacion
            List<EntTipoOperaciones> entTipoOperaciones = _IMDParametroConfig.TipoOperaciones;
            if(entTipoOperaciones != null && entTipoOperaciones.Count > 0)
            {
                var entOperacion = entTipoOperaciones.SingleOrDefault(x => x.sClave == OperacionesMovimientosMonedero.VentSaldo.GetDescription());
                if(entOperacion != null)
                {
                    _Operacion = entOperacion.sNombre;
                    _IdTipoOperacion = entOperacion.uIdTipoOperacion;
                }
            }

            //Valida tarjeta
            if (!string.IsNullOrEmpty(entAbonar.sNumeroTarjeta))
            {
                var estatusTarjeta = await BValidaEstatusTarjeta(entAbonar.sNumeroTarjeta, token);
                if (estatusTarjeta.HasError == true)
                {
                    response.ErrorCode = estatusTarjeta.ErrorCode;
                    string message = Menssages.BusCard;
                    if (estatusTarjeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                    {
                        message = message + estatusTarjeta.Message;
                    }
                    else
                    {
                        message = estatusTarjeta.Message;
                    }
                    response.SetError(message);
                    return (response, response02);
                }
                else
                {
                    //Se cargan las operaciones
                    var operacionesPermitidasTarjeta = estatusTarjeta.Result;

                    //Determina si puede sRecargar la tarjeta
                    if (operacionesPermitidasTarjeta.sTodasOperaciones != OperacionesTarjeta.TodasOperaciones.GetDescription() || operacionesPermitidasTarjeta.sTodasOperaciones == null)
                    {
                        if (operacionesPermitidasTarjeta.sRecarga != null)
                        {
                            if (operacionesPermitidasTarjeta.sRecarga != OperacionesTarjeta.Recarga.GetDescription())
                            {
                                response.ErrorCode = estatusTarjeta.ErrorCode;
                                string message = Menssages.BusCard;
                                if (estatusTarjeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                                {
                                    message = message + estatusTarjeta.Message;
                                }
                                else
                                {
                                    message = estatusTarjeta.Message;
                                }
                                response.SetError(message);
                                return (response, response02);
                            }

                        }
                        else
                        {
                            response.ErrorCode = estatusTarjeta.ErrorCode;
                            response.SetError(estatusTarjeta.Message);
                            return (response, response02);
                        }
                    }
                    //Termina la validacion de la tarjeta
                }

            }
            //Termina valida tarjeta
            //Valida que estatus tiene el monedero
            var estatusMonedero = await BEstatusMonedero(entAbonar.uIdMonedero);
            string mensajeMonedero = Menssages.BusMonedero;
            if (estatusMonedero.HasError == true)
            {
                response.ErrorCode = estatusMonedero.ErrorCode;

                if (estatusMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                {
                    response.SetError(mensajeMonedero + estatusMonedero.Message);
                }
                else
                {
                    response.SetError(estatusMonedero.Message);
                }

                return (response, response02);
            }


            var operacionesValidas = estatusMonedero.Result;

            if (operacionesValidas.sTodasOperaciones != OperacionesMonedero.TodasOperaciones.GetDescription() || operacionesValidas.sTodasOperaciones == null)
            {
                if (operacionesValidas.sRecarga != OperacionesMonedero.Recarga.GetDescription() || operacionesValidas.sRecarga == null)
                {
                    response.ErrorCode = estatusMonedero.ErrorCode;
                    response.SetError(mensajeMonedero + estatusMonedero.Message);
                    return (response, response02);
                }
                else if (operacionesValidas.sRecarga == OperacionesMonedero.Recarga.GetDescription())
                {
                    mensajeErrorEstatus = mensajeMonedero + estatusMonedero.Message;
                    errorCodeEstatus = estatusMonedero.ErrorCode;
                }
            }
            //Termina validación del estatus del monedero

            var obtenerFolio = await BGeneraFolio(OperacionesMovimientosMonedero.VentSaldo);

            if (obtenerFolio.HasError == true)
            {
                response.ErrorCode = obtenerFolio.ErrorCode;
                response.SetError(obtenerFolio.Message);
                return (response, response02);
            }

            AbonarSaldo body = new AbonarSaldo
            {
                monto = entAbonar.dImporte,
                idMonedero = entAbonar.uIdMonedero,
                operacion = _Operacion,
                idOperacion = entAbonar.uIdOperacion,
                fechaOperacion = DateTime.Now,
                idTipoOperacion = _IdTipoOperacion,
                idPaquete = entAbonar.uIdPaquete,
                observaciones = "",
                folioVenta = obtenerFolio.Result
            };
            response02.SetSuccess(body);

            string sJson = System.Text.Json.JsonSerializer.Serialize(body);
            _logger.LogInformation("Contenido entidad AbonarSaldo: " + sJson);

            HttpClient httpClient = new HttpClient();

            var httpResponse = await _servGenerico.SPostBody(_iMDServiceConfig.MonederoC_Host, _iMDServiceConfig.MonederoC_Abonar, body, token);

            if (httpResponse.HasError != true)
            {
                
                QueueMessage<EntUpdateFolioEstatusOrden> queue = new()
                {
                    Content = new()
                    {
                        sFolio = "F-" + body.folioVenta,
                        uIdOrden = body.idOperacion.ToString()
                    }
                };

                response.SetSuccess(true);

                _notificationsServiceIMD.SendAsync<EntUpdateFolioEstatusOrden>(AppMonederoCommand.Entities.Enums.RoutingKeys.EstatusOrdenCreacionFolio.GetDescription(), _exchangeConfig, queue);

                if (errorCodeEstatus != 0)
                {
                    response.ErrorCode = errorCodeEstatus;
                    response.SetSuccess(true, mensajeErrorEstatus);
                }
                else
                {
                    response.SetSuccess(true);
                }

            }
            else
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.SetError(httpResponse.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntAbonar entAbonar, string token): {ex.Message}", entAbonar, token, ex, response));
        }
        return (response, response02); ;
    }

    
    public async Task<IMDResponse<long>> BGeneraFolio(OperacionesMovimientosMonedero sOperacion)
    {
        IMDResponse<long> response = new IMDResponse<long>();

        string metodo = nameof(this.BGeneraFolio);
        _logger.LogInformation(IMDSerializer.Serialize(67823463733627, $"Inicia {metodo}({sOperacion})"));

        try
        {
            var existeFolio = await _datFolio.DGetFolio(sOperacion);

            response.SetSuccess(existeFolio.Result);
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823463734404;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823463734404, $"Error en {metodo}(): {ex.Message}", sOperacion, ex, response));
        }
        return response;
    }

    public async Task<IMDResponse<EntOperacionesPermitidasMonedero>> BEstatusMonedero(Guid uIdMonedero, int? iTipoTarjeta = null)
    {
        IMDResponse<EntOperacionesPermitidasMonedero> response = new IMDResponse<EntOperacionesPermitidasMonedero>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(67823464081723, $"Inicia {metodo}(Guid uIdMonedero, string token)", uIdMonedero));

        try
        {
            //Consultar info monedero
            var entInfoMonedero = await BConsultarMonedero(uIdMonedero);
            if (entInfoMonedero == null)
            {
                response.SetError(Menssages.BusNoExistMonedero);
                return response;
            }
            if (entInfoMonedero.HasError)
            {
                response.ErrorCode = entInfoMonedero.ErrorCode;
                response.SetError(entInfoMonedero.Message);
                return response;
            }

            response = await BEstatusMonedero(entInfoMonedero.Result, iTipoTarjeta);
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464082500;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464082500, $"Error en {metodo}(Guid uIdMonedero, string token): {ex.Message}", uIdMonedero, ex, response));
        }
        return response;
    }

   
    //Valida la tarjeta con llamado al servicio
    public async Task<IMDResponse<EntOperacionesPermitidasTarjeta>> BValidaEstatusTarjeta(string sNumeroTarjeta, string token, int? iTipoTarjeta = null)
    {
        IMDResponse<EntOperacionesPermitidasTarjeta> response = new IMDResponse<EntOperacionesPermitidasTarjeta>();

        string metodo = nameof(this.BValidaEstatusTarjeta);
        _logger.LogInformation(IMDSerializer.Serialize(67823464123681, $"Inicia {metodo}(string sNumeroTarjeta, string token)", sNumeroTarjeta, token));

        try
        {
            long numeroTarjeta = 0;
            long.TryParse(sNumeroTarjeta,out numeroTarjeta);
            EntOperacionesPermitidasTarjeta operacionesPermitidas = new EntOperacionesPermitidasTarjeta();


            IMDResponse<EntReadTarjetas> resTarjeta =await _busTarjetas.BGetByNumTarjeta(numeroTarjeta);
            if (!resTarjeta.HasError)
            {
                EntReadTarjetas tarjeta = resTarjeta.Result;
                if (tarjeta.bActivo == true && tarjeta.bBaja == false)
                {
                    switch (tarjeta.uIdEstatusTarjeta.ToString())
                    {
                        case EntConfiguracionEstatusTarjeta.sActiva:
                            operacionesPermitidas.sTodasOperaciones = OperacionesTarjeta.TodasOperaciones.GetDescription();
                            response.SetSuccess(operacionesPermitidas);
                            break;
                        case EntConfiguracionEstatusTarjeta.sInactiva:
                            response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                            response.SetError("esta inactiva");
                            return response;
                            break;
                        case EntConfiguracionEstatusTarjeta.sBloqueada:
                            if (tarjeta.entMotivos != null)
                            {
                                if (tarjeta.entMotivos.bPermitirReactivar == null)
                                {
                                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                    response.SetError(Menssages.BusCardBloq);// Motivo
                                    return response;
                                }
                                if (tarjeta.entMotivos.bPermitirReactivar == false)
                                {
                                    operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                    operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + tarjeta.entMotivos.sMotivo);
                                }
                                else
                                {
                                    if (tarjeta.entMotivos.bPermitirOperaciones == null)
                                    {
                                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                        response.SetError(Menssages.BusCardBloq);
                                        return response;
                                    }
                                    if (tarjeta.entMotivos.bPermitirOperaciones == true)
                                    {
                                        operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                        operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                        operacionesPermitidas.sRecarga = OperacionesTarjeta.Recarga.GetDescription();
                                        operacionesPermitidas.sTraspasos = OperacionesTarjeta.Traspasos.GetDescription();
                                        operacionesPermitidas.sVincular = OperacionesTarjeta.Vincular.GetDescription();
                                        operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                        response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + tarjeta.entMotivos.sMotivo);
                                    }
                                    else
                                    {
                                        operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                        operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                        operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                        response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + tarjeta.entMotivos.sMotivo);

                                    }
                                }

                            }
                            else
                            {
                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetError(Menssages.BusCardBloq);// Motivo
                                return response;
                            }
                            break;
                        default:
                            response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                            response.SetError(Menssages.BusNoCorrectStatusConfig);
                            return response;
                            break;
                    }
                }
                else
                {
                    if (tarjeta.bActivo == false && tarjeta.bBaja == true)
                    {
                        switch (tarjeta.uIdEstatusTarjeta.ToString())
                        {

                            case EntConfiguracionEstatusTarjeta.sBaja:
                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetError(Menssages.BusNoAviableSytem);
                                return response;
                                break;
                            default:
                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetError(Menssages.BusNoCorrectStatusConfig);
                                return response;
                                break;
                        }
                    }
                    else
                    {
                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                        response.SetError(Menssages.BusNoCorrectConfig);
                        return response;
                    }
                }

                //Ing. Benigno Manzano
                //Se agrega una validacion adicional en la cual se identifica que las de sin costo no se pueden realizar recargas ni traspasos
                if (iTipoTarjeta != null)
                {
                    if (eTipoTarjeta.SINCOSTO == (eTipoTarjeta)iTipoTarjeta)
                    {
                        //Si hay todas las operaciones se tiene que agregar las operaciones restantes y solo no incluir las que se bloquearian
                        if (operacionesPermitidas.sTodasOperaciones != null)
                        {
                            operacionesPermitidas.sTodasOperaciones = null;
                            operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                            operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                            operacionesPermitidas.sGenerarQR = OperacionesTarjeta.GenerarQR.GetDescription();
                            operacionesPermitidas.sVincular = OperacionesTarjeta.Vincular.GetDescription();
                            operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();
                        }
                        //Se quitan las operaciones no permitidas
                        else
                        {
                            operacionesPermitidas.sTodasOperaciones = null;
                            operacionesPermitidas.sRecarga = null;
                            operacionesPermitidas.sTraspasos = null;
                        }
                        response.SetSuccess(operacionesPermitidas);
                    }
                }
            }
            else
            {
                response.ErrorCode = resTarjeta.ErrorCode;
                response.SetError(resTarjeta.Message);
                return response;
            }

        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464124458;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464124458, $"Error en {metodo}(string sNumeroTarjeta, string token): {ex.Message}", sNumeroTarjeta, token, ex, response));
        }
        return response;
    }

    //Este metodo, se debera cambiar a un consumo local de base de datos
    [IMDMetodo(67823464750720, 67823464749943)]
    public async Task<IMDResponse<EntMonederoRes>> BDatosMonedero(Guid uIdMonedero)
    {
        IMDResponse<EntMonederoRes> response = new IMDResponse<EntMonederoRes>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMonedero)", uIdMonedero));

        try
        {
            var respMonedero = await BConsultarMonedero(uIdMonedero);
            if (respMonedero == null)
            {
                response.SetError(Menssages.BusNoExistMonedero);
                return response;
            }
            if (respMonedero.HasError)
            {
                response.ErrorCode = respMonedero.ErrorCode;
                response.SetError(respMonedero.Message);
                return response;
            }

            EntMonederoRes entMonederoRes = new EntMonederoRes()
            {
                idEstadoCuenta = respMonedero.Result.IdEstadoDeCuenta,
                idMonedero = respMonedero.Result.IdMonedero,
                numMonedero = long.Parse(respMonedero.Result.NumeroMonedero),
                idTipoTarifa = respMonedero.Result.IdTipoTarifa,
                tarifa = respMonedero.Result.TipoTarifa,
                saldo = respMonedero.Result.Saldo,
                idEstatus = respMonedero.Result.IdEstatusMonedero,
                estatus = respMonedero.Result.Estatus,
                telefono = respMonedero.Result.Telefono,
                activo = respMonedero.Result.Activo,
                baja = respMonedero.Result.Baja,
                fechaUltimoAbono = respMonedero.Result.FechaUltimoAbono,
                fechaUltimaOperacion = respMonedero.Result.FechaUltimaOperacion,
                fechaCreacion = respMonedero.Result.FechaCreacion,
                fechaVigencia = respMonedero.Result.FechaVigencia,
                nombreUsuario = respMonedero.Result.Nombre,
                apellidoPaterno = respMonedero.Result.ApellidoPaterno,
                apellidoMaterno = respMonedero.Result.ApellidoMaterno,
                correoUsuario = respMonedero.Result.Correo
            };

            response.SetSuccess(entMonederoRes);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMonedero): {ex.Message}", uIdMonedero, ex, response));
        }
        return response;
    }

    
    [IMDMetodo(67823465941861, 67823465942638)]
    public async Task<IMDResponse<bool>> BMonederoCreacion(EntCreateReplicaMonederos entMonederoNotificacion)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
        try
        {
            response = await _datMonedero.DMonederoCreacion(entMonederoNotificacion);
        }
        catch (Exception ex)
        {
            response.ErrorCode = 500;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entMonederoNotificacion, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465929429, 67823465930206)]
    public async Task<IMDResponse<EntInfoMonedero>> BConsultarMonedero(Guid uIdMonedero)
    {
        IMDResponse<EntInfoMonedero> response = new IMDResponse<EntInfoMonedero>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
        try
        {
            response = await _datMonedero.DConsultarMonedero(uIdMonedero);
        }
        catch (Exception ex)
        {
            response.ErrorCode = 500;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdMonedero, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465927875, 67823465928652)]
    public async Task<IMDResponse<EntOperacionesPermitidasMonedero>> BEstatusMonedero(EntInfoMonedero entInfoMonedero, int? iTipoTarjeta = null)
    {
        IMDResponse<EntOperacionesPermitidasMonedero> response = new IMDResponse<EntOperacionesPermitidasMonedero>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

        try
        {
            EntOperacionesPermitidasMonedero operacionesPermitidas = new EntOperacionesPermitidasMonedero();

            operacionesPermitidas.NoMonedero = entInfoMonedero.NumeroMonedero.ToString();
            //Valida que estatus tiene el monedero
            if (entInfoMonedero.Activo == true && entInfoMonedero.Baja == false)
            {
                EntMotivo motivo=null;
                if (entInfoMonedero.uIdMotivo != null)
                {
                    motivo = _IMDParametroConfig.Motivos.Where(x => x.uIdMotivo == entInfoMonedero.uIdMotivo.Value).FirstOrDefault();
                }
                switch (entInfoMonedero.IdEstatusMonedero.ToString())
                {
                    case EntConfiguracionEstatusMonedero.sActivo:
                        operacionesPermitidas.sTodasOperaciones = OperacionesMonedero.TodasOperaciones.GetDescription();
                        response.SetSuccess(operacionesPermitidas);
                        break;
                    case EntConfiguracionEstatusMonedero.sBloqueado:
                        if (motivo != null)
                        {
                            if (motivo.bPermitirReactivar == null)
                            {
                                response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                response.SetError(Menssages.BusMonederoBloqValid);// Motivo
                                return response;
                            }

                            if (motivo.bPermitirReactivar == false)
                            {
                                operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();

                                response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + motivo.sMotivo);
                            }
                            else
                            {
                                if (motivo.bPermitirOperaciones == null)
                                {
                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetError(Menssages.BusMonederoBloqValid);// Motivo
                                    return response;
                                }
                                if (motivo.bPermitirOperaciones == true)
                                {
                                    operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();
                                    operacionesPermitidas.sRecarga = OperacionesMonedero.Recarga.GetDescription();
                                    operacionesPermitidas.sTraspasos = OperacionesMonedero.Traspasos.GetDescription();
                                    operacionesPermitidas.sVerTarjetas = OperacionesMonedero.VerTarjetas.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + motivo.sMotivo);
                                }
                                else
                                {
                                    operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();
                                    operacionesPermitidas.sVerTarjetas = OperacionesMonedero.VerTarjetas.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + motivo.sMotivo);

                                }
                            }

                        }
                        else
                        {
                            response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                            response.SetError($"{Menssages.BusIsBloq}, {Menssages.BusNoMotivoBloq}");
                            return response;
                        }
                        break;
                    default:
                        response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                        response.SetError(Menssages.BusNoCorrectStatusConfig);
                        return response;
                }
            }
            else
            {
                if (entInfoMonedero.Activo == false && entInfoMonedero.Baja == true)
                {
                    switch (entInfoMonedero.IdEstatusMonedero.ToString())
                    {
                        case EntConfiguracionEstatusMonedero.sBaja:
                            response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                            response.SetSuccess(operacionesPermitidas, Menssages.BusMonedero + " " + Menssages.BusNoAviableSytem);
                            break;
                        default:
                            response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                            response.SetError(Menssages.BusNoCorrectStatusConfig);
                            break;
                    }
                }
                else
                {
                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                    response.SetError(Menssages.BusNoCorrectConfig);
                    return response;
                }
            }
            //Termina validación de  estatus de monedero
            //Ing. Benigno Manzano
            //Se agrega una validacion adicional en la cual se identifica que las de sin costo no se pueden realizar recargas ni traspasos
            if (iTipoTarjeta != null)
            {
                if (eTipoTarjeta.SINCOSTO == (eTipoTarjeta)iTipoTarjeta)
                {
                    //Si hay todas las operaciones se tiene que agregar las operaciones restantes y solo no incluir las que se bloquearian
                    if (operacionesPermitidas.sTodasOperaciones != null)
                    {
                        operacionesPermitidas.sTodasOperaciones = null;
                        operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                        operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();
                        operacionesPermitidas.sGenerarQR = OperacionesMonedero.GenerarQR.GetDescription();
                        operacionesPermitidas.sVerTarjetas = OperacionesMonedero.VerTarjetas.GetDescription();
                    }
                    //Se quitan las operaciones no permitidas
                    else
                    {
                        operacionesPermitidas.sRecarga = null;
                        operacionesPermitidas.sTraspasos = null;
                    }
                    response.SetSuccess(operacionesPermitidas);
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 500;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entInfoMonedero, iTipoTarjeta, ex, response));
        }
        return response;
    }


}

