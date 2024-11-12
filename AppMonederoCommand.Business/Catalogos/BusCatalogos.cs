namespace AppMonederoCommand.Business.Catalogos
{
    public class BusCatalogos : IBusCatalogos
    {
        private readonly ILogger<BusCatalogos> _logger;
        private readonly IServGenerico _servGenerico;
        private readonly IAuthService _authService;

        private string BASE_URL_CATALOGOS;
        private string ENDPOINT_GET_GENEROS;

        public BusCatalogos(ILogger<BusCatalogos> logger, IServGenerico servGenerico, IAuthService authService)
        {
            _logger = logger;
            _servGenerico = servGenerico;
            _authService = authService;

            BASE_URL_CATALOGOS = Environment.GetEnvironmentVariable("URLBASE_PAQUETES") ?? "";
            ENDPOINT_GET_GENEROS = Environment.GetEnvironmentVariable("ENDPOINT_GET_GENEROS") ?? "";
        }

        [IMDMetodo(67823465824534, 67823465823757)]
        public async Task<IMDResponse<List<EntGenero>>> BObtenerGeneros()
        {
            IMDResponse<List<EntGenero>> response = new IMDResponse<List<EntGenero>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));

            try
            {
                var token = _authService.BIniciarSesion();
                if (token.Result.HasError)
                {
                    response.SetError(token.Result.Message);
                }

                var generosResponse = await _servGenerico.SGetPath(BASE_URL_CATALOGOS, ENDPOINT_GET_GENEROS, token.Result.Result.sToken);
                var obj = JsonSerializer.Deserialize<EntGeneroResponse>(generosResponse.Result.ToString()!);

                response.SetSuccess(obj.generos);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }

            return response;
        }

    }
}