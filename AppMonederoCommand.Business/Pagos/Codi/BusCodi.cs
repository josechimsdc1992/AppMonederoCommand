namespace AppMonederoCommand.Business.Pagos.Codi;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 11/01/2024 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class BusCodi : IBusCodi
{
    private readonly ILogger<BusBanorte> _logger;
    private readonly IServGenerico _servGenerico;
    private readonly IBusOrden _busOrden;
    private string URLBase;
    private string endPointSavePayment;

    public BusCodi(ILogger<BusBanorte> logger, IServGenerico servGenerico, IBusOrden busOrden)
    {
        URLBase = Environment.GetEnvironmentVariable("URLBASE_PAGOS") ?? string.Empty;
        endPointSavePayment = Environment.GetEnvironmentVariable("ENDPOINT_SAVE_PAYMENT_CODI") ?? string.Empty;
        _logger = logger;
        _servGenerico = servGenerico;
        _busOrden = busOrden;
    }


    [IMDMetodo(67823464422826, 67823464422049)]
    public async Task<IMDResponse<dynamic?>> BRegistrarPago(EntPagoCodi entPagoCodi, string token)
    {
        IMDResponse<dynamic?> response = new IMDResponse<dynamic?>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntPagoCodi entPagoCodi, string token)", entPagoCodi, token));

        try
        {
            var resOrdenUsuario = await _busOrden.BObtenerByReferencia(entPagoCodi.sReference!, token);

            if (resOrdenUsuario.HasError)
            {
                return response.GetResponse(resOrdenUsuario);
            }

            //Llenado de datos para el request
            dynamic pago = new ExpandoObject();
            pago.IdOrden = resOrdenUsuario.Result.IdOrden;
            pago.reference = entPagoCodi.sReference;
            pago.identificador = Convert.ToDecimal(entPagoCodi.sIdentificador);
            pago.bankID = Convert.ToDecimal(entPagoCodi.sBankID);
            pago.date = entPagoCodi.dtDate;
            pago.cadena = entPagoCodi.sCadena;
            pago.user = entPagoCodi.sUser;
            pago.password = entPagoCodi.sPassword;
            pago.amount = Convert.ToDecimal(entPagoCodi.dAmount);
            pago.PaymentType = entPagoCodi.sPaymentType;
            pago.clientInformation = entPagoCodi.sClientInformation;
            pago.branch = Convert.ToDecimal(entPagoCodi.sBranch);
            pago.EstatusCoDi = entPagoCodi.sEstatusCoDi;
            pago.DescripcionCoDi = entPagoCodi.sDescripcionCoDi;
            pago.FolioCoDi = entPagoCodi.sFolioCoDi;

            var apiResponse = await _servGenerico.SPostBody(URLBase, endPointSavePayment, pago, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            if (entPagoCodi.sEstatusCoDi!.ToLower() == "exitoso")
            {
                EntActualizarOrden entActualizarOrden = new EntActualizarOrden
                {
                    uIdOrden = resOrdenUsuario.Result!.IdOrden,
                    uIdUsuario = resOrdenUsuario.Result.IdUsuario,
                    sReferencia = resOrdenUsuario.Result.OrdenRef,
                };

                var resActualizaOrden = await _busOrden.BActualizar(entActualizarOrden, token);

                if (resActualizaOrden.HasError)
                {
                    return response.GetResponse(resActualizaOrden);
                }

                response.SetSuccess(resOrdenUsuario.Result.IdOrden, Menssages.BusCompleteCorrect);
            }
            else
            {
                response.SetError("Pago no exitoso");
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntPagoCodi entPagoCodi, string token): {ex.Message}", entPagoCodi, token, ex, response));
        }
        return response;
    }

}
