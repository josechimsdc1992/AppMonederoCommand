namespace AppMonederoCommand.Business;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 21/08/2023 | Ing. César Cárdenas    | Creación
* ---------------------------------------------------------------------------------------
*/
public class BusNotificaciones : IBusNotificaciones
{
    private readonly ILogger<BusNotificaciones> _logger;
    private readonly ExchangeConfig _exchangeConfig;
    private readonly string _url = Environment.GetEnvironmentVariable("NOTIFICACION_URL") ?? "";
    private readonly IDatUsuario _datUsuario;
    private readonly string _errorCodeSesion = Environment.GetEnvironmentVariable("ERROR_CODE_SESION") ?? "";

    public BusNotificaciones(ILogger<BusNotificaciones> logger, ExchangeConfig exchangeConfig, IDatUsuario datUsuario)
    {
        _logger = logger;
        _exchangeConfig = exchangeConfig;
        _datUsuario = datUsuario;
    }

    [IMDMetodo(67823462345128, 67823462344351)]
    public async Task<IMDResponse<List<EntNotificaciones>>> BGetNotificaciones(string sToken, Guid uIdUsuario, string? sIdAplicacion = null)
    {
        IMDResponse<List<EntNotificaciones>> response = new IMDResponse<List<EntNotificaciones>>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sToken)", sToken));

        var responseJson = string.Empty;
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
                        response.Result = new List<EntNotificaciones>();
                        response.HttpCode = HttpStatusCode.PreconditionFailed;
                        response.ErrorCode = int.Parse(_errorCodeSesion);
                        return response;
                    }
                    if (entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.REPORTADO)
                    {
                        response.SetError(Menssages.BusBlockedAccountApp);
                        response.Result = new List<EntNotificaciones>();
                        response.HttpCode = HttpStatusCode.PreconditionFailed;
                        response.ErrorCode = int.Parse(_errorCodeSesion);
                        return response;
                    }
                }
            }

            string url = _url + $"{uIdUsuario}";

            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    responseJson = await result.Content.ReadAsStringAsync();
                    EntNotificacionesRes responseObject = JsonSerializer.Deserialize<EntNotificacionesRes>(responseJson)!;

                    var data = responseObject.Result.Select(ent => new EntNotificaciones()
                    {
                        uId = ent.IdHistorial,
                        sTitulo = ent.Titulo,
                        sMensaje = ent.Contenido,
                        dtFechaNotificacion = ent.Fecha,
                        bLeido = ent.Leido
                    }).ToList();

                    response.SetSuccess(data);
                }
                else
                {
                    response.SetError(Menssages.BusNoRespon);
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sToken): {ex.Message}", sToken, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462422828, 67823462422051)]
    public async Task<IMDResponse<bool>> BBorrarNotificacion(Guid uIdNotificacion)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdNotificacion)", uIdNotificacion));

        var responseJson = string.Empty;
        EntNotificacionesBool ent = new EntNotificacionesBool();
        try
        {
            string url = _url + uIdNotificacion.ToString() + "/eliminar";

            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.DeleteAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    responseJson = await result.Content.ReadAsStringAsync();
                    EntNotificacionesBool responseObject = JsonSerializer.Deserialize<EntNotificacionesBool>(responseJson)!;
                    response.SetSuccess(responseObject.Result, responseObject.Message!);
                }
                else
                {
                    response.SetError(Menssages.BusNoRespon);
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdNotificacion): {ex.Message}", uIdNotificacion, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462424382, 67823462423605)]
    public async Task<IMDResponse<bool>> BNotificacionLeida(Guid uIdNotificacion)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdNotificacion)", uIdNotificacion));

        var responseJson = string.Empty;
        EntNotificacionesBool ent = new EntNotificacionesBool();
        try
        {
            string url = _url + uIdNotificacion.ToString() + "/leer";

            using (var client = new HttpClient())
            {
                HttpContent content = new StringContent("");
                HttpResponseMessage result = await client.PutAsync(url, content);
                if (result.IsSuccessStatusCode)
                {
                    responseJson = await result.Content.ReadAsStringAsync();
                    EntNotificacionesBool responseObject = JsonSerializer.Deserialize<EntNotificacionesBool>(responseJson)!;
                    response.SetSuccess(responseObject.Result, responseObject.Message!);
                }
                else
                {
                    response.SetError(Menssages.BusNoRespon);
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdNotificacion): {ex.Message}", uIdNotificacion, ex, response));
        }
        return response;
    }

}
