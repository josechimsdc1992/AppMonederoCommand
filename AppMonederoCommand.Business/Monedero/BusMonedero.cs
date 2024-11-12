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
    private readonly IDatTarjetaUsuario _datTarjetas;
    private readonly string _urlTarjeta = Environment.GetEnvironmentVariable("TARJETA_URL") ?? "";
    private readonly IDatFolio _datFolio;
    private readonly string _urlMonederoQ = Environment.GetEnvironmentVariable("MONEDEROQ_URL") ?? "";
    private readonly string _urlCatalogo = Environment.GetEnvironmentVariable("URLBASE_PAQUETES") ?? "";
    private readonly IBusParametros _busParametros;
    private readonly IMDRabbitNotifications _notificationsServiceIMD;
    private readonly ExchangeConfig _exchangeConfig;
    private readonly IDatMonedero _datMonedero;
    private readonly IBusMotivos _busMotivos;
    private readonly IBusTipoTarifa _busTipoTarifa;
    private readonly IBusTipoOperaciones _busTipoOperaciones;
    private readonly IDatUsuario _datUsuario;
    private readonly string _errorCodeSesion = Environment.GetEnvironmentVariable("ERROR_CODE_SESION") ?? "";
    private readonly IMDServiceConfig _iMDServiceConfig;
    private readonly IMDParametroConfig _IMDParametroConfig;
    private readonly IBusTarjetas _busTarjetas;

    public BusMonedero(ILogger<BusMonedero> logger, IAuthService auth, IServGenerico servGenerico,
        IDatTarjetaUsuario datTarjetas, IDatFolio datFolio,
        IBusParametros busParametros,
        IMDRabbitNotifications notificationsServiceIMD,
        IBusTipoOperaciones busTipoOperaciones,
        ExchangeConfig exchangeConfig, IDatMonedero datMonedero, IBusMotivos busMotivos, IBusTipoTarifa busTipoTarifa, IDatUsuario datUsuario,
        IMDParametroConfig iMDParametroConfig, IMDServiceConfig iMDServiceConfig, IBusTarjetas busTarjetas)
    {
        _logger = logger;
        _servGenerico = servGenerico;
        _datTarjetas = datTarjetas;
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

    public async Task<IMDResponse<EntSaldo>> BGetSaldo(Guid uIdMonedero, string token, string? NumeroTarjeta, Guid? uIdUsuario, string? sIdAplicacion = null)
    {
        IMDResponse<EntSaldo> response = new IMDResponse<EntSaldo>();
        EntSaldo entSaldo = new EntSaldo();

        string metodo = nameof(this.BGetSaldo);
        _logger.LogInformation(IMDSerializer.Serialize(67823464094155, $"Inicia {metodo}(Guid uIdMonedero, string token, string? NumeroTarjeta)", uIdMonedero, token, NumeroTarjeta));

        try
        {
            string message = null;

            if (sIdAplicacion != null)
            {
                if (uIdUsuario.HasValue)
                {
                    var entUsuarios = await _datUsuario.DGet(uIdUsuario.Value);

                    if (!entUsuarios.HasError && entUsuarios.Result != null)
                    {
                        if ((entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.BLOQUEADO || entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.DESBLOQUEADO) && entUsuarios.Result.sIdAplicacion != sIdAplicacion)
                        {
                            response.SetError(Menssages.BusLoginOtherDevice);
                            response.Result = new EntSaldo();
                            response.HttpCode = HttpStatusCode.PreconditionFailed;
                            response.ErrorCode = int.Parse(_errorCodeSesion);
                            return response;
                        }
                        if (entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.REPORTADO)
                        {
                            response.SetError(Menssages.BusBlockedAccountApp);
                            response.Result = new EntSaldo();
                            response.HttpCode = HttpStatusCode.PreconditionFailed;
                            response.ErrorCode = int.Parse(_errorCodeSesion);
                            return response;
                        }
                    }
                }
            }

            //Valida tarjeta
            if (!string.IsNullOrEmpty(NumeroTarjeta))
            {
                var estatusTarjeta = await BValidaEstatusTarjeta(NumeroTarjeta, token);
                if (estatusTarjeta.HasError == true)
                {
                    response.ErrorCode = estatusTarjeta.ErrorCode;
                    if (estatusTarjeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                    {
                        message = message + estatusTarjeta.Message;
                    }
                    else
                    {
                        message = estatusTarjeta.Message;
                    }
                    response.SetError(message);
                    return response;
                }
            }
            //Termina valida tarjeta

            //Valida que estatus tiene el monedero
            var estatusMonedero = await BEstatusMonedero(uIdMonedero);
            message = Menssages.BusMonedero;
            if (estatusMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
            {
                entSaldo.bBajaMonedero = true;
                estatusMonedero.Message = message + estatusMonedero.Message;
            }

            if (estatusMonedero.HasError == true)
            {
                response.ErrorCode = estatusMonedero.ErrorCode;
                if (estatusMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                {
                    message = message + estatusMonedero.Message;
                }
                else
                {
                    message = message + estatusMonedero.Message;
                }
                response.SetError(message);
                return response;
            }
            /*else
            {
                //Operaciones permitidas...
                var operaciones = await BValidaMonedero(uIdMonedero, token);
                if (!operaciones.HasError)
                {
                    entSaldo.entOperacionesMonedero = operaciones.Result;
                }
                else
                {
                    entSaldo.entOperacionesMonedero = new EntOperacionesMonedero();
                }
            }*/

            var httpResponseMonedero = await BDatosMonedero(uIdMonedero);

            if (httpResponseMonedero.HasError != true)
            {
                if (NumeroTarjeta == null)
                {
                    var httpResponseCatalogo = await BTipoTarifa(httpResponseMonedero.Result.idTipoTarifa);
                    //Ing. Benigno Manzano
                    //Se valida los monederos para que sepamos que operaciones se permiten
                    var operaciones = await BValidaMonederoV2(uIdMonedero, token, httpResponseCatalogo.Result.tipoTarjeta);
                    if (!operaciones.HasError)
                    {
                        entSaldo.entOperacionesMonedero = operaciones.Result;
                    }
                    else
                    {
                        entSaldo.entOperacionesMonedero = new EntOperacionesMonedero();
                    }

                    if (!httpResponseCatalogo.HasError)
                    {
                        entSaldo.dSaldo = httpResponseMonedero.Result.saldo;
                        entSaldo.sVigenciaTarjeta = httpResponseMonedero.Result.fechaVigencia;
                        entSaldo.uIdTipoTarifa = httpResponseMonedero.Result.idTipoTarifa;
                        entSaldo.sTipoTarifa = httpResponseCatalogo.Result.nombreTarifa;
                        entSaldo.iTipoTarjeta = httpResponseCatalogo.Result.tipoTarjeta;

                        response.SetSuccess(entSaldo, estatusMonedero.Message);
                        response.ErrorCode = estatusMonedero.ErrorCode;
                    }
                    else
                    {
                        response.ErrorCode = httpResponseCatalogo.ErrorCode;
                        response.SetError(httpResponseCatalogo.Message);
                    }
                }
                else
                {
                    Guid idUsuario = uIdUsuario != null ? Guid.Parse(uIdUsuario.ToString()) : Guid.NewGuid();

                    //Obtener las tarifas...
                    var tarifas = await BGetTipoTarias(token);
                    if (!tarifas.HasError)
                    {
                        var tarjetas = await _datTarjetas.DTarjetas(idUsuario);
                        if (!tarjetas.HasError)
                        {
                            entSaldo.dSaldo = httpResponseMonedero.Result.saldo;
                            entSaldo.sVigenciaTarjeta = httpResponseMonedero.Result.fechaVigencia;
                            entSaldo.uIdTipoTarifa = httpResponseMonedero.Result.idTipoTarifa;

                            var tarjeta = tarjetas.Result.Where(w => w.sNumeroTarjeta == NumeroTarjeta).FirstOrDefault();
                            var tipoTarifa = tarifas.Result.tipostarifa.Where(w => w.idTipoTarifa == tarjeta.uIdTipoTarifa.ToString()).FirstOrDefault();

                            if (tipoTarifa != null && tarjeta != null)
                            {
                                entSaldo.sTipoTarifa = tipoTarifa.nombreTarifa;
                                entSaldo.iTipoTarjeta = tipoTarifa.tipoTarjeta;
                            }
                            else
                            {
                                entSaldo.sTipoTarifa = "General";
                                entSaldo.iTipoTarjeta = 0;
                            }

                            //Ing. Benigno Manzano
                            //Se valida los monederos para que sepamos que operaciones se permiten
                            var operaciones = await BValidaMonederoV2(uIdMonedero, token, entSaldo.iTipoTarjeta);
                            if (!operaciones.HasError)
                            {
                                entSaldo.entOperacionesMonedero = operaciones.Result;
                            }
                            else
                            {
                                entSaldo.entOperacionesMonedero = new EntOperacionesMonedero();
                            }

                            response.SetSuccess(entSaldo, estatusMonedero.Message);
                            response.ErrorCode = estatusMonedero.ErrorCode;
                        }
                        else
                        {
                            response.ErrorCode = tarjetas.ErrorCode;
                            response.SetError(tarjetas.Message);
                        }
                    }
                    else
                    {

                        response.ErrorCode = tarifas.ErrorCode;
                        response.SetError(tarifas.Message);
                    }
                }
            }
            else
            {
                response.ErrorCode = httpResponseMonedero.ErrorCode;
                response.SetError(httpResponseMonedero.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464094932;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464094932, $"Error en {metodo}(Guid uIdMonedero, string token, string? NumeroTarjeta): {ex.Message}", uIdMonedero, token, NumeroTarjeta, ex, response));
        }
        return response;
    }

    public async Task<IMDResponse<EntMovimientos>> BConsultarMovimiento(Guid uIdMonedero, string FechaInicial, string FechaFinal, int numPagReq, int numRegReq, string token, string? NumeroTarjeta)
    {
        IMDResponse<EntMovimientos> response = new IMDResponse<EntMovimientos>();

        string metodo = nameof(this.BConsultarMovimiento);
        _logger.LogInformation(IMDSerializer.Serialize(67823464095709, $"Inicia {metodo}(Guid uIdMonedero, string FechaInicial, string FechaFinal, int numPagReq, int numRegReq, string token, string? NumeroTarjeta)", uIdMonedero, FechaInicial, FechaFinal, numPagReq, numRegReq, token, NumeroTarjeta));

        try
        {
            //Valida tarjeta
            if (!string.IsNullOrEmpty(NumeroTarjeta))
            {
                var estatusTarjeta = await BValidaEstatusTarjeta(NumeroTarjeta, token);
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
                        message = message + estatusTarjeta.Message;
                    }
                    response.SetError(message);
                    return response;
                }
            }
            //Termina valida tarjeta
            //Valida que estatus tiene el monedero
            var estatusMonedero = await BEstatusMonedero(uIdMonedero);
            if (estatusMonedero.HasError == true)
            {
                response.ErrorCode = estatusMonedero.ErrorCode;
                string message = Menssages.BusMonedero;
                if (estatusMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                {
                    message = message + estatusMonedero.Message;
                }
                else
                {
                    message = message + estatusMonedero.Message;
                }
                response.SetError(message);
                return response;
            }

            //Termina validación del estatus del monedero
            var body = new
            {
                idMonedero = uIdMonedero,
                numPag = numPagReq,
                numReg = numRegReq,
                fechaInicial = DateTime.ParseExact(FechaInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                fechaFinal = DateTime.ParseExact(FechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            };

            HttpClient httpClient = new HttpClient();
            string ruta = "movimiento";

            var httpResponse = await _servGenerico.SPostBody(_urlMonederoQ, ruta, body, token);

            List<EntTipoOperaciones> entTipoOperaciones = null;
            try { entTipoOperaciones = _busTipoOperaciones.BObtenerTipoOperaciones().Result.Result; } catch (Exception e) { }


            if (httpResponse.HasError != true)
            {
                var responseObject = JsonConvert.DeserializeObject<EntPagination<Movimiento>>(JsonConvert.SerializeObject(httpResponse.Result));
                var res = responseObject?.Datos;

                if (res == null || responseObject == null)
                {

                    var entPaginacion = new EntPaginacion
                    {
                        iNumPag = numPagReq,
                        iNumReg = numRegReq
                    };

                    var emptyMovimientos = new EntMovimientos
                    {
                        Operaciones = new List<Operaciones>(),
                        Paginacion = entPaginacion
                    };

                    response.SetSuccess(emptyMovimientos, Menssages.BusCompleteCorrect);
                    return response;
                }

                string serie = _busParametros.BObtener("APP_SERIE").Result.Result.sValor ?? "";

                //Obtener los datos del monedero...
                EntReadTipoTarifas entReadTipoTarifa = null;
                var datosMonedero = BDatosMonedero(uIdMonedero).Result;
                if (!datosMonedero.HasError)
                {
                    var tipoTarifa = await BTipoTarifa(datosMonedero.Result.idTipoTarifa);
                    if (tipoTarifa.HasError)
                    {
                        response.SetError(Menssages.BusNoTypeTarifaForMoney);
                        return response;
                    }
                    else
                    {
                        entReadTipoTarifa = tipoTarifa.Result;
                    }

                }
                else
                {
                    response.SetError(Menssages.BusNoDatsMoney);
                    return response;
                }

                //Obtener las ordenes...
                List<EntOrden> listaOrdenes = new List<EntOrden>();
                var listaIdsOrdenes = res.Where(w => w.TipoMovimiento == TipoMovimientos.ABONO.GetDescription()).Select(item => item.IdOperacion).ToList();
                if (listaIdsOrdenes.Count > 0)
                {
                    try
                    {
                        listaOrdenes = BObtenerByListOrdenes(listaIdsOrdenes, token).Result.Result;
                        if (listaOrdenes == null)
                        {
                            listaOrdenes = new List<EntOrden>();
                        }
                    }
                    catch
                    {
                        listaOrdenes = new List<EntOrden>();
                    }
                }

                List<Operaciones> operaciones = new List<Operaciones>();
                res.ToList().ForEach(item =>
                {
                    string sTipoOperacion = item.Operacion;
                    if(entTipoOperaciones != null)
                    {
                        var tipo = entTipoOperaciones.Where(x => x.uIdTipoOperacion == item.IdTipoOperacion).FirstOrDefault();
                        if(tipo != null)
                            sTipoOperacion = tipo.sNombre;
                    }
                    var _operacion = new Operaciones
                    {
                        uIdOperacion = serie + "-" + item.folioMovimiento,
                        sOperacion = sTipoOperacion,
                        sTipoMovimiento = item.TipoMovimiento,
                        dImporteGeneral = item.Monto,
                        iNumeroMonedero = item.NumeroMonedero.ToString(),
                        iTipoTarjeta = entReadTipoTarifa.tipoTarjeta,
                        sNombreTarifa = entReadTipoTarifa.nombreTarifa
                    };

                    var orden = listaOrdenes.Where(w => w.IdOrden == item.IdOperacion).FirstOrDefault();
                    if (orden != null)
                    {
                        _operacion.sConcepto = orden.Concepto;
                        _operacion.dImporte = orden.Detalle.Sum(s => s.Monto);
                        _operacion.dtFechaOperacion = item.FechaOperacion.ToString("yyyy-MM-ddTHH:mm:ss.fffff");
                        _operacion.BIsVentaSaldo = true;
                    }
                    else
                    {
                        _operacion.sConcepto = BObtenerConcepto(item.Operacion, item.TipoMovimiento, -1, item.NumeroMonedero.ToString(), out bool isVentaSaldo);
                        _operacion.dImporte = item.Monto;
                        _operacion.dtFechaOperacion = item.FechaOperacion.ToString("yyyy-MM-ddTHH:mm:ss.fffff");
                        _operacion.BIsVentaSaldo = isVentaSaldo;
                    }

                    operaciones.Add(_operacion);
                });

                var paginacion = new EntPaginacion
                {
                    iNumPag = responseObject.iPagina,
                    iNumReg = responseObject.iNumeroRegistros
                };

                var respuesta = new EntMovimientos
                {
                    Operaciones = operaciones.OrderByDescending(ord => ord.dtFechaOperacion).ToList(),
                    Paginacion = paginacion
                };

                response.SetSuccess(respuesta);

            }
            else
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.SetError(httpResponse.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464096486;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464096486, $"Error en {metodo}(Guid uIdMonedero, string FechaInicial, string FechaFinal, int numPagReq, int numRegReq, string token, string? NumeroTarjeta): {ex.Message}", uIdMonedero, FechaInicial, FechaFinal, numPagReq, numRegReq, token, NumeroTarjeta, ex, response));
        }
        return response;
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

    public async Task<decimal> MonederoSaldo(Guid uIdMonedero, string token)
    {
        var body = new
        {
            idMonedero = uIdMonedero,
            numPag = 1,
            numReg = 1
        };

        HttpClient httpClient = new HttpClient();
        string ruta = "vista-monedero";

        var httpResponseMonedero = await _servGenerico.SPostBody(_urlMonederoQ, ruta, body, token);

        if (httpResponseMonedero.HasError != true)
        {
            var responseObject = JsonConvert.DeserializeObject<EntPagination<EntMonederoRes>>(JsonConvert.SerializeObject(httpResponseMonedero.Result));
            var res = responseObject?.Datos?.FirstOrDefault();

            if (res == null || responseObject == null)
            {
                return 0;
            }

            return res.saldo;
        }

        return 0;
    }

    public async Task<IMDResponse<dynamic>> BActualizaMonedero(EntRequestHTTPActualizaMonedero reqActualizaMonedero, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        string metodo = nameof(this.BActualizaMonedero);
        _logger.LogInformation(IMDSerializer.Serialize(67823463651265, $"Inicia {metodo}(EntRequestHTTPActualizaMonedero reqActualizaMonedero, string token)", reqActualizaMonedero, token));

        try
        {
            string ruta = "actualizacion";


            string tokenKong = token;

            //Valida que estatus tiene el monedero
            var estatusMonedero = await BEstatusMonedero(reqActualizaMonedero.uIdMonedero);
            if (estatusMonedero.HasError == true)
            {
                response.ErrorCode = estatusMonedero.ErrorCode;
                response.SetError(estatusMonedero.Message);
                return response;
            }
            //Termina validación del estatus del monedero
            var httpResponse = await _servGenerico.SPutBodyModeroC(_iMDServiceConfig.MonederoC_Host, ruta, reqActualizaMonedero, tokenKong);

            if (httpResponse.HasError != true)
            {

                response.SetSuccess(httpResponse.Message);

            }
            else
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.SetError(httpResponse.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823463652042;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823463652042, $"Error en {metodo}(EntRequestHTTPActualizaMonedero reqActualizaMonedero, string token): {ex.Message}", reqActualizaMonedero, token, ex, response));
        }
        return response;
    }

    //Regresa a que servicios tiene acceso el monedero, este servicio obtiene la tarifa del usuario, se debe mejorar este servicio **********
    public async Task<IMDResponse<EntOperacionesMonedero>> BValidaMonedero(Guid uIdMonedero, string token)
    {
        IMDResponse<EntOperacionesMonedero> response = new IMDResponse<EntOperacionesMonedero>();

        string metodo = nameof(this.BValidaMonedero);
        _logger.LogInformation(IMDSerializer.Serialize(67823464156315, $"Inicia {metodo}(Guid uIdMonedero, string token)", uIdMonedero, token));

        try
        {
            //Ing. Benigno Manzano
            //Se valida los monederos para que sepamos que operaciones se permiten
            var httpResponseMonedero = BDatosMonedero(uIdMonedero).Result;
            string mensajeMonedero = Menssages.BusMonedero;
            if (httpResponseMonedero.HasError != true)
            {
                var httpResponseCatalogo = await BTipoTarifa(httpResponseMonedero.Result.idTipoTarifa);
                var resPermisos = await BValidaMonederoV2(uIdMonedero, token, httpResponseCatalogo.Result.tipoTarjeta);
                if (resPermisos.HasError != true)
                    response.SetSuccess(resPermisos.Result, resPermisos.Message);
                else
                {
                    response.ErrorCode = resPermisos.ErrorCode;
                    response.SetError(resPermisos.Message);
                }
            }
            else
            {
                response.ErrorCode = httpResponseMonedero.ErrorCode;
                response.SetError(httpResponseMonedero.Message);
            }
            /*
            var permisosMonedero = await BEstatusMonedero(uIdMonedero, token);
            string mensajeMonedero = Menssages.BusMonedero;
            if (permisosMonedero.HasError != true)
            {
                var permisos = permisosMonedero.Result;
                EntOperacionesMonedero resPermisos = new EntOperacionesMonedero();
                if (!(string.IsNullOrEmpty(permisos.sTodasOperaciones)))
                {
                    resPermisos.bTodasOperaciones = true;
                    resPermisos.bDetalles = true;
                    resPermisos.bMovimientos = true;
                    resPermisos.bRecarga = true;
                    resPermisos.bTraspasos = true;
                    resPermisos.bGenerarQR = true;
                    resPermisos.bVerTarjetas = true;
                }
                else
                {
                    resPermisos.bTodasOperaciones = (string.IsNullOrEmpty(permisos.sTodasOperaciones)) ? false : true;
                    resPermisos.bDetalles = (string.IsNullOrEmpty(permisos.sDetalles)) ? false : true;
                    resPermisos.bMovimientos = (string.IsNullOrEmpty(permisos.sMovimientos)) ? false : true;
                    resPermisos.bRecarga = (string.IsNullOrEmpty(permisos.sRecarga)) ? false : true;
                    resPermisos.bTraspasos = (string.IsNullOrEmpty(permisos.sTraspasos)) ? false : true;
                    resPermisos.bGenerarQR = (string.IsNullOrEmpty(permisos.sGenerarQR)) ? false : true;
                    resPermisos.bVerTarjetas = (string.IsNullOrEmpty(permisos.sVerTarjetas)) ? false : true;
                }


                response.ErrorCode = permisosMonedero.ErrorCode;
                response.SetSuccess(resPermisos, mensajeMonedero + permisosMonedero.Message);


            }
            else
            {
                response.ErrorCode = permisosMonedero.ErrorCode;

                if (permisosMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                {
                    response.SetError(mensajeMonedero + permisosMonedero.Message);
                }
                else
                {
                    response.SetError(permisosMonedero.Message);
                }

                return response;
            }
            */
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464157092;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464157092, $"Error en {metodo}(Guid uIdMonedero, string token): {ex.Message}", uIdMonedero, token, ex, response));
        }
        return response;
    }

    //Ing. Benigno Manzano
    //Se agrega un nueva validacion para que se reciba la El tipo de tarjeta que esto ayuda a otras validaciones
    public async Task<IMDResponse<EntOperacionesMonedero>> BValidaMonederoV2(Guid uIdMonedero, string token, int? iTipoTarjeta = null)
    {
        IMDResponse<EntOperacionesMonedero> response = new IMDResponse<EntOperacionesMonedero>();

        string metodo = nameof(this.BValidaMonederoV2);
        _logger.LogInformation(IMDSerializer.Serialize(67823464155795, $"Inicia {metodo}(Guid uIdMonedero, string token, int iTipoTarjeta)", uIdMonedero, token, iTipoTarjeta));

        try
        {
            var permisosMonedero = await BEstatusMonedero(uIdMonedero, iTipoTarjeta);
            string mensajeMonedero = Menssages.BusMonedero;
            if (permisosMonedero.HasError != true)
            {
                var permisos = permisosMonedero.Result;
                EntOperacionesMonedero resPermisos = new EntOperacionesMonedero();
                if (!(string.IsNullOrEmpty(permisos.sTodasOperaciones)))
                {
                    resPermisos.bTodasOperaciones = true;
                    resPermisos.bDetalles = true;
                    resPermisos.bMovimientos = true;
                    resPermisos.bRecarga = true;
                    resPermisos.bTraspasos = true;
                    resPermisos.bGenerarQR = true;
                    resPermisos.bVerTarjetas = true;
                }
                else
                {
                    resPermisos.bTodasOperaciones = (string.IsNullOrEmpty(permisos.sTodasOperaciones)) ? false : true;
                    resPermisos.bDetalles = (string.IsNullOrEmpty(permisos.sDetalles)) ? false : true;
                    resPermisos.bMovimientos = (string.IsNullOrEmpty(permisos.sMovimientos)) ? false : true;
                    resPermisos.bRecarga = (string.IsNullOrEmpty(permisos.sRecarga)) ? false : true;
                    resPermisos.bTraspasos = (string.IsNullOrEmpty(permisos.sTraspasos)) ? false : true;
                    resPermisos.bGenerarQR = (string.IsNullOrEmpty(permisos.sGenerarQR)) ? false : true;
                    resPermisos.bVerTarjetas = (string.IsNullOrEmpty(permisos.sVerTarjetas)) ? false : true;
                }


                response.ErrorCode = permisosMonedero.ErrorCode;
                response.SetSuccess(resPermisos, mensajeMonedero + permisosMonedero.Message);


            }
            else
            {
                response.ErrorCode = permisosMonedero.ErrorCode;

                if (permisosMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                {
                    response.SetError(mensajeMonedero + permisosMonedero.Message);
                }
                else
                {
                    response.SetError(permisosMonedero.Message);
                }

                return response;
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823499457092;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823499457092, $"Error en {metodo}(Guid uIdMonedero, string token, int iTipoTarjeta): {ex.Message}", uIdMonedero, token, iTipoTarjeta, ex, response));
        }
        return response;
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

    //Valida el estatus del monedro sin tener que realizar la llamada al servicio
    public async Task<IMDResponse<EntOperacionesPermitidasMonedero>> BEstatusMonedero(EntMonederoRes monedero)
    {
        IMDResponse<EntOperacionesPermitidasMonedero> response = new IMDResponse<EntOperacionesPermitidasMonedero>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(67823464083277, $"Inicia {metodo}(EntMonederoRes monedero)", monedero));

        try
        {
            EntOperacionesPermitidasMonedero operacionesPermitidas = new EntOperacionesPermitidasMonedero();


            //Validaque estatus tiene el monedero
            if (monedero.activo == true && monedero.baja == false)
            {
                switch (monedero.idEstatus.ToString())
                {
                    case EntConfiguracionEstatusMonedero.sActivo:
                        operacionesPermitidas.sTodasOperaciones = OperacionesMonedero.TodasOperaciones.GetDescription();
                        response.SetSuccess(operacionesPermitidas);
                        break;
                    case EntConfiguracionEstatusMonedero.sBloqueado:
                        if (monedero.entMotivos != null)
                        {
                            if (monedero.entMotivos.PermiteReactivar == null)
                            {
                                response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                response.SetError(Menssages.BusMonederoBloqValid);// Motivo
                                return response;
                            }

                            if (monedero.entMotivos.PermiteReactivar == false)
                            {
                                operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();

                                response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + monedero.entMotivos.Nombre);
                            }
                            else
                            {
                                if (monedero.entMotivos.PermiteOperaciones == null)
                                {
                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetError(Menssages.BusMonederoBloqValid);// Motivo
                                    return response;
                                }
                                if (monedero.entMotivos.PermiteOperaciones == true)
                                {
                                    operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();
                                    operacionesPermitidas.sRecarga = OperacionesMonedero.Recarga.GetDescription();
                                    operacionesPermitidas.sTraspasos = OperacionesMonedero.Traspasos.GetDescription();
                                    operacionesPermitidas.sVerTarjetas = OperacionesMonedero.VerTarjetas.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + monedero.entMotivos.Nombre);
                                }
                                else
                                {
                                    operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();
                                    operacionesPermitidas.sVerTarjetas = OperacionesMonedero.VerTarjetas.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + monedero.entMotivos.Nombre);

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
                        break;
                }
            }
            else
            {
                if (monedero.activo == false && monedero.baja == true)
                {
                    switch (monedero.idEstatus.ToString())
                    {
                        case EntConfiguracionEstatusMonedero.sBaja:
                            response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                            response.SetError(Menssages.BusNoAviableSytem);
                            return response;
                            break;
                        default:
                            response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                            response.SetError(Menssages.BusNoCorrectStatusConfig);
                            return response;
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

        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464084054;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464084054, $"Error en {metodo}(EntMonederoRes monedero): {ex.Message}", monedero, ex, response));
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
            int numeroTarjeta = 0;
            int.TryParse(sNumeroTarjeta,out numeroTarjeta);
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

    [IMDMetodo(67823464752274, 67823464751497)]
    public async Task<IMDResponse<EntReadTipoTarifas>> BTipoTarifa(Guid uIdTipoTarifa)
    {
        IMDResponse<EntReadTipoTarifas> response = new IMDResponse<EntReadTipoTarifas>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdTipoTarifa, string token)", uIdTipoTarifa));

        try
        {
            var resTarifa = await _busTipoTarifa.BObtenerTipoTarifa(uIdTipoTarifa);
            if (resTarifa == null)
            {
                response.SetError(Menssages.BusNoRegisters);
                return response;
            }
            if (resTarifa.HasError)
            {
                response.ErrorCode = resTarifa.ErrorCode;
                response.SetError(resTarifa.Message);
                return response;
            }

            EntReadTipoTarifas entReadTipoTarifas = new EntReadTipoTarifas()
            {
                idTipoTarifa = resTarifa.Result.uIdTipoTarifa.ToString(),
                nombreTarifa = resTarifa.Result.sTipoTarifa,
                claveTarifa = resTarifa.Result.sClaveTipoTarifa,
                tipoTarjeta = resTarifa.Result.iTipoTarjeta
            };

            response.SetSuccess(entReadTipoTarifas);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdTipoTarifa, string token): {ex.Message}", uIdTipoTarifa, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823464878148, 67823464877371)]
    public async Task<IMDResponse<List<EntOrden>>> BObtenerByListOrdenes(List<Guid>? uIdsOrdenes, string token)
    {
        IMDResponse<List<EntOrden>> response = new IMDResponse<List<EntOrden>>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(List<Guid> uIdsOrdenes, string token)", uIdsOrdenes, token));

        try
        {
            string URLBasePagosQuery = Environment.GetEnvironmentVariable("URLBASE_PAGOS_QUERYS") ?? string.Empty;
            string endGetOrdenesByList = Environment.GetEnvironmentVariable("ENDPOINT_GET_ORDENES_BY_LIST") ?? string.Empty;

            var apiResponse = await _servGenerico.SPostBody(URLBasePagosQuery, endGetOrdenesByList, uIdsOrdenes, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            var listaOrdenes = System.Text.Json.JsonSerializer.Deserialize<List<EntOrden>>(apiResponse.Result.ToString());

            response.SetSuccess(listaOrdenes);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(List<Guid> uIdsOrdenes, string token): {ex.Message}", uIdsOrdenes, token, ex, response));
        }
        return response;
    }

    private string BObtenerConcepto(string operacion, string tipo, int opcionPago, string NoMonedero, out bool isVentaSaldo)
    {
        string sConcepto = null;
        isVentaSaldo = false;

        OperacionesMovimientosMonedero enumOperacion = EmunExtensions.GetEnumValueFromDescription<OperacionesMovimientosMonedero>(operacion);

        switch (enumOperacion)
        {
            case OperacionesMovimientosMonedero.Desconocido:
                sConcepto = Menssages.BusMonederoConceptoDesconocido;
                break;
            case OperacionesMovimientosMonedero.Traspaso:
                sConcepto = tipo == TipoMovimientos.ABONO.GetDescription() ? Menssages.BusMonederoConceptoTraspasoAbono + ": " + NoMonedero : Menssages.BusMonederoConceptoTraspasoCargo + ": " + NoMonedero;
                break;
            case OperacionesMovimientosMonedero.VentQR:
                sConcepto = Menssages.BusMonederoConceptoVentaTicket;
                break;
            case OperacionesMovimientosMonedero.VentSaldo:
                isVentaSaldo = true;
                if (opcionPago == 1)
                {
                    sConcepto = Menssages.BusMonederoConceptoSaldoPayPal;
                }
                else if (opcionPago == 2)
                {
                    sConcepto = Menssages.BusMonederoConceptoSaldoMercadoPago;
                }
                else if (opcionPago == 3)
                {
                    sConcepto = Menssages.BusMonederoConceptoSaldoTarjeta;
                }
                else if (opcionPago == 4)
                {
                    sConcepto = Menssages.BusMonederoConceptoSaldoCoDi;
                }
                else
                {
                    sConcepto = Menssages.BusMonederoConceptoSaldo;
                }

                break;
            case OperacionesMovimientosMonedero.AbordajeNFC:
                sConcepto = Menssages.BusMonederoConceptoAbordaje;
                break;
            case OperacionesMovimientosMonedero.AbordajeQrApp:
                sConcepto = Menssages.BusMonederoConceptoAbordaje;
                break;
            case OperacionesMovimientosMonedero.VentTj:
                sConcepto = Menssages.BusMonederoConceptoVentaTarjeta;
                break;
        }

        return sConcepto;
    }

    [IMDMetodo(67823464736734, 67823464735957)]
    private async Task<IMDResponse<EntListTipoTarifas>> BGetTipoTarias(string token)
    {
        IMDResponse<EntListTipoTarifas> response = new IMDResponse<EntListTipoTarifas>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string token)", token));

        try
        {
            var ruta = "TipoTarifas/list";
            var httpResponseCatalogo = await _servGenerico.SGetPath(_urlCatalogo, ruta, token);

            if (httpResponseCatalogo.HasError != true)
            {
                string s = httpResponseCatalogo.Result.ToString();
                var obj = JsonConvert.DeserializeObject<EntListTipoTarifas>(httpResponseCatalogo.Result.ToString());

                if (obj == null)
                {
                    response.SetError("Sin respuesta exitosa del catalogo de Tipo Tarifa.");
                    return response;
                }
                else
                {
                    response.SetSuccess(obj);
                }
            }
            else
            {
                response.ErrorCode = httpResponseCatalogo.ErrorCode;
                response.SetError(httpResponseCatalogo.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string token): {ex.Message}", token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465372320, 67823465371543)]
    public async Task<IMDResponse<EntDatosTarjeta>> BGetDatosByNumMonedero(string sNumMonedero, string token)
    {
        IMDResponse<EntDatosTarjeta> response = new IMDResponse<EntDatosTarjeta>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sNumMonedero, string token)", sNumMonedero, token));

        try
        {
            var ruta = "vista-monedero";

            dynamic body = new ExpandoObject();
            body.numPag = 1;
            body.numReg = 10;
            body.numMonedero = sNumMonedero;

            var httpResponse = await _servGenerico.SPostBody(_urlMonederoQ, ruta, body, token);

            if (httpResponse.HasError != true)
            {
                string s = httpResponse.Result.ToString();
                dynamic json = JObject.Parse(s);

                if (json != null)
                {
                    if (json["datos"].Count > 0)
                    {
                        //Obtener la tarifa...
                        int iTipoTarjeta = 0;
                        var tarifa = await BTipoTarifa(Guid.Parse(json["datos"][0].idTipoTarifa.ToString()));
                        if (!tarifa.HasError)
                        {
                            iTipoTarjeta = tarifa.Result.tipoTarjeta;
                        }

                        var monedero = new EntDatosTarjeta
                        {
                            uIdMonedero = json["datos"][0].idMonedero,
                            NumeroTarjeta = json["datos"][0].numMonedero,
                            dSaldo = json["datos"][0].saldo,
                            iTipoTarjeta = iTipoTarjeta,
                            sEstatus = json["datos"][0].estatus
                        };

                        response.SetSuccess(monedero);
                    }
                    else
                    {
                        response.SetError("Sin respuesta exitosa del catalogo de monederos.");
                        return response;
                    }
                }
                else
                {
                    response.SetError("Sin respuesta exitosa del catalogo de monederos.");
                    return response;
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
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sNumMonedero, string token): {ex.Message}", sNumMonedero, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823456478246, 67823462478133)]
    public async Task<IMDResponse<EntDatosTarjeta>> BGetDatosByNumTarjeta(string sNumeroTarjeta, string token)
    {
        IMDResponse<EntDatosTarjeta> response = new IMDResponse<EntDatosTarjeta>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sNumeroTarjeta)", sNumeroTarjeta));

        try
        {
            var ruta = "Tarjetas/All";

            dynamic body = new ExpandoObject();
            body.numPag = 1;
            body.numReg = 10;
            body.iNumeroTarjeta = sNumeroTarjeta;

            var httpResponse = await _servGenerico.SPostBody(_urlTarjeta, ruta, body, token);

            if (httpResponse.HasError != true)
            {
                string s = httpResponse.Result.ToString();
                dynamic json = JObject.Parse(s);

                if (json != null)
                {
                    if (json["datos"].Count > 0)
                    {
                        //Obtener el saldo de la tarjeta...
                        decimal dSaldo = 0;
                        var datosMonedero = await BDatosMonedero(Guid.Parse(json["datos"][0].IdMonedero.ToString()));
                        if (!datosMonedero.HasError)
                        {
                            dSaldo = datosMonedero.Result.saldo;
                        }

                        //Obtener la tarifa...
                        int iTipoTarjeta = 0;
                        var tarifa = await BTipoTarifa(Guid.Parse(json["datos"][0].idTipoTarifa.ToString()));
                        if (!tarifa.HasError)
                        {
                            iTipoTarjeta = tarifa.Result.tipoTarjeta;
                        }

                        var monedero = new EntDatosTarjeta
                        {
                            uIdMonedero = json["datos"][0].IdMonedero,
                            NumeroTarjeta = json["datos"][0].NumeroTarjeta,
                            dSaldo = dSaldo,
                            iTipoTarjeta = iTipoTarjeta,
                            sEstatus = datosMonedero.Result.estatus
                        };

                        response.SetSuccess(monedero);
                    }
                    else
                    {
                        response.SetError("Sin respuesta exitosa del catalogo de tarjetas.");
                        return response;
                    }
                }
                else
                {
                    response.SetError("Sin respuesta exitosa del catalogo de tarjetas.");
                    return response;
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

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sNumeroTarjeta): {ex.Message}", sNumeroTarjeta, ex, response));
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
                IMDResponse<EntMotivo> resMotivo = new IMDResponse<EntMotivo>();
                if (entInfoMonedero.uIdMotivo != null)
                {
                    resMotivo = await _busMotivos.BObtenerMotivo(entInfoMonedero.uIdMotivo.Value);
                }
                switch (entInfoMonedero.IdEstatusMonedero.ToString())
                {
                    case EntConfiguracionEstatusMonedero.sActivo:
                        operacionesPermitidas.sTodasOperaciones = OperacionesMonedero.TodasOperaciones.GetDescription();
                        response.SetSuccess(operacionesPermitidas);
                        break;
                    case EntConfiguracionEstatusMonedero.sBloqueado:
                        if (resMotivo.Result != null)
                        {
                            if (resMotivo.Result.bPermitirReactivar == null)
                            {
                                response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                response.SetError(Menssages.BusMonederoBloqValid);// Motivo
                                return response;
                            }

                            if (resMotivo.Result.bPermitirReactivar == false)
                            {
                                operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();

                                response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + resMotivo.Result.sMotivo);
                            }
                            else
                            {
                                if (resMotivo.Result.bPermitirOperaciones == null)
                                {
                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetError(Menssages.BusMonederoBloqValid);// Motivo
                                    return response;
                                }
                                if (resMotivo.Result.bPermitirOperaciones == true)
                                {
                                    operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();
                                    operacionesPermitidas.sRecarga = OperacionesMonedero.Recarga.GetDescription();
                                    operacionesPermitidas.sTraspasos = OperacionesMonedero.Traspasos.GetDescription();
                                    operacionesPermitidas.sVerTarjetas = OperacionesMonedero.VerTarjetas.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + resMotivo.Result.sMotivo);
                                }
                                else
                                {
                                    operacionesPermitidas.sDetalles = OperacionesMonedero.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesMonedero.Movimientos.GetDescription();
                                    operacionesPermitidas.sVerTarjetas = OperacionesMonedero.VerTarjetas.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusMonedero.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, $"{Menssages.BusIsBloq}: " + resMotivo.Result.sMotivo);

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

