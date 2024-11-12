namespace AppMonederoCommand.Business;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 12/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class BusConfiguracionPago : IBusConfiguracionPago
{
    private readonly ILogger<BusConfiguracionPago> _logger;
    private readonly IServGenerico _servGenerico;
    private string URLBase;
    private string endPointGetConfig;

    public BusConfiguracionPago(ILogger<BusConfiguracionPago> logger, IServGenerico servGenerico)
    {
        URLBase = Environment.GetEnvironmentVariable("URLBASE_PAGOS") ?? string.Empty;
        endPointGetConfig = Environment.GetEnvironmentVariable("ENDPOINT_CONFIG_PAGOS") ?? string.Empty;
        _logger = logger;
        _servGenerico = servGenerico;
    }

    [IMDMetodo(67823463308608, 67823463307831)]
    public async Task<IMDResponse<dynamic>> BObtenerConfiguracion(int iOpcionPago, string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string token)", token));

        try
        {
            var apiResponse = await _servGenerico.SGetPath(URLBase, endPointGetConfig, iOpcionPago, token);

            if (apiResponse.HasError)
            {
                return response.GetResponse(apiResponse);
            }

            var entPagoConfig = JsonSerializer.Deserialize<EntPagoConfig>(apiResponse.Result.ToString()!);

            response.SetSuccess(entPagoConfig, Menssages.BusCompleteCorrect);
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
