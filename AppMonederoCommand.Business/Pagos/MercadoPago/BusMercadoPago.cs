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
*/
public class BusMercadoPago : IBusMercadoPago
{
    private readonly ILogger<BusMercadoPago> _logger;
    private readonly IServGenerico _servGenerico;
    private readonly IBusOrden _busOrden;
    private string URLBase;
    private string endPointSavePayment;

    public BusMercadoPago(ILogger<BusMercadoPago> logger, IServGenerico servGenerico, IBusOrden busOrden)
    {
        URLBase = Environment.GetEnvironmentVariable("URLBASE_PAGOS") ?? string.Empty;
        endPointSavePayment = Environment.GetEnvironmentVariable("ENDPOINT_SAVE_PAYMENT_MERCADOPAGO") ?? string.Empty;
        _logger = logger;
        _servGenerico = servGenerico;
        _busOrden = busOrden;
    }

    [IMDMetodo(67823463316378, 67823463315601)]
    public async Task<IMDResponse<dynamic>> BRegistrarPago(EntPagoMercadoPago entPagoMercadoPago, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntPagoMercadoPago entPagoMercadoPago, string token)", entPagoMercadoPago, token));

        try
        {
            dynamic pago = new ExpandoObject();
            pago.IdPagoMercadoPago = entPagoMercadoPago.uIdPagoMercadoPago;
            pago.IdOrden = entPagoMercadoPago.uIdOrden;
            pago.IdPago = entPagoMercadoPago.sIdPago;
            pago.Estatus = entPagoMercadoPago.sEstatus;
            pago.ReferenciaExterna = entPagoMercadoPago.sReferenciaExterna;
            pago.TipoPago = entPagoMercadoPago.sTipoPago;
            pago.IdOrdenComercial = entPagoMercadoPago.sIdOrdenComercial;
            pago.IdSitio = entPagoMercadoPago.sIdSitio;
            pago.ModoProcesamiento = entPagoMercadoPago.sModoProcesamiento;
            pago.IdCuentaComercial = entPagoMercadoPago.sIdCuentaComercial;

            var apiResponse = await _servGenerico.SPostBody(URLBase, endPointSavePayment, pago, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            response.SetSuccess(apiResponse.Result, Menssages.BusCompleteCorrect);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntPagoMercadoPago entPagoMercadoPago, string token): {ex.Message}", entPagoMercadoPago, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823466117463, 67823466118240)]
    public async Task<IMDResponse<bool>> BRegistrarPagoMercadoPagoCancelado(EntPagoMercadoPagoCancelado entPagoMercadoPagoCancelado, string token)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entPagoMercadoPagoCancelado, token));
        try
        {
            dynamic pago  = new ExpandoObject();
            pago.IdOrden = entPagoMercadoPagoCancelado.uIdOrden;
            pago.IdPago = entPagoMercadoPagoCancelado.sIdPago;
            pago.Estatus = entPagoMercadoPagoCancelado.sEstatus;
            pago.ReferenciaExterna = entPagoMercadoPagoCancelado.sReferenciaExterna;
            pago.TipoPago = entPagoMercadoPagoCancelado.sTipoPago;
            pago.IdOrdenComercial = entPagoMercadoPagoCancelado.sIdOrdenComercial;
            pago.IdSitio = entPagoMercadoPagoCancelado.sIdSitio;
            pago.ModoProcesamiento = entPagoMercadoPagoCancelado.sModoProcesamiento;
            pago.IdCuentaComercial = entPagoMercadoPagoCancelado.sIdCuentaComercial;

            string endpoint = "Pagos/mercadopago/cancelado";
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

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entPagoMercadoPagoCancelado, token, ex, response));
        }
        return response;
    }
}