namespace AppMonederoCommand.Business;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class BusUbicacionFavorita : IBusUbicacionFavorita
{
    private readonly ILogger<BusUbicacionFavorita> _logger;
    private readonly IDatUbicacionFavorita _datUbicacionFavorita;
    private readonly IDatUsuario _datUsuario;
    private readonly string _errorCodeSesion = Environment.GetEnvironmentVariable("ERROR_CODE_SESION") ?? "";

    public BusUbicacionFavorita(ILogger<BusUbicacionFavorita> logger, IDatUbicacionFavorita datUbicacionFavorita, IDatUsuario datUsuario)
    {
        _logger = logger;
        _datUbicacionFavorita = datUbicacionFavorita;
        _datUsuario = datUsuario;
    }


    [IMDMetodo(67823462464786, 67823462464009)]
    public async Task<IMDResponse<dynamic>> BGetAll(Guid uIdUsuario, string? sIdAplicacion = null)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

        try
        {
            if (sIdAplicacion != null)
            {
                var entUsuarios = await _datUsuario.DGet(uIdUsuario);

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

            var datResponse = await _datUbicacionFavorita.DGetAllByIdUsuario(uIdUsuario);

            if (!datResponse.HasError)
            {
                datResponse.Result.ForEach(item =>
                {
                    item.sDireccion = !item.sDireccion.IsNullOrEmpty() ? item.sDireccion!.ToUTF8() : string.Empty;
                });
            }

            response = new IMDResponse<dynamic>
            {
                Result = datResponse.Result,
                HttpCode = datResponse.HttpCode,
                HasError = datResponse.HasError,
                ErrorCode = datResponse.ErrorCode,
                Message = datResponse.Message
            };
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462540932, 67823462540155)]
    public async Task<IMDResponse<EntUbicacionFavorita>> BCreate(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario)
    {
        IMDResponse<EntUbicacionFavorita> response = new IMDResponse<EntUbicacionFavorita>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntUbicacionFavorita ubicacionFavorita)", ubicacionFavorita));

        try
        {
            response = await _datUbicacionFavorita.DSave(ubicacionFavorita, uIdUsuario);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntUbicacionFavorita ubicacionFavorita): {ex.Message}", ubicacionFavorita, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462550256, 67823462549479)]
    public async Task<IMDResponse<bool>> BDelete(Guid iKey, Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid iKey)", iKey));

        try
        {
            response = await _datUbicacionFavorita.DDelete(iKey, uIdUsuario);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid iKey): {ex.Message}", iKey, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462593768, 67823462592991)]
    public async Task<IMDResponse<bool>> BUpdate(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario)", ubicacionFavorita, uIdUsuario));

        try
        {
            response = await _datUbicacionFavorita.DUpdate(ubicacionFavorita, uIdUsuario);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario): {ex.Message}", ubicacionFavorita, uIdUsuario, ex, response));
        }
        return response;
    }

}
