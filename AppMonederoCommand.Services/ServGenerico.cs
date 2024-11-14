namespace AppMonederoCommand.Services;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 03/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class ServGenerico : IServGenerico
{

    private readonly ILogger<ServGenerico> _logger;
    private HttpClient httpClient;
    HttpClientHandler clientHandler = new HttpClientHandler();

    public ServGenerico(ILogger<ServGenerico> logger)
    {
        _logger = logger;
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        httpClient = new HttpClient(clientHandler);
        httpClient.Timeout = TimeSpan.FromMinutes(5);
    }

    [IMDMetodo(67823463262765, 67823463262765)]
    public async Task<IMDResponse<object>> SGetPath(string pUrlBase, string pUrlContoller, object pParam, string? token)
    {

        IMDResponse<object> response = new IMDResponse<object>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})", pUrlBase, pUrlContoller, pParam, token));

        try
        {
            var lenguaje = Environment.GetEnvironmentVariable("LENGUAJE_CONSUMO") ?? eLenguajes.EspañolLenguaje.GetDescription();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lenguaje));

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await httpClient.GetFromJsonAsync<IMDResponse<object>>($"{pUrlBase}/{pUrlContoller}/{pParam.ToString()}");

            if (httpResponse != null)
            {
                response = httpResponse;
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", pUrlBase, pUrlContoller, pParam, token, ex, response));
        }
        return response;

    }

    [IMDMetodo(67823463288406, 67823463287629)]
    public async Task<IMDResponse<object>> SGetPath(string pUrlBase, string pUrl, string? token)
    {
        IMDResponse<object> response = new IMDResponse<object>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})", pUrlBase, pUrl, token));

        try
        {
            var lenguaje = Environment.GetEnvironmentVariable("LENGUAJE_CONSUMO") ?? eLenguajes.EspañolLenguaje.GetDescription();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lenguaje));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await httpClient.GetFromJsonAsync<IMDResponse<object>>($"{pUrlBase}/{pUrl}");

            if (httpResponse != null)
            {
                response = httpResponse;
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", pUrlBase, pUrl, token, ex, response));
        }
        return response;

    }

    [IMDMetodo(67823463266650, 67823463265873)]
    public async Task<IMDResponse<object>> SPostBody(string pUrlBase, string pUrlContoller, object pParam, string? token, Dictionary<string, string>? headers = null)
    {
        IMDResponse<object> response = new IMDResponse<object>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, 
            $"Inicia {metodo.sNombre}({metodo.sParametros})", 
            pUrlBase, pUrlContoller, pParam, token, headers)
            );

        try
        {
            var lenguaje = Environment.GetEnvironmentVariable("LENGUAJE_CONSUMO") ?? eLenguajes.EspañolLenguaje.GetDescription();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lenguaje));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    try { httpClient.DefaultRequestHeaders.Add(item.Key, item.Value); }
                    catch { }
                }
            }

            string path = $"{pUrlBase}/{pUrlContoller}";
            _logger.LogInformation("POST_URL:"+path);
            var result = await httpClient.PostAsJsonAsync(path, pParam);

            var stringRe = result.Content.ReadAsStringAsync().Result;
            var httpResponse = JsonConvert.DeserializeObject<IMDResponse<object>>(stringRe);

            if (httpResponse != null)
            {
                if (httpResponse.HasError)
                {
                    response.SetError(httpResponse.Message);
                    response.ErrorCode = httpResponse.ErrorCode;
                }
                else
                {
                    response.SetSuccess(httpResponse.Result, Menssages.BusCompleteCorrect);
                }
            }

        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", pUrlBase, pUrlContoller, pParam, token, headers, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463268204, 67823463267427)]
    public async Task<IMDResponse<object>> SPutBody(string pUrlBase, string pUrlContoller, object pParam, string? token)
    {
        IMDResponse<object> response = new IMDResponse<object>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})", pUrlBase, pUrlContoller, pParam, token));

        try
        {
            var lenguaje = Environment.GetEnvironmentVariable("LENGUAJE_CONSUMO") ?? eLenguajes.EspañolLenguaje.GetDescription();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lenguaje));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.PutAsJsonAsync($"{pUrlBase}/{pUrlContoller}", pParam);

            var stringRe = result.Content.ReadAsStringAsync().Result;
            var httpResponse = JsonConvert.DeserializeObject<IMDResponse<object>>(stringRe);

            if (httpResponse != null)
            {
                if (httpResponse.HasError)
                {
                    response.SetError(httpResponse.Message);
                    response.ErrorCode = httpResponse.ErrorCode;
                }
                else
                {
                    response = httpResponse;
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", pUrlBase, pUrlContoller, pParam, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463269758, 67823463268981)]
    public async Task<IMDResponse<object>> SDeletePath(string pUrlBase, string pUrlContoller, object pParam, string? token)
    {
        IMDResponse<object> response = new IMDResponse<object>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})", pUrlBase, pUrlContoller, pParam, token));

        try
        {
            var lenguaje = Environment.GetEnvironmentVariable("LENGUAJE_CONSUMO") ?? eLenguajes.EspañolLenguaje.GetDescription();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lenguaje));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.DeleteFromJsonAsync<IMDResponse<object>>($"{pUrlBase}/{pUrlContoller}/{pParam}");
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", pUrlBase, pUrlContoller, pParam, token, ex, response));
        }
        return response;
    }

    public async Task<IMDResponse<object>> SPutBodyModeroC(string pUrlBase, string pUrlContoller, object pParam, string? token)
    {
        IMDResponse<object> response = new IMDResponse<object>();

        string metodo = nameof(this.SPutBodyModeroC);
        _logger.LogInformation(IMDSerializer.Serialize(67823463685453, $"Inicia {metodo}(string pUrlBase, string pUrlContoller, object pParam, string? token)", pUrlBase, pUrlContoller, pParam, token));

        try
        {
            var lenguaje = Environment.GetEnvironmentVariable("LENGUAJE_CONSUMO") ?? eLenguajes.EspañolLenguaje.GetDescription();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lenguaje));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var json = JsonConvert.SerializeObject(pParam);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await httpClient.PutAsync($"{pUrlBase}/{pUrlContoller}", content);

            var stringRe = result.Content.ReadAsStringAsync().Result;
            var httpResponse = JsonConvert.DeserializeObject<IMDResponse<object>>(stringRe);

            if (httpResponse != null)
            {
                if (httpResponse.HasError)
                {
                    response.SetError(httpResponse.Message);
                    response.ErrorCode = httpResponse.ErrorCode;
                }
                else
                {
                    response = httpResponse;
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823463686230;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823463686230, $"Error en {metodo}(string pUrlBase, string pUrlContoller, object pParam, string? token): {ex.Message}", pUrlBase, pUrlContoller, pParam, token, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823464881256, 67823464880479)]
    public async Task<IMDResponse<object>> SGetBody(string pUrlBase, string pUrlContoller, object pParam, string? token)
    {
        IMDResponse<object> response = new IMDResponse<object>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string pUrlBase, string pUrlContoller, object pParam, string? token)", pUrlBase, pUrlContoller, pParam, token));

        try
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, pUrlBase + "/" + pUrlContoller);

            var lenguaje = Environment.GetEnvironmentVariable("LENGUAJE_CONSUMO") ?? eLenguajes.EspañolLenguaje.GetDescription();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(lenguaje));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(pParam), Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage);

            using (HttpContent httpContent = httpResponse.Content)
            {
                var jsonString = await httpContent.ReadAsStringAsync();
                var _respose = JsonConvert.DeserializeObject<IMDResponse<object>>(jsonString);

                if (_respose != null)
                {
                    if (_respose.HasError)
                    {
                        response.SetError(_respose.Message);
                        response.ErrorCode = _respose.ErrorCode;
                    }
                    else
                    {
                        response = _respose;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823463686230;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string pUrlBase, string pUrlContoller, object pParam, string? token): {ex.Message}", pUrlBase, pUrlContoller, pParam, token, ex, response));
        }
        return response;
    }
}
