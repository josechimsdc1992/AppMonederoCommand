namespace AppMonederoCommand.Business;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 06/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*      2        | 01/02/2024 | Neftalí Rodríguez      | Se envía la comisión al bus de eventos.
* ---------------------------------------------------------------------------------------
*/
public class BusOrden : IBusOrden
{
    private readonly ILogger<BusOrden> _logger;
    private readonly IServGenerico _servGenerico;
    private readonly IBusUsuario _busUsuario;
    private readonly IBusMonedero _busMonedero;
    private readonly IBusParametros _busParametros;
    private readonly IBusTarjetaUsuario _busTarjetaUsuario;
    private string URLBaseCommand;
    private string URLBaseQuerys;
    private string endCreateOrder;
    private string endGetOrder;
    private string endUpdateOrder;
    private string endGetOrderByRef;

    //Se agrega para enviar notificaciones al bus de eventos.
    private readonly ExchangeConfig _exchangeConfig;
    private readonly IMDRabbitNotifications _rabbitNotifications;

    public BusOrden(ILogger<BusOrden> logger, IServGenerico servGenerico, IBusUsuario busUsuario, IBusMonedero busMonedero, IBusParametros busParametros, ExchangeConfig exchangeConfig, IServiceProvider serviceProvider, IBusTarjetaUsuario busTarjetaUsuario)
    {
        URLBaseCommand = Environment.GetEnvironmentVariable("URLBASE_PAGOS") ?? string.Empty;
        URLBaseQuerys = Environment.GetEnvironmentVariable("URLBASE_PAGOS_QUERYS") ?? string.Empty;
        endCreateOrder = Environment.GetEnvironmentVariable("ENDPOINT_CREATE_ORDER") ?? string.Empty;
        endGetOrder = Environment.GetEnvironmentVariable("ENDPOINT_GET_ORDER_BY_KEY") ?? string.Empty;
        endUpdateOrder = Environment.GetEnvironmentVariable("ENDPOINT_UPDATE_ORDER") ?? string.Empty;
        endGetOrderByRef = Environment.GetEnvironmentVariable("ENDPOINT_GET_ORDER_BY_REF") ?? string.Empty;
        _logger = logger;
        _servGenerico = servGenerico;
        _busUsuario = busUsuario;
        _busMonedero = busMonedero;
        this._busParametros = busParametros;
        _exchangeConfig = exchangeConfig;
        _rabbitNotifications = serviceProvider.GetRequiredService<IMDRabbitNotifications>();
        _busTarjetaUsuario = busTarjetaUsuario;
    }

    [IMDMetodo(67823463303946, 67823463303169)]
    public async Task<IMDResponse<dynamic>> BCrearOrden(EntOrdenRequest entOrdenRequest, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntOrdenRequest entOrdenRequest, string token)", entOrdenRequest, token));

        try
        {
            if (entOrdenRequest.uIdUsuario == Guid.Empty)
            {
                var usuario = await _busUsuario.BGetByMonedero(entOrdenRequest.uIdMonedero);
                if (!usuario.HasError)
                {
                    entOrdenRequest.uIdUsuario = usuario.Result.uIdUsuario;
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                    return response;
                }
            }

            dynamic orden = new ExpandoObject();
            orden.IdUsuario = entOrdenRequest.uIdUsuario;
            orden.Concepto = entOrdenRequest.sConcepto;
            orden.Monto = entOrdenRequest.dMonto;
            orden.OpcionPago = entOrdenRequest.iOpcionPago;
            orden.IdPaquete = entOrdenRequest.uIdPaquete;
            orden.IdMonedero = entOrdenRequest.uIdMonedero;
            orden.EmailUsuario = entOrdenRequest.sEmailUsuario;
            orden.IdAplicacion = entOrdenRequest.sIdAplicacion;
            orden.InfoWeb = entOrdenRequest.entPagosWebInfoComprador;

            var apiResponse = await _servGenerico.SPostBody(URLBaseCommand, endCreateOrder, orden, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            var entOrden = JsonSerializer.Deserialize<EntOrden>(apiResponse.Result.ToString()!);

            response.SetSuccess(entOrden, Menssages.BusCompleteCorrect);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntOrdenRequest entOrdenRequest, string token): {ex.Message}", entOrdenRequest, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463305500, 67823463304723)]
    public async Task<IMDResponse<dynamic>> BObtener(Guid uIdOrden, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdOrden, string token)", uIdOrden, token));

        try
        {
            var apiResponse = await _servGenerico.SGetPath(URLBaseQuerys, endGetOrder, uIdOrden, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            var entOrden = JsonSerializer.Deserialize<EntOrden>(apiResponse.Result.ToString()!);

            response.SetSuccess(entOrden, Menssages.BusCompleteCorrect);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdOrden, string token): {ex.Message}", uIdOrden, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463307054, 67823463306277)]
    public async Task<IMDResponse<bool>> BActualizar(EntActualizarOrden entActualizarOrden, string token)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntActualizarOrden entActualizarOrden, string token)", entActualizarOrden, token));

        try
        {
            var resEntOrden = await BObtener(entActualizarOrden.uIdOrden, token);

            if (resEntOrden.HasError != true)
            {
                if (resEntOrden.Result.Pagado == true)
                {
                    response.SetError("La orden ya se encuentra pagada");
                    return response;
                }

                dynamic orden = new ExpandoObject();
                orden.IdOrden = entActualizarOrden.uIdOrden;
                orden.Pagado = true;
                orden.OpcionPago = resEntOrden.Result.OpcionPago;

                var apiResponse = await _servGenerico.SPutBody(URLBaseCommand, endUpdateOrder, orden, token);

                if (apiResponse.HasError)
                {
                    return response.GetResponse(apiResponse);
                }

                EntAbonar entAbonar = new EntAbonar
                {
                    uIdMonedero = resEntOrden.Result.IdMonedero,
                    uIdPaquete = resEntOrden.Result.IdPaquete,
                    dImporte = resEntOrden.Result.Monto,
                    uIdOperacion = entActualizarOrden.uIdOrden,
                    sReferencia = entActualizarOrden.sReferencia
                };

                var result = await _busMonedero.BAbonar(entAbonar, token);
                var resMonedero = result.Item1;
                var bodyAbono = result.Item2;

                if (resMonedero.HasError)
                {
                    return response.GetResponse(resMonedero);
                }

                //Se envia por el bus de eventos la comisión al estado de cuenta del monedero.
                List<EntOrdenDetalle> detalle = resEntOrden.Result.Detalle;

                if (detalle.Where(w => w.Comision == true).FirstOrDefault() != null)
                {
                    EntReplicaOrdenDetalle entReplicaOrdenDetalle = new EntReplicaOrdenDetalle
                    {
                        dComision = detalle.Where(w => w.Comision == true).FirstOrDefault().Monto,
                        iOpcionPago = (int)resEntOrden.Result.OpcionPago,
                        sConcepto = detalle.Where(w => w.Comision == true).FirstOrDefault().Concepto,
                        sOpcionPago = ((OpcionesPago)((int)resEntOrden.Result.OpcionPago)).GetDescription(),
                        sOrdenRef = resEntOrden.Result.OrdenRef,
                        uIdOperacion = Guid.Parse(resEntOrden.Result.IdOrden.ToString())
                    };

                    await _rabbitNotifications.SendAsync("Monedero.App.OrdenesDetalle", _exchangeConfig, new QueueMessage<EntReplicaOrdenDetalle>
                    {
                        Content = entReplicaOrdenDetalle
                    });
                }

                #region [Envio de correo, mensaje]
                try
                {
                    string tipoTarjeta = null, sNumeroTarjeta = null;

                    var tarjetaOrigen = _busTarjetaUsuario.BGetTarjetaByIdMonedero(resEntOrden.Result.IdMonedero).Result;
                    if (!tarjetaOrigen.HasError)
                    {
                        sNumeroTarjeta = tarjetaOrigen.Result.sNumeroTarjeta;
                        tipoTarjeta = Menssages.BusCardTransfer;
                    }
                    else
                    {
                        var monederoOrigen = await _busMonedero.BDatosMonedero(resEntOrden.Result.IdMonedero);
                        if (!monederoOrigen.HasError)
                        {
                            sNumeroTarjeta = monederoOrigen.Result.numMonedero.ToString();
                            tipoTarjeta = Menssages.BusWalledTransfer;
                        }
                    }

                    if (resEntOrden.Result.InfoWeb == null)
                    {
                        var resEntUsuario = await _busUsuario.BGet(entActualizarOrden.uIdUsuario);
                        if (resEntUsuario.HasError != true)
                        {
                            bool enviaValidacion = false;
                            enviaValidacion = bool.Parse(_busParametros.BObtener("APP_VALIDACION_ABONO_MAIL").Result.Result.sValor ?? "false");
                            if (enviaValidacion)
                            {
                                EntBusMessCorreoValidacionAbono entBusMessCoreoValidacionAbono = new EntBusMessCorreoValidacionAbono();
                                entBusMessCoreoValidacionAbono.uIdUsuario = resEntUsuario.Result.uIdUsuario;
                                entBusMessCoreoValidacionAbono.sCorreo = resEntUsuario.Result.sCorreo!;
                                entBusMessCoreoValidacionAbono.sMonto = String.Format("{0:0.00}", Convert.ToDecimal(resEntOrden.Result.Monto));
                                entBusMessCoreoValidacionAbono.sNombre = (resEntUsuario.Result.sNombre + " " + resEntUsuario.Result.sApellidoPaterno + " " + resEntUsuario.Result.sApellidoMaterno).Trim();
                                entBusMessCoreoValidacionAbono.sTipoTarjeta = tipoTarjeta;
                                entBusMessCoreoValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;
                                entBusMessCoreoValidacionAbono.sFolio = bodyAbono.Result.folioVenta.ToString();
                                entBusMessCoreoValidacionAbono.dtFecha = bodyAbono.Result.fechaOperacion;
                                entBusMessCoreoValidacionAbono.sConcepto = resEntOrden.Result.Concepto;

                                await _busUsuario.EnviarCorreoValidacionAbono(entBusMessCoreoValidacionAbono);
                            }

                            enviaValidacion = bool.Parse(_busParametros.BObtener("APP_VALIDACION_ABONO_SMS").Result.Result.sValor ?? "false");
                            if (enviaValidacion)
                            {
                                EntBusMessSmsValidacionAbono entBusMessSMSValidacionAbono = new EntBusMessSmsValidacionAbono();
                                entBusMessSMSValidacionAbono.uIdUsuario = resEntUsuario.Result.uIdUsuario;
                                entBusMessSMSValidacionAbono.sNumeroTelefono = resEntUsuario.Result.sTelefono!;
                                entBusMessSMSValidacionAbono.sMonto = String.Format("{0:0.00}", Convert.ToDecimal(resEntOrden.Result.Monto));
                                entBusMessSMSValidacionAbono.sTipoTarjeta = tipoTarjeta;
                                entBusMessSMSValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;

                                await _busUsuario.EnviarSmsValidacionAbono(entBusMessSMSValidacionAbono);
                            }

                            enviaValidacion = bool.Parse(_busParametros.BObtener("APP_VALIDACION_ABONO_PUSH").Result.Result.sValor ?? "false");
                            if (enviaValidacion)
                            {
                                int iDispositivos = int.Parse(_busParametros.BObtener("APP_DISPOSITIVOS_NOTIFICA_ABONO_PUSH").Result.Result.sValor ?? "2");
                                var resFirebaseToken = await _busUsuario.BObtenerFireBaseToken(resEntUsuario.Result.uIdUsuario, iDispositivos);

                                if (resFirebaseToken.Result != null)
                                {
                                    List<string> lstTokens = new List<string>();
                                    foreach (var item in resFirebaseToken.Result)
                                    {
                                        lstTokens.Add(item.sFcmToken);
                                    }

                                    EntBusMessPushValidacionAbono entBusMessPushValidacionAbono = new EntBusMessPushValidacionAbono();
                                    entBusMessPushValidacionAbono.uIdUsuario = resEntUsuario.Result.uIdUsuario;
                                    entBusMessPushValidacionAbono.lstTokens = lstTokens;
                                    entBusMessPushValidacionAbono.sMonto = String.Format("{0:0.00}", Convert.ToDecimal(resEntOrden.Result.Monto));
                                    entBusMessPushValidacionAbono.sTipoTarjeta = tipoTarjeta;
                                    entBusMessPushValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;

                                    await _busUsuario.EnviarPushValidacionAbono(entBusMessPushValidacionAbono);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Pago desde la web
                        EntBusMessCorreoValidacionAbono entBusMessCoreoValidacionAbono = new EntBusMessCorreoValidacionAbono();
                        entBusMessCoreoValidacionAbono.uIdUsuario = resEntOrden.Result.IdUsuario;
                        entBusMessCoreoValidacionAbono.sCorreo = resEntOrden.Result.InfoWeb.Email!;
                        entBusMessCoreoValidacionAbono.sMonto = String.Format("{0:0.00}", Convert.ToDecimal(resEntOrden.Result.Monto));
                        entBusMessCoreoValidacionAbono.sNombre = (resEntOrden.Result.InfoWeb.Nombre + " " + resEntOrden.Result.InfoWeb.ApellidoPaterno + " " + resEntOrden.Result.InfoWeb.ApellidoMaterno).Trim();
                        entBusMessCoreoValidacionAbono.sTipoTarjeta = tipoTarjeta;
                        entBusMessCoreoValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;
                        entBusMessCoreoValidacionAbono.sFolio = bodyAbono.Result.folioVenta.ToString();
                        entBusMessCoreoValidacionAbono.dtFecha = bodyAbono.Result.fechaOperacion;
                        entBusMessCoreoValidacionAbono.sConcepto = resEntOrden.Result.Concepto;

                        await _busUsuario.EnviarCorreoValidacionAbono(entBusMessCoreoValidacionAbono);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntActualizarOrden entActualizarOrden, string token): {ex.Message}", entActualizarOrden, ex, response));
                }
                #endregion

                response.SetSuccess(apiResponse.Result, Menssages.BusCompleteCorrect);
            }
            else
            {
                response.ErrorCode = resEntOrden.ErrorCode;
                response.SetError(resEntOrden.Message);
            }

        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntActualizarOrden entActualizarOrden, string token): {ex.Message}", entActualizarOrden, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463303946, 67823463303169)]
    public async Task<IMDResponse<dynamic>> BCrearOrdenCodi(EntOrdenCodiRequest entOrdenRequest, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntOrdenRequest entOrdenRequest, string token)", entOrdenRequest, token));

        try
        {
            Guid uIdUsuario = Guid.Empty;
            if (entOrdenRequest.uIdUsuario == uIdUsuario)
            {
                var user = await _busUsuario.BGetByMonedero(entOrdenRequest.uIdMonedero);
                if (user.HasError)
                {
                    response.SetError(Menssages.BusOrderUserNoAuthorized);
                    return response;
                }
                else
                {
                    uIdUsuario = user.Result.uIdUsuario;
                }
            }

            var iOpcionPago = await _busParametros.BObtener("OPCIONPAGOCODI");

            dynamic orden = new ExpandoObject();
            orden.IdUsuario = uIdUsuario;
            orden.Concepto = entOrdenRequest.sConcepto;
            orden.Monto = entOrdenRequest.dMonto;
            orden.OpcionPago = Convert.ToInt16(iOpcionPago.Result.sValor);
            orden.IdPaquete = entOrdenRequest.uIdPaquete;
            orden.IdMonedero = entOrdenRequest.uIdMonedero;
            orden.EmailUsuario = entOrdenRequest.sEmailUsuario;
            orden.InfoWeb = entOrdenRequest.entPagosWebInfoComprador;

            var apiResponse = await _servGenerico.SPostBody(URLBaseCommand, endCreateOrder, orden, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            var entOrden = JsonSerializer.Deserialize<EntOrdenCodi>(apiResponse.Result.ToString()!);

            response.SetSuccess(entOrden, Menssages.BusCompleteCorrect);

            return response;
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntOrdenRequest entOrdenRequest, string token): {ex.Message}", entOrdenRequest, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823464418164, 67823464417387)]
    public async Task<IMDResponse<dynamic?>> BObtenerByReferencia(string sReferencia, string token)
    {
        IMDResponse<dynamic?> response = new IMDResponse<dynamic?>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sReferencia, string token)", sReferencia, token));

        try
        {
            var apiResponse = await _servGenerico.SGetPath(URLBaseQuerys, endGetOrderByRef, sReferencia, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            var entOrden = JsonSerializer.Deserialize<EntOrdenCodi>(apiResponse.Result.ToString()!);

            response.SetSuccess(entOrden, Menssages.BusCompleteCorrect);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sReferencia, string token): {ex.Message}", sReferencia, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463307054, 67823463306277)]
    public async Task<IMDResponse<bool>> BActualizarForWebhooks(EntOrden entActualizarOrden, string token)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}(EntActualizarOrden entActualizarOrden, string token)", entActualizarOrden, token));

        try
        {
            EntAbonar entAbonar = new EntAbonar
            {
                uIdMonedero = (Guid)entActualizarOrden.IdMonedero,
                uIdPaquete = (Guid)entActualizarOrden.IdPaquete,
                dImporte = entActualizarOrden.Monto,
                uIdOperacion = entActualizarOrden.IdOrden,
                sReferencia = entActualizarOrden.OrdenRef
            };

            string sJson = JsonSerializer.Serialize(entAbonar);
            _logger.LogInformation("Contenido entidad EntAbonar: " + sJson);

            var result = await _busMonedero.BAbonar(entAbonar, token);
            var resMonedero = result.Item1;
            var bodyAbono = result.Item2;

            if (resMonedero.HasError)
            {
                return response.GetResponse(resMonedero);
            }

            //Se envia por el bus de eventos la comisión al estado de cuenta del monedero.
            List<EntOrdenDetalle> detalle = entActualizarOrden.Detalle;

            if (detalle.Where(w => w.Comision == true).FirstOrDefault() != null)
            {
                EntReplicaOrdenDetalle entReplicaOrdenDetalle = new EntReplicaOrdenDetalle
                {
                    dComision = detalle.Where(w => w.Comision == true).FirstOrDefault().Monto,
                    iOpcionPago = entActualizarOrden.OpcionPago,
                    sConcepto = detalle.Where(w => w.Comision == true).FirstOrDefault().Concepto,
                    sOpcionPago = ((OpcionesPago)entActualizarOrden.OpcionPago).GetDescription(),
                    sOrdenRef = entActualizarOrden.OrdenRef,
                    uIdOperacion = Guid.Parse(entActualizarOrden.IdOrden.ToString())
                };

                await _rabbitNotifications.SendAsync("Monedero.App.OrdenesDetalle", _exchangeConfig, new QueueMessage<EntReplicaOrdenDetalle>
                {
                    Content = entReplicaOrdenDetalle
                });
            }

            #region [Envio de correo, mensaje]
            try
            {
                string tipoTarjeta = null, sNumeroTarjeta = null;

                var tarjetaOrigen = _busTarjetaUsuario.BGetTarjetaByIdMonedero((Guid)entActualizarOrden.IdMonedero).Result;
                if (!tarjetaOrigen.HasError)
                {
                    sNumeroTarjeta = tarjetaOrigen.Result.sNumeroTarjeta;
                    tipoTarjeta = Menssages.BusCardTransfer;
                }
                else
                {
                    var monederoOrigen = await _busMonedero.BDatosMonedero((Guid)entActualizarOrden.IdMonedero);
                    if (!monederoOrigen.HasError)
                    {
                        sNumeroTarjeta = monederoOrigen.Result.numMonedero.ToString();
                        tipoTarjeta = Menssages.BusWalledTransfer;
                    }
                }

                if (entActualizarOrden.InfoWeb == null)
                {
                    var resEntUsuario = await _busUsuario.BGet(entActualizarOrden.IdUsuario);
                    if (resEntUsuario.HasError != true)
                    {
                        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Datos del usuario: ", resEntUsuario));

                        bool enviaValidacion = false;
                        enviaValidacion = bool.Parse(_busParametros.BObtener("APP_VALIDACION_ABONO_MAIL").Result.Result.sValor ?? "false");
                        if (enviaValidacion)
                        {
                            EntBusMessCorreoValidacionAbono entBusMessCoreoValidacionAbono = new EntBusMessCorreoValidacionAbono();
                            entBusMessCoreoValidacionAbono.uIdUsuario = resEntUsuario.Result.uIdUsuario;
                            entBusMessCoreoValidacionAbono.sNombre = resEntUsuario.Result.sNombre;
                            entBusMessCoreoValidacionAbono.sCorreo = resEntUsuario.Result.sCorreo!;
                            entBusMessCoreoValidacionAbono.sMonto = String.Format("{0:0.00}", entActualizarOrden.Monto);
                            entBusMessCoreoValidacionAbono.sTipoTarjeta = tipoTarjeta;
                            entBusMessCoreoValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;
                            entBusMessCoreoValidacionAbono.sFolio = bodyAbono.Result.folioVenta.ToString();
                            entBusMessCoreoValidacionAbono.dtFecha = bodyAbono.Result.fechaOperacion;
                            entBusMessCoreoValidacionAbono.sConcepto = entActualizarOrden.Concepto;

                            await _busUsuario.EnviarCorreoValidacionAbono(entBusMessCoreoValidacionAbono);
                        }

                        enviaValidacion = bool.Parse(_busParametros.BObtener("APP_VALIDACION_ABONO_SMS").Result.Result.sValor ?? "false");
                        if (enviaValidacion)
                        {
                            EntBusMessSmsValidacionAbono entBusMessSMSValidacionAbono = new EntBusMessSmsValidacionAbono();
                            entBusMessSMSValidacionAbono.uIdUsuario = resEntUsuario.Result.uIdUsuario;
                            entBusMessSMSValidacionAbono.sNumeroTelefono = resEntUsuario.Result.sTelefono!;
                            entBusMessSMSValidacionAbono.sMonto = String.Format("{0:0.00}", entActualizarOrden.Monto);
                            entBusMessSMSValidacionAbono.sTipoTarjeta = tipoTarjeta;
                            entBusMessSMSValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;

                            await _busUsuario.EnviarSmsValidacionAbono(entBusMessSMSValidacionAbono);
                        }

                        enviaValidacion = bool.Parse(_busParametros.BObtener("APP_VALIDACION_ABONO_PUSH").Result.Result.sValor ?? "false");
                        if (enviaValidacion)
                        {
                            int iDispositivos = int.Parse(_busParametros.BObtener("APP_DISPOSITIVOS_NOTIFICA_ABONO_PUSH").Result.Result.sValor ?? "2");
                            var resFirebaseToken = await _busUsuario.BObtenerFireBaseToken(resEntUsuario.Result.uIdUsuario, iDispositivos);

                            if (resFirebaseToken.Result != null)
                            {
                                List<string> lstTokens = new List<string>();
                                foreach (var item in resFirebaseToken.Result)
                                {
                                    lstTokens.Add(item.sFcmToken);
                                }

                                EntBusMessPushValidacionAbono entBusMessPushValidacionAbono = new EntBusMessPushValidacionAbono();
                                entBusMessPushValidacionAbono.uIdUsuario = resEntUsuario.Result.uIdUsuario;
                                entBusMessPushValidacionAbono.lstTokens = lstTokens;
                                entBusMessPushValidacionAbono.sMonto = String.Format("{0:0.00}", entActualizarOrden.Monto);
                                entBusMessPushValidacionAbono.sTipoTarjeta = tipoTarjeta;
                                entBusMessPushValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;

                                await _busUsuario.EnviarPushValidacionAbono(entBusMessPushValidacionAbono);
                            }
                        }
                    }
                }
                else
                {
                    EntBusMessCorreoValidacionAbono entBusMessCoreoValidacionAbono = new EntBusMessCorreoValidacionAbono();
                    entBusMessCoreoValidacionAbono.uIdUsuario = entActualizarOrden.IdUsuario;
                    entBusMessCoreoValidacionAbono.sNombre = (entActualizarOrden.InfoWeb.Nombre + " " + entActualizarOrden.InfoWeb.ApellidoPaterno + " " + entActualizarOrden.InfoWeb.ApellidoMaterno).Trim();
                    entBusMessCoreoValidacionAbono.sCorreo = entActualizarOrden.InfoWeb.Email;
                    entBusMessCoreoValidacionAbono.sMonto = String.Format("{0:0.00}", entActualizarOrden.Monto);
                    entBusMessCoreoValidacionAbono.sTipoTarjeta = tipoTarjeta;
                    entBusMessCoreoValidacionAbono.sNumeroTarjeta = sNumeroTarjeta;
                    entBusMessCoreoValidacionAbono.sFolio = bodyAbono.Result.folioVenta.ToString();
                    entBusMessCoreoValidacionAbono.dtFecha = bodyAbono.Result.fechaOperacion;
                    entBusMessCoreoValidacionAbono.sConcepto = entActualizarOrden.Concepto;

                    _logger.LogError(IMDSerializer.Serialize("BActualizarForWebhooks => entidad EntBusMessCorreoValidacionAbono" + entBusMessCoreoValidacionAbono));

                    await _busUsuario.EnviarCorreoValidacionAbono(entBusMessCoreoValidacionAbono);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}(EntActualizarOrden entActualizarOrden, string token): {ex.Message}", entActualizarOrden, ex, response));
            }
            #endregion

            response.SetSuccess(true, Menssages.BusCompleteCorrect);

        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}(EntActualizarOrden entActualizarOrden, string token): {ex.Message}", entActualizarOrden, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465373874, 67823465373097)]
    public async Task<IMDResponse<EntDatosTarjeta>> BGetDatosByNumTarjeta(string sNumMonedero, string token)
    {
        IMDResponse<EntDatosTarjeta> response = new IMDResponse<EntDatosTarjeta>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sNumMonedero, string token)", sNumMonedero, token));

        try
        {
            var monedero = await _busMonedero.BGetDatosByNumMonedero(sNumMonedero, token);

            if (!monedero.HasError)
            {
                response.SetSuccess(monedero.Result);
            }
            else
            {
                var tarjeta = await _busTarjetaUsuario.BGetDatosByNumTarjeta(sNumMonedero, token);
                if (!tarjeta.HasError)
                {
                    response.SetSuccess(tarjeta.Result);
                }
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

    [IMDMetodo(67823465401846, 67823465401069)]
    public async Task<IMDResponse<List<string>>> BOpcionesPago(string token)
    {
        IMDResponse<List<string>> response = new IMDResponse<List<string>>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string token)", token));

        try
        {
            var ruta = "Ordenes/obtener/opciones-pagos";
            var httpResponse = await _servGenerico.SGetPath(URLBaseCommand, ruta, token);
            if (!httpResponse.HasError)
            {
                var obj = JsonSerializer.Deserialize<List<string>>(httpResponse.Result.ToString()!);
                if (obj != null)
                {
                    response.SetSuccess(obj);
                }
                else
                {
                    response.SetError(Menssages.DatRegisterNoExist);
                }
            }
            else
            {
                response.SetError(httpResponse.Message);
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


}
