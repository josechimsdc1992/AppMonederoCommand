namespace AppMonederoCommand.Business.Webhooks
{
    public class BusWebhooks : IBusWebhooks
    {
        private readonly ILogger<BusWebhooks> _logger;
        private readonly IServGenerico _servGenerico;
        private readonly IAuthService _authService;
        private readonly IBusOrden _busOrden;

        private string URLBaseCommand;
        private List<Enum> lPEstatus;
        private List<Enum> lPCodeEstatus;
        public BusWebhooks(ILogger<BusWebhooks> logger, IServGenerico servGenerico, IAuthService authService, IBusOrden busOrden)
        {
            _logger = logger;
            _servGenerico = servGenerico;
            _authService = authService;
            _busOrden = busOrden;

            URLBaseCommand = Environment.GetEnvironmentVariable("URLBASE_PAGOS") ?? string.Empty;

            lPEstatus = new()
            {
                eEventosPaypal.EventoAutorizado,
                eEventosPaypal.EventoCreado,
                eEventosPaypal.EventoCompletado,
                eEventosPaypal.EventoDeclinado,
                eEventosPaypal.EventoCheckCreado,
                eEventosPaypal.EventoCheckAprovado,
                eEventosPaypal.EventoCheckCobrado
            };

            lPCodeEstatus = new()
            {
                eCodigosEventosPaypal.EventoAutorizado,
                eCodigosEventosPaypal.EventoCreado,
                eCodigosEventosPaypal.EventoCompletado,
                eCodigosEventosPaypal.EventoDeclinado,
                eCodigosEventosPaypal.EventoCheckCreado,
                eCodigosEventosPaypal.EventoCheckAprovado,
                eCodigosEventosPaypal.EventoCheckCobrado
            };
        }

        [IMDMetodo(67823465024224, 67823465023447)]
        public async Task<IMDResponse<bool>> BNotificarMercadoPago(Dictionary<string, string> headers, object body)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}(Dictionary<string, string> headers, object body)", headers, body));
            try
            {
                var token = _authService.BIniciarSesion().Result;

                if (token.Result != null)
                {

                    var responseServ = await _servGenerico.SPostBody(URLBaseCommand, "webhooks/Mercado", body, token.Result.sToken, headers);
                    if (!responseServ.HasError)
                    {
                        response.SetSuccess(true);

                        EntOrden entOrden = JsonSerializer.Deserialize<EntOrden>(responseServ.Result.ToString());
                        _logger.LogInformation(IMDSerializer.Serialize("Pre a validacion mercado: ", entOrden));
                        if (entOrden.Activo)
                        {
                            _logger.LogInformation(IMDSerializer.Serialize("Entra a validacion mercado: ", entOrden));
                            response = await _busOrden.BActualizarForWebhooks(entOrden, token.Result.sToken);
                        }

                    }
                    else
                    {
                        response.SetError(responseServ.Message);
                    }
                }
                else
                {
                    response.SetError(token.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}(Dictionary<string, string> headers, object body): {ex.Message}", headers, body, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465025778, 67823465025001)]
        public async Task<IMDResponse<bool>> BNotificarPayPal(Dictionary<string, string> headers, object body)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}(Dictionary<string, string> headers, object body)", headers, body));

            try
            {
                var token = _authService.BIniciarSesion().Result;

                if (token.Result != null)
                {
                    var responseServ = await _servGenerico.SPostBody(URLBaseCommand, "webhooks/PayPal", body, token.Result.sToken, headers);
                    if (!responseServ.HasError)
                    {
                        string me = responseServ.Message;
                        response.SetSuccess(true);

                        EntOrden entOrden = JsonSerializer.Deserialize<EntOrden>(responseServ.Result.ToString());
                        _logger.LogInformation(IMDSerializer.Serialize("Pre a validacion Paypal: " + me, entOrden));

                        if (entOrden.Activo)
                        {
                            _logger.LogInformation(IMDSerializer.Serialize("Entra a validacion Paypal: " + me, entOrden));

                            response = await _busOrden.BActualizarForWebhooks(entOrden, token.Result.sToken);
                        }
                    }
                    else
                    {
                        response.SetError(responseServ.Message);
                    }
                }
                else
                {
                    response.SetError(token.Message);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}(Dictionary<string, string> headers, object body): {ex.Message}", headers, body, ex, response));
            }
            return response;
        }

    }
}

