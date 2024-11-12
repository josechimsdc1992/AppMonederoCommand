namespace AppMonederoCommand.Business;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 25/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class BusHistorialRecuperarCuenta : IBusHistorialRecuperarCuenta
{
    private readonly ILogger<BusHistorialRecuperarCuenta> _logger;
    private readonly IDatHistorialRecuperarCuenta _datHistorialRecuperarCuenta;

    public BusHistorialRecuperarCuenta(ILogger<BusHistorialRecuperarCuenta> logger, IDatHistorialRecuperarCuenta datHistorialRecuperarCuenta)
    {
        _logger = logger;
        _datHistorialRecuperarCuenta = datHistorialRecuperarCuenta;
    }

    [IMDMetodo(67823462687008, 67823462686231)]
    public async Task<IMDResponse<EntHistorialRecuperarCuenta>> BGetByCorreoAndToken(string sCorreo, string sToken)
    {
        IMDResponse<EntHistorialRecuperarCuenta> response = new IMDResponse<EntHistorialRecuperarCuenta>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sCorreo, string sToken)", sCorreo, sToken));
        try
        {
            var busResponse = await _datHistorialRecuperarCuenta.DGetByCorreoAndToken(sCorreo, sToken);

            if (!busResponse.HasError)
            {
                var historialRecuperarCuenta = busResponse.Result;

                if (DateTime.Now <= historialRecuperarCuenta.dtFechaVencimiento)
                {
                    response = busResponse;
                }
                else
                {
                    if (historialRecuperarCuenta.bActivo)
                    {
                        var deleteResponse = await _datHistorialRecuperarCuenta.DDelete(historialRecuperarCuenta.uIdHistorialRecuperarCuenta);
                    }
                    response.SetNotFound(new EntHistorialRecuperarCuenta(), Menssages.DatRegisterNoExist);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo, string sToken)", sCorreo, sToken, response));
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo, string sToken): {ex.Message}", sCorreo, sToken, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462688562, 67823462687785)]
    public async Task<IMDResponse<EntHistorialRecuperarCuenta>> BSave(string sCorreo)
    {
        IMDResponse<EntHistorialRecuperarCuenta> response = new IMDResponse<EntHistorialRecuperarCuenta>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sCorreo)", sCorreo));

        try
        {
            var existResponse = await _datHistorialRecuperarCuenta.DGetByCorreo(sCorreo);

            if (existResponse.HasError)
            {
                await _datHistorialRecuperarCuenta.DDelete(existResponse.Result.uIdHistorialRecuperarCuenta);
            }

            EntHistorialRecuperarCuenta historialRecuperarCuenta = new EntHistorialRecuperarCuenta();
            historialRecuperarCuenta.uIdHistorialRecuperarCuenta = Guid.NewGuid();
            historialRecuperarCuenta.sCorreo = sCorreo;
            historialRecuperarCuenta.sToken = generateRandomString();
            historialRecuperarCuenta.bActivo = true;
            historialRecuperarCuenta.dtFechaCreacion = DateTime.Now;
            historialRecuperarCuenta.dtFechaVencimiento = historialRecuperarCuenta.dtFechaCreacion.AddMinutes(10);

            response = await _datHistorialRecuperarCuenta.DSave(historialRecuperarCuenta);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo): {ex.Message}", sCorreo, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462690116, 67823462689339)]
    public async Task<IMDResponse<bool>> BUpdate(string sCorreo, string sToken)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sCorreo, string sToken)", sCorreo, sToken));

        try
        {
            var existResponse = await _datHistorialRecuperarCuenta.DGetByCorreo(sCorreo);

            if (!existResponse.HasError)
            {
                EntHistorialRecuperarCuenta historialRecuperarCuenta = new EntHistorialRecuperarCuenta();
                historialRecuperarCuenta.sCorreo = sCorreo;
                historialRecuperarCuenta.sToken = sToken;

                response = await _datHistorialRecuperarCuenta.DUpdate(historialRecuperarCuenta);
            }
            else
            {
                response.SetNotFound(false, Menssages.DatRegisterNoExist);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo, string sToken): {ex.Message}", sCorreo, sToken, ex, response));
        }
        return response;
    }

    private string generateRandomString()
    {

        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[16];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new String(stringChars);
    }
}