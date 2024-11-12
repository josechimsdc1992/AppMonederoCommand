namespace AppMonederoCommand.Business.BusBoletos
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 12/09/2023 | L.I. Oscar Luna       | Creación
    * ---------------------------------------------------------------------------------------
    *      2        | 11/12/2023 | L.I. Oscar Luna       | Actualización
    * ---------------------------------------------------------------------------------------
    *      3        | 23/01/2024 | L.I. Oscar Luna       | Generación de boleto con nuevo formato
    * ---------------------------------------------------------------------------------------
    */
    public class BusBoleto : IBusBoleto
    {
        private readonly ILogger<BusBoleto> _logger;
        private readonly IDatHistorialBoletoVirtual _datHistorialBoletoVirtual;
        private readonly IAuthService _auth;
        private readonly IServGenerico _servGenerico;
        //Para la validacion de tarjetas
        private readonly IBusTarjetaUsuario _busTarjetaUsuario;
        //Para consultar saldo
        private readonly IBusMonedero _busMonedero;
        //Se agrega para crear poder enviar publicación al bus
        private readonly ExchangeConfig _exchangeConfig;
        private readonly IMDRabbitNotifications _rabbitNotifications;
        //Parametros
        private readonly IBusParametros _busParametros;
        private readonly string _urlHttpClientBoletoVirtual;
        //Para consultar TipoTarifa
        private readonly IBusTipoTarifa _busTipoTarifa;
        private readonly IBusTicket _busTickets;
        private readonly IDatUsuario _datUsuario;
        private readonly string _errorCodeSesion = Environment.GetEnvironmentVariable("ERROR_CODE_SESION") ?? "";


        public BusBoleto(ILogger<BusBoleto> logger, IDatHistorialBoletoVirtual datHistorialBoletoVirtual, IAuthService auth, IServGenerico servGenerico,
            IBusMonedero busMonedero, IBusParametros busParametros,
            IServiceProvider serviceProvider, ExchangeConfig exchangeConfig, //Se agrega para crear poder enviar publicación al bus
            IBusTarjetaUsuario busTarjetaUsuario, IBusTipoTarifa busTipoTarifa, IBusTicket busTicket, IDatUsuario datUsuario
            )
        {
            this._logger = logger;
            this._datHistorialBoletoVirtual = datHistorialBoletoVirtual;
            this._auth = auth;
            this._servGenerico = servGenerico;
            _busMonedero = busMonedero;
            //Se rellena la variable para poder publicar en el bus
            this._rabbitNotifications = serviceProvider.GetRequiredService<IMDRabbitNotifications>();
            this._exchangeConfig = exchangeConfig;
            this._urlHttpClientBoletoVirtual = Environment.GetEnvironmentVariable("TICKETS_URL") ?? string.Empty;
            this._busParametros = busParametros;
            this._busTarjetaUsuario = busTarjetaUsuario;
            this._busTipoTarifa = busTipoTarifa;
            this._busTickets = busTicket;
            _datUsuario = datUsuario;
        }

        #region Métodos Service BusBoleto

        //Funcion modificada para que pueda consumir el metodo que consume al TicketService para el ajuste de Qr
        public async Task<IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>>> BCreateBoletoQR(EntRequestBoletoVirtual pSolicitud)
        {
            IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>> response = new IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>>();

            string metodo = nameof(this.BCreateBoletoQR);
            _logger.LogInformation(IMDSerializer.Serialize(67823464551031, $"Inicia {metodo}(EntRequestBoletoVirtual pSolicitud)", pSolicitud));

            try
            {
                var entUsuarios = await _datUsuario.DGet(pSolicitud.uIdUsuario!.Value);

                if (pSolicitud.sIdAplicacion != null)
                {
                    if ((entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.BLOQUEADO || entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.DESBLOQUEADO) && entUsuarios.Result.sIdAplicacion != pSolicitud.sIdAplicacion)
                    {
                        response.SetError(Menssages.BusLoginOtherDevice);
                        response.Result = new List<EntResponseHttpBoletoVirtualQR>();
                        response.HttpCode = HttpStatusCode.PreconditionFailed;
                        response.ErrorCode = int.Parse(_errorCodeSesion);
                        return response;
                    }
                    if (entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.REPORTADO)
                    {
                        response.SetError(Menssages.BusBlockedAccountApp);
                        response.Result = new List<EntResponseHttpBoletoVirtualQR>();
                        response.HttpCode = HttpStatusCode.PreconditionFailed;
                        response.ErrorCode = int.Parse(_errorCodeSesion);
                        return response;
                    }
                }

                //Consultar info monedero
                var entInfoMonedero = await _busMonedero.BConsultarMonedero(pSolicitud.uIdMonedero);
                if (entInfoMonedero.HasError)
                {
                    response.ErrorCode = entInfoMonedero.ErrorCode;
                    response.UserMessage = Menssages.BusErrorGetMonedero;
                    response.TypeCode = 2;
                    response.SetError(entInfoMonedero.Message);
                    return response;
                }

                var puedeMonederoGenerarQR = await BValidaMonedero(entInfoMonedero.Result);

                if (puedeMonederoGenerarQR.HasError == true || puedeMonederoGenerarQR.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                {
                    response.ErrorCode = puedeMonederoGenerarQR.ErrorCode;
                    response.UserMessage = Menssages.BusBoletoErrorValidaMonedero;
                    response.TypeCode = 2;
                    response.SetError(puedeMonederoGenerarQR.Message);
                    return response;
                }

                if (!string.IsNullOrEmpty(pSolicitud.sNumeroTarjeta))
                {
                    var puedeTarjetaGenerarQR = await BValidaTarjeta(pSolicitud.sNumeroTarjeta);
                    if (puedeMonederoGenerarQR.HasError == true)
                    {
                        response.ErrorCode = puedeTarjetaGenerarQR.ErrorCode;
                        response.UserMessage = Menssages.BusBoletoErrorValidaTarjeta;
                        response.TypeCode = 2;
                        response.SetError(puedeTarjetaGenerarQR.Message);
                        return response;
                    }
                }

                if (!(pSolicitud.iCantidad > 0))
                {
                    response.UserMessage = Menssages.BusErrorCountTickets;
                    response.TypeCode = 2;
                    response.SetError(Menssages.BusErrorCountTickets);
                    return response;
                }

                IMDResponse<EntParametros> maxBoletos = new IMDResponse<EntParametros>();
                IMDResponse<EntParametros> respTarifaGeneral = new IMDResponse<EntParametros>();
                respTarifaGeneral = await _busParametros.BObtener("APP_TARIFAGENERAL_GUID");
                var iIdTarifaGeneral = respTarifaGeneral.Result.sValor ?? Guid.Empty.ToString();
                if (Guid.Parse(iIdTarifaGeneral) == pSolicitud.uIdTarifa)
                {
                    maxBoletos = await _busParametros.BObtener("MAXBOLETOS_GRAL");
                }
                else
                {
                    maxBoletos = await _busParametros.BObtener("MAXBOLETOS");
                }

                if (maxBoletos.HasError != true)
                {

                    if (int.TryParse(maxBoletos.Result.sValor, out int maximo))
                    {
                        if (pSolicitud.iCantidad > maximo)
                        {
                            response.TypeCode = 1;
                            response.UserMessage = Menssages.BusAmountIsMayor;
                            response.SetError(Menssages.BusAmountIsMayor);
                            return response;
                        }
                    }
                    else
                    {
                        response.TypeCode = 2;
                        response.UserMessage = Menssages.BusValueNoValidTickets;
                        response.SetError(Menssages.BusValueNoValidTickets);
                        return response;
                    }
                }
                else
                {
                    response.TypeCode = 2;
                    response.UserMessage = Menssages.BusBoletoErrorService;
                    response.ErrorCode = maxBoletos.ErrorCode;
                    response.SetError(maxBoletos.Message);
                    return response;
                }

                EntRequestHttpBoletoVirtual requesHttpBoletoVirtual = new EntRequestHttpBoletoVirtual
                {
                    monedero = pSolicitud.uIdMonedero,
                    tarifa = pSolicitud.uIdTarifa,
                    saldo = entInfoMonedero.Result.Saldo,
                    cantidad = pSolicitud.iCantidad,
                    claveApp = pSolicitud.sIdAplicacion
                };

                string sJson = System.Text.Json.JsonSerializer.Serialize(requesHttpBoletoVirtual);
                _logger.LogInformation("Contenido requesHttpBoletoVirtual: " + sJson);

                var boletoCreado = await BHttpGeneraBoletoVirtual(requesHttpBoletoVirtual);//Solo se modifico el nombre del metodo

                if (!boletoCreado.HasError)
                {
                    var resTarifa = await _busTipoTarifa.BObtenerTipoTarifa(entInfoMonedero.Result.IdTipoTarifa);

                    var listaBoletos = boletoCreado.Result;

                    DateTime dtFechaServer = DateTime.Now;
                    listaBoletos.ForEach(boleto =>
                    {
                        boleto.iTipoTarifa = resTarifa.Result.iTipoTarjeta;
                        boleto.sTipoTarifa = ((eTipoTarjeta)boleto.iTipoTarifa).GetDescription();
                        boleto.dtFechaGeneracion = boleto.ListEntQR.EntSignatures.FirstOrDefault().dtValidFrom.ToUniversalTime();
                        boleto.dtFechaVigencia = boleto.ListEntQR.EntSignatures.LastOrDefault().dtValidUntil.ToUniversalTime();
                        boleto.dtFechaServidor = dtFechaServer;
                    });

                    response.SetSuccess(listaBoletos);
                }
                else
                {
                    //Manejo de errores...
                    int _typeCode = 0;
                    string _mensaje = BObtenerError((eErroresBoletos)boletoCreado.ErrorCode, out _typeCode);
                    response.ErrorCode = boletoCreado.ErrorCode;
                    response.TypeCode = _typeCode;
                    response.UserMessage = _mensaje;

                    response.SetError(boletoCreado.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823464551808;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823464551808, $"Error en {metodo}(EntRequestBoletoVirtual pSolicitud): {ex.Message}", pSolicitud, ex, response));
            }
            return response;
        }

        private string BObtenerError(eErroresBoletos eErrores, out int typeCode)
        {
            typeCode = 2;
            string messageError = null;
            switch (eErrores)
            {
                case eErroresBoletos.SALDO:
                    typeCode = 1;
                    messageError = Menssages.BusBoletoError100Saldo;
                    break;
                case eErroresBoletos.NOCANTIDAD:
                    typeCode = 1;
                    messageError = Menssages.BusBoletoError101NoCantidad;
                    break;
                case eErroresBoletos.MAXCANTIDAD:
                    typeCode = 1;
                    messageError = Menssages.BusBoletoError102MaxCantidad;
                    break;
                case eErroresBoletos.NOPAQUETE:
                    typeCode = 1;
                    messageError = Menssages.BusBoletoError103NoPaquete;
                    break;
                case eErroresBoletos.VIGENCIAPAQUETE:
                    typeCode = 1;
                    messageError = Menssages.BusBoletoError104VigenciaPaquete;
                    break;
                default:
                    typeCode = 2;
                    messageError = Menssages.BusBoletoErrorService;
                    break;
            }

            return messageError;
        }

        public async Task<IMDResponseQR<List<EntTicketQR>>> BListaBoleto(Guid uIdMonedero, string? sNumeroTarjeta, string? ClaveApp, Guid? uIdSolicitud)
        {
            IMDResponseQR<List<EntTicketQR>> response = new IMDResponseQR<List<EntTicketQR>>();

            string metodo = nameof(this.BListaBoleto);
            _logger.LogInformation(IMDSerializer.Serialize(67823464184287, $"Inicia {metodo}(Guid uIdMonedero, string? sNumeroTarjeta, string? ClaveApp)", uIdMonedero, sNumeroTarjeta, ClaveApp));

            try
            {

                //Consultar info monedero
                var entInfoMonedero = await _busMonedero.BConsultarMonedero(uIdMonedero);
                if (entInfoMonedero.HasError)
                {
                    response.ErrorCode = entInfoMonedero.ErrorCode;
                    response.UserMessage = Menssages.BusErrorGetMonedero;
                    response.TypeCode = 2;
                    response.SetError(entInfoMonedero.Message);
                    return response;
                }

                var puedeMonederoVerQR = await BValidaMonedero(entInfoMonedero.Result);

                if (puedeMonederoVerQR.HasError == true)
                {
                    response.TypeCode = 1;
                    response.UserMessage = Menssages.BusBoletoErrorValidaMonedero;
                    response.ErrorCode = puedeMonederoVerQR.ErrorCode;
                    response.SetError(puedeMonederoVerQR.Message);
                    return response;
                }

                if (!string.IsNullOrEmpty(sNumeroTarjeta))
                {
                    var puedeTarjetaVerQR = await BValidaTarjeta(sNumeroTarjeta);
                    if (puedeTarjetaVerQR.HasError == true)
                    {
                        response.TypeCode = 1;
                        response.UserMessage = Menssages.BusBoletoErrorValidaTarjeta;
                        response.ErrorCode = puedeTarjetaVerQR.ErrorCode;
                        response.SetError(puedeTarjetaVerQR.Message);
                        return response;
                    }
                }

                EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual = new EntRequestHttpListaBoletoVirtual
                {
                    uIdMonedero = uIdMonedero,
                    iUsado = 0,
                    iVigente = 1,
                    sClaveApp = ClaveApp,
                    uIdSolicitud = uIdSolicitud
                };

                var saldoActual = entInfoMonedero.Result.Saldo;
                int? tipoTarifa = null;
                var resTarifa = await _busTipoTarifa.BObtenerTipoTarifa(entInfoMonedero.Result.IdTipoTarifa);
                tipoTarifa = resTarifa.Result.iTipoTarjeta;

                /*if (saldoActual > 0)
                {
                    var resTarifa = await _busTipoTarifa.BObtenerTipoTarifa(entInfoMonedero.Result.IdTipoTarifa);
                    tipoTarifa = resTarifa.Result.iTipoTarjeta;
                }
                else
                {
                    response.TypeCode = 1;
                    response.UserMessage = Menssages.BusBoletoErrorValidaSaldo;
                    response.SetError(Menssages.BusNoObtainTarifa);
                    return response;
                }*/

                var obtenlistaBoletos = await BHttpListaBoletoVirtual(reqListaBoletoVirtual, eOpcionesTicket.Listar);

                if (!obtenlistaBoletos.HasError)
                {

                    List<EntTicketQR> listaBoletos = obtenlistaBoletos.Result;
                    DateTime dtFechaServer = DateTime.Now;
                    listaBoletos.ForEach(boleto =>
                    {
                        boleto.iTipoTarifa = tipoTarifa;
                        boleto.sTipoTarifa = ((eTipoTarjeta)boleto.iTipoTarifa).GetDescription();

                        //Se actualizan las fechas a UTC.
                        boleto.ListEntQR.dtDate = boleto.ListEntQR.dtDate.ToUniversalTime();
                        boleto.ListEntQR.EntSignatures.ForEach(date =>
                        {
                            date.dtValidFrom = date.dtValidFrom.ToUniversalTime();
                            date.dtValidUntil = date.dtValidUntil.ToUniversalTime();
                        });

                        List<EntSignatureQR> listQRs = boleto.ListEntQR.EntSignatures;

                        boleto.dtFechaGeneracion = listQRs.FirstOrDefault().dtValidFrom;
                        boleto.dtFechaVigencia = listQRs.LastOrDefault().dtValidUntil;
                        boleto.bBajaMonedero = puedeMonederoVerQR.ErrorCode == -1 ? true : false;

                        boleto.dtFechaServidor = dtFechaServer;
                    });

                    response.SetSuccess(listaBoletos.Where(x => x.dtFechaVigencia >= DateTime.UtcNow.AddSeconds(-5)).OrderBy(t => t.dtFechaGeneracion).ToList());
                }
                else
                {
                    if (obtenlistaBoletos.HttpCode == HttpStatusCode.NotFound)
                    {
                        response.SetSuccess(new List<EntTicketQR>());
                    }
                    else
                    {
                        response.TypeCode = 2;
                        response.UserMessage = Menssages.BusBoletoErrorService;
                        response.ErrorCode = obtenlistaBoletos.ErrorCode;
                        response.SetError(obtenlistaBoletos.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                response.TypeCode = 2;
                response.UserMessage = Menssages.BusBoletoErrorService;
                response.ErrorCode = 67823464185064;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823464185064, $"Error en {metodo}(Guid uIdMonedero, string? sNumeroTarjeta, string? ClaveApp): {ex.Message}", uIdMonedero, sNumeroTarjeta, ClaveApp, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465291512, 67823465290735)]
        public async Task<IMDResponseQR<bool>> BCancelarBoletos(Guid uIdMonedero, string ClaveApp, Guid uIdSolicitud)
        {
            IMDResponseQR<bool> response = new IMDResponseQR<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMonedero, string? ClaveApp)", uIdMonedero, ClaveApp));

            try
            {
                EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual = new EntRequestHttpListaBoletoVirtual
                {
                    uIdMonedero = uIdMonedero,
                    iUsado = 0,
                    iVigente = 1,
                    sClaveApp = ClaveApp,
                    uIdSolicitud = uIdSolicitud
                };

                var obtenlistaBoletos = await BHttpListaBoletoVirtual(reqListaBoletoVirtual, eOpcionesTicket.Listar);
                _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, "obtenlistaBoletos: ", obtenlistaBoletos));
                if (!obtenlistaBoletos.HasError)
                {
                    List<EntTicketQR> listaBoletos = obtenlistaBoletos.Result;
                    _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, "listaBoletos: ", listaBoletos));
                    if (listaBoletos.Count > 0)
                    {
                        var tickets = listaBoletos.Select(s => s.uIdTicket).ToList();
                        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, "tickets: ", tickets));

                        var responseHttpCancelar = await BHttpCancelarBoletos(new EntBoletoCancelar
                        {
                            Tickets = tickets
                        });

                        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, "responseHttpCancelar: ", responseHttpCancelar));

                        if (!responseHttpCancelar.HasError)
                        {
                            response.SetSuccess(true);
                        }
                        else
                        {
                            response.TypeCode = 2;
                            response.UserMessage = Menssages.BusBoletoErrorServiceCancelation;
                            response.ErrorCode = responseHttpCancelar.ErrorCode;
                            response.SetError(responseHttpCancelar.Message);
                        }
                    }
                    else
                    {
                        //Devuelve true porque no encontró registros para cancelar..
                        response.SetSuccess(true);
                    }
                }
                else
                {
                    if (obtenlistaBoletos.HttpCode == HttpStatusCode.NotFound)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.TypeCode = 2;
                        response.UserMessage = Menssages.BusBoletoErrorServiceCancelation;
                        response.ErrorCode = obtenlistaBoletos.ErrorCode;
                        response.SetError(obtenlistaBoletos.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMonedero, string? ClaveApp): {ex.Message}", uIdMonedero, ClaveApp, ex, response));
            }
            return response;
        }
        #endregion

        [IMDMetodo(67823465709538, 67823465708761)]
        public async Task<IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>>> BRegenerar(EntRequestBoletoVirtual entRequestBoletoVirtual)
        {
            IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>> response = new IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntRequestBoletoVirtual entRequestBoletoVirtual)", entRequestBoletoVirtual));

            try
            {
                EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual = new EntRequestHttpListaBoletoVirtual
                {
                    uIdMonedero = entRequestBoletoVirtual.uIdMonedero,
                    iUsado = 0,
                    iVigente = 1,
                    sClaveApp = entRequestBoletoVirtual.sIdAplicacion,
                    uIdSolicitud = entRequestBoletoVirtual.uIdSolicitud
                };

                var respCountTickets = await BGetCountBoletos(reqListaBoletoVirtual);

                if (!respCountTickets.HasError)
                {
                    var newTickets = new EntRequestBoletoVirtual
                    {
                        uIdUsuario = entRequestBoletoVirtual.uIdUsuario,
                        iCantidad = respCountTickets.Result,
                        sIdAplicacion = entRequestBoletoVirtual.sIdAplicacion,
                        sNumeroTarjeta = entRequestBoletoVirtual.sNumeroTarjeta,
                        uIdMonedero = entRequestBoletoVirtual.uIdMonedero,
                        uIdTarifa = entRequestBoletoVirtual.uIdTarifa,
                    };

                    response = await BCreateBoletoQR(newTickets);
                }
                else
                {
                    if (respCountTickets.HttpCode == HttpStatusCode.NotFound)
                    {
                        response.SetSuccess(new List<EntResponseHttpBoletoVirtualQR>());
                    }
                    else
                    {
                        response.TypeCode = 2;
                        response.UserMessage = Menssages.BusBoletoErrorService;
                        response.ErrorCode = respCountTickets.ErrorCode;
                        response.SetError(respCountTickets.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntRequestBoletoVirtual entRequestBoletoVirtual): {ex.Message}", entRequestBoletoVirtual, ex, response));
            }
            return response;
        }


        #region Request
        public async Task<IMDResponse<List<EntResponseHttpBoletoVirtualQR>>> BHttpGeneraBoletoVirtual(EntRequestHttpBoletoVirtual reqBoletoVirtual)
        {
            IMDResponse<List<EntResponseHttpBoletoVirtualQR>> response = new IMDResponse<List<EntResponseHttpBoletoVirtualQR>>();

            string metodo = nameof(this.BHttpGeneraBoletoVirtual);
            _logger.LogInformation(IMDSerializer.Serialize(67823464549477, $"Inicia {metodo}(EntRequestHttpBoletoVirtual reqBoletoVirtual)", reqBoletoVirtual));

            try
            {
                string ruta = "genera-qr/boleto-virtual";

                var iniciaSesionTokenKong = await _auth.BIniciarSesion();
                if (iniciaSesionTokenKong.HasError != true)
                {
                    string tokenKong = iniciaSesionTokenKong.Result.sToken;
                    var T = System.DateTime.UtcNow;//--> Note the initial Time
                    var httpResponse = await _servGenerico.SPostBody(_urlHttpClientBoletoVirtual, ruta, reqBoletoVirtual, tokenKong);
                    var TT = System.DateTime.UtcNow - T;//--> Note the Time Difference
                    _logger.LogInformation(IMDSerializer.Serialize(metodo, $"TIEMPO CONSUMO TICKETS: {TT}"));
                    if (httpResponse.HasError != true)
                    {
                        JArray ticketsHttp = (JArray)httpResponse.Result;
                        string jsonHttp = ticketsHttp.ToString();

                        List<EntResponseHttpBoletoVirtualQR> listaBoletos = ticketsHttp.ToObject<List<EntResponseHttpBoletoVirtualQR>>();
                        response.SetSuccess(listaBoletos);

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
                response.ErrorCode = 67823464550254;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823464550254, $"Error en {metodo}(EntRequestHttpBoletoVirtual reqBoletoVirtual): {ex.Message}", reqBoletoVirtual, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463481102, 67823463480325)]
        public async Task<IMDResponse<dynamic>> BSaldoMonedero(Guid uIdMonedero)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMonedero)", uIdMonedero));

            try
            {
                var iniciaSesionTokenKong = await _auth.BIniciarSesion();
                if (iniciaSesionTokenKong.HasError != true)
                {
                    string tokenKong = iniciaSesionTokenKong.Result.sToken;
                    var consultaSaldo = await _busMonedero.BGetSaldo(uIdMonedero, tokenKong, null, null);


                    if (consultaSaldo.HasError != true)
                    {
                        response.SetSuccess(consultaSaldo.Result);
                    }
                    else
                    {
                        response.ErrorCode = consultaSaldo.ErrorCode;
                        response.SetError(consultaSaldo.Message);
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
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMonedero): {ex.Message}", uIdMonedero, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463283744, 67823463282967)]
        public async Task<IMDResponse<dynamic>> BHttpListaBoletoVirtual(EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual, eOpcionesTicket opcion)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual)", reqListaBoletoVirtual));

            try
            {
                var maxListaBoletos = await _busParametros.BObtener("MAXLISTABOLETOS");
                int? maximoListado = null;
                if (maxListaBoletos.HasError != true)
                {

                    if (int.TryParse(maxListaBoletos.Result.sValor, out int maximo))
                    {
                        maximoListado = maximo;
                    }
                    else
                    {
                        response.SetError("El valor del parámetro de cantidad máxima de boletos a listar no es un número válido.");
                        return response;
                    }
                }
                else
                {
                    response.ErrorCode = maxListaBoletos.ErrorCode;
                    response.SetError(maxListaBoletos.Message);
                    return response;
                }

                #region Consulta BD Tickets
                EntConsultaTickets entReplicaTicket = new EntConsultaTickets()
                {
                    uIdMonedero = reqListaBoletoVirtual.uIdMonedero,
                    claveApp = reqListaBoletoVirtual.sClaveApp,
                    uIdSolicitud = reqListaBoletoVirtual.uIdSolicitud,
                    bUsado = reqListaBoletoVirtual.iUsado != 0,
                    bVigente = reqListaBoletoVirtual.iVigente != 0,
                    skip = 0,
                    take = maximoListado != null ? maximoListado.Value : 0,
                    sOpcion = opcion
                };

                var respTickets = await _busTickets.BGetListadoTickets(entReplicaTicket);
                _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Termina llamada a listado de tickets"));
                if (respTickets.HasError)
                {
                    response.ErrorCode = respTickets.ErrorCode;
                    response.SetError(respTickets.Message);
                    return response;
                }

                List<EntTicketResponse> lstTickets = new List<EntTicketResponse>();
                lstTickets = respTickets.Result;
                #endregion

                if (lstTickets.Count > 0)
                {
                    List<EntTicketQR> boletosVirtuales = new List<EntTicketQR>();
                    foreach (var ticket in lstTickets)
                    {
                        List<EntSignatureQR> signature = new List<EntSignatureQR>();
                        var lisSignature = ticket.qr.signatures;
                        foreach (var signatureItem in lisSignature)
                        {
                            EntSignatureQR entSignatureQR = new EntSignatureQR
                            {
                                sData = signatureItem.data,
                                dtValidFrom = signatureItem.validFrom,
                                dtValidUntil = signatureItem.validUntil
                            };
                            signature.Add(entSignatureQR);
                        }

                        var qr = new EntQR
                        {
                            sBaseQrCode = ticket.qr.baseQrCode,
                            EntSignatures = signature,
                            dtDate = ticket.qr.date,
                            sPanHash = ticket.qr.panHash
                        };

                        boletosVirtuales.Add(new EntTicketQR
                        {
                            uIdTicket = ticket.uIdTicket,
                            dtFechaGeneracion = ticket.dtFechaCreacion,
                            dtFechaVigencia = ticket.dtFechaVigencia,
                            ListEntQR = qr,
                            uIdSolicitud = ticket.uIdSolicitud,
                            sClaveApp = ticket.claveApp
                        });

                    }
                    response.SetSuccess(boletosVirtuales);
                }
                else
                {
                    response.SetNotFound(Menssages.BusNoticketsAviables);

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual): {ex.Message}", reqListaBoletoVirtual, ex, response));

            }
            return response;
        }

        [IMDMetodo(67823465293066, 67823465292289)]
        public async Task<IMDResponse<bool>> BHttpCancelarBoletos(EntBoletoCancelar Tickets)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(List<Guid> Tickets)", Tickets));

            try
            {
                string ruta = "cancelar";

                var sesion = await _auth.BIniciarSesion();
                if (sesion.HasError != true)
                {
                    string tokenKong = sesion.Result.sToken;

                    var httpResponse = await _servGenerico.SPostBody(_urlHttpClientBoletoVirtual, ruta, Tickets, tokenKong);

                    if (httpResponse.HasError != true)
                    {
                        if (httpResponse.Result != null)
                        {
                            response.SetSuccess((bool)httpResponse.Result);
                        }
                        else
                        {
                            response.SetSuccess(false);
                        }
                    }
                    else
                    {
                        response.ErrorCode = httpResponse.ErrorCode;
                        response.SetError(httpResponse.Message);
                    }

                }
                else
                {
                    response.ErrorCode = sesion.ErrorCode;
                    response.SetError(sesion.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(List<Guid> Tickets): {ex.Message}", Tickets, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465926321, 67823465927098)]
        public async Task<IMDResponse<bool>> BValidaMonedero(EntInfoMonedero entInfoMonedero)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                var operacionesMonedero = await _busMonedero.BEstatusMonedero(entInfoMonedero);
                string mensajeMonedero = "El monedero ";
                if (operacionesMonedero.HasError == true)
                {
                    response.ErrorCode = operacionesMonedero.ErrorCode;
                    if (operacionesMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                    {
                        mensajeMonedero = mensajeMonedero + operacionesMonedero.Message;
                    }
                    else
                    {
                        mensajeMonedero = operacionesMonedero.Message;
                    }
                    response.SetError(mensajeMonedero);
                    return response;
                }

                var operacionesValidas = operacionesMonedero.Result;

                //Determina si puede generar QR el monedero
                if (operacionesValidas.sTodasOperaciones == OperacionesMonedero.TodasOperaciones.GetDescription())
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetSuccess(true, mensajeMonedero + operacionesMonedero.Message);
                    response.ErrorCode = operacionesMonedero.ErrorCode;
                    return response;

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entInfoMonedero, ex, response));
            }

            //Termina validación del estatus del monedero
            return response;
        }

        public async Task<IMDResponse<bool>> BValidaTarjeta(string sNumeroTarjeta)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();


            var iniciaSesionTokenKong = await _auth.BIniciarSesion();
            if (iniciaSesionTokenKong.HasError != true)
            {
                string tokenKong = iniciaSesionTokenKong.Result.sToken;
                //Valida tarjeta
                if (!string.IsNullOrEmpty(sNumeroTarjeta))
                {
                    var estatusTarjeta = await _busTarjetaUsuario.BValidaEstatusTarjeta(sNumeroTarjeta, tokenKong);
                    string message = "La tarjeta ";
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
                    else
                    {
                        //Se cargan las operaciones
                        var operacionesPermitidasTarjeta = estatusTarjeta.Result;

                        //Determina si puede generar QR la tarjeta
                        if (operacionesPermitidasTarjeta.sTodasOperaciones == OperacionesTarjeta.TodasOperaciones.GetDescription())
                        {
                            response.SetSuccess(true);

                        }
                        else
                        {
                            response.ErrorCode = estatusTarjeta.ErrorCode;
                            response.SetError(message + estatusTarjeta.Message);
                            return response;
                        }
                    }
                }
                else
                {
                    response.SetError(Menssages.BusNoValidCards);
                    return response;
                }
                //Termina valida tarjeta
            }
            else
            {
                response.ErrorCode = iniciaSesionTokenKong.ErrorCode;
                response.SetError(iniciaSesionTokenKong.Message);
            }

            //Termina validación del estatus del monedero
            return response;
        }

        [IMDMetodo(67823466146989, 67823466147766)]
        public async Task<IMDResponse<int>> BGetCountBoletos(EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual)
        {
            IMDResponse<int> response = new IMDResponse<int>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", reqListaBoletoVirtual));

            try
            {
                #region Consulta BD Tickets
                EntConsultaTickets entReplicaTicket = new EntConsultaTickets()
                {
                    uIdMonedero = reqListaBoletoVirtual.uIdMonedero,
                    claveApp = reqListaBoletoVirtual.sClaveApp,
                    uIdSolicitud = reqListaBoletoVirtual.uIdSolicitud,
                    bUsado = reqListaBoletoVirtual.iUsado != 0
                };

                var respTickets = await _busTickets.BGetCountTickets(entReplicaTicket);
                if (respTickets.HasError)
                {
                    response.ErrorCode = respTickets.ErrorCode;
                    response.SetError(respTickets.Message);
                    return response;
                }
                #endregion

                response.SetSuccess(respTickets.Result);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", reqListaBoletoVirtual, ex, response));

            }
            return response;
        }

        #endregion
    }
}
