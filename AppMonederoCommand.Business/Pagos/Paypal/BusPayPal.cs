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
public class BusPayPal : IBusPayPal
{
    private readonly ILogger<BusPayPal> _logger;
    private readonly IServGenerico _servGenerico;
    private readonly IBusOrden _busOrden;
    private string URLBase;
    private string endPointGetConfig;
    private string endPointSavePayment;
    private string idPaypalConfig;

    public BusPayPal(ILogger<BusPayPal> logger, IServGenerico servGenerico, IBusOrden busOrden)
    {
        URLBase = Environment.GetEnvironmentVariable("URLBASE_PAGOS") ?? string.Empty;
        endPointGetConfig = Environment.GetEnvironmentVariable("ENDPOINT_CONFIG_PAYPAL") ?? string.Empty;
        idPaypalConfig = Environment.GetEnvironmentVariable("PAYPAL_ID_CONFIG") ?? string.Empty;
        endPointSavePayment = Environment.GetEnvironmentVariable("ENDPOINT_SAVE_PAYMENT_PAYPAL") ?? string.Empty;
        _logger = logger;
        _servGenerico = servGenerico;
        _busOrden = busOrden;
    }

    [IMDMetodo(67823463308608, 67823463307831)]
    public async Task<IMDResponse<dynamic>> BRegistrarPago(EntPagoPayPal entPagoPayPal, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string token)", token));

        try
        {

            dynamic pago = new ExpandoObject();
            pago.IdPagoPayPal = entPagoPayPal.uIdPagoPayPal;
            pago.IdOrden = entPagoPayPal.uIdOrden;
            pago.IdReferencia = entPagoPayPal.uIdReferencia;
            pago.IdPagador = entPagoPayPal.sIdPagador;
            pago.NombreCompleto = entPagoPayPal.sNombreCompleto;
            pago.Correo = entPagoPayPal.sCorreo;
            pago.Estatus = entPagoPayPal.sEstatus;
            pago.FechaPago = entPagoPayPal.dtFechaPago;
            pago.IdPago = entPagoPayPal.sIdPago;

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

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string token): {ex.Message}", token, ex, response));
        }
        return response;
    }
}
