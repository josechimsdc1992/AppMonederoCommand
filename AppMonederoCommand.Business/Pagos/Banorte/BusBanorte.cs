namespace AppMonederoCommand.Business;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 08/01/2024 | Neftalí Rodríguez	  | Creación
* ---------------------------------------------------------------------------------------
*/

public class BusBanorte : IBusBanorte
{
    private readonly ILogger<BusBanorte> _logger;
    private readonly IServGenerico _servGenerico;
    private readonly IBusOrden _busOrden;
    private string URLBase;
    private string endPointSavePayment;

    public BusBanorte(ILogger<BusBanorte> logger, IServGenerico servGenerico, IBusOrden busOrden)
    {
        URLBase = Environment.GetEnvironmentVariable("URLBASE_PAGOS") ?? string.Empty;
        endPointSavePayment = Environment.GetEnvironmentVariable("ENDPOINT_SAVE_PAYMENT_BANORTE") ?? string.Empty;
        _logger = logger;
        _servGenerico = servGenerico;
        _busOrden = busOrden;
    }

    [IMDMetodo(67823464368436, 67823464367659)]
    public async Task<IMDResponse<dynamic>> BRegistrarPago(EntPagoBanorte entPagoBanorte, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntPagoBanorte entPagoBanorte, string token)", entPagoBanorte, token));
        try
        {
            dynamic pago = new ExpandoObject();
            pago.IdOrden = entPagoBanorte.uIdOrden;
            pago.ChypherData = entPagoBanorte.sChypherData;

            var apiResponse = await _servGenerico.SPostBody(URLBase, endPointSavePayment, pago, token);
            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            var resOrdenUsuario = await _busOrden.BObtener(entPagoBanorte.uIdOrden, token);

            if (resOrdenUsuario.HasError)
            {
                return response.GetResponse(resOrdenUsuario);
            }

            EntActualizarOrden entActualizarOrden = new EntActualizarOrden
            {
                uIdOrden = resOrdenUsuario.Result.IdOrden,
                uIdUsuario = resOrdenUsuario.Result.IdUsuario,
                sReferencia = resOrdenUsuario.Result.OrdenRef,
            };

            var resActualizaOrden = await _busOrden.BActualizar(entActualizarOrden, token);

            if (resActualizaOrden.HasError)
            {
                return response.GetResponse(resActualizaOrden);
            }

            response.SetSuccess(apiResponse.Result, Menssages.BusCompleteCorrect);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntPagoBanorte entPagoBanorte, string token): {ex.Message}", entPagoBanorte, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465543260, 67823465542483)]
    public async Task<IMDResponse<bool>> BRegistrarPagoCancelado(EntPagoCanceladoBanorte entPagoCanceladoBanorte, string token)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntPagoCanceladoBanorte entPagoCanceladoBanorte)", entPagoCanceladoBanorte));

        try
        {
            dynamic pago = new ExpandoObject();
            pago.IdOrden = entPagoCanceladoBanorte.uIdOrden;
            pago.status3D = entPagoCanceladoBanorte.sEstatus3D;
            pago.eci = entPagoCanceladoBanorte.sECI;
            pago.id = entPagoCanceladoBanorte.sId;
            pago.message = entPagoCanceladoBanorte.sTextoAdicional;
            pago.numeroControl = entPagoCanceladoBanorte.sNumeroControl;
            pago.resultadoPayw = entPagoCanceladoBanorte.sResultadoPayworks;

            string endpoint = "Pagos/banorte/cancelado";
            var apiResponse = await _servGenerico.SPostBody(URLBase, endpoint, pago, token);
            if (apiResponse.HasError)
            {
                response.SetError(apiResponse.Message);
            }
            else
            {
                response.SetSuccess(true);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntPagoCanceladoBanorte entPagoCanceladoBanorte): {ex.Message}", entPagoCanceladoBanorte, ex, response));
        }
        return response;
    }
}

