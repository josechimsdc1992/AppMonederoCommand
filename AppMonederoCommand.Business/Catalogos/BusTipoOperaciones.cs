namespace AppMonederoCommand.Business.Catalogos
{
    public class BusTipoOperaciones : IBusTipoOperaciones
    {
        private readonly ILogger<BusMotivos> _logger;
        private readonly IDatTipoOperaciones _datTipoOperaciones;

        public BusTipoOperaciones(ILogger<BusMotivos> logger, IAuthService auth, IServGenerico servGenerico, IDatTipoOperaciones datTipoOperaciones)
        {
            _logger = logger;
            _datTipoOperaciones = datTipoOperaciones;
        }

        [IMDMetodo(67823465994697, 67823465995474)]
        public async Task<IMDResponse<List<EntTipoOperaciones>>> BObtenerTipoOperaciones()
        {
            IMDResponse<List<EntTipoOperaciones>> response = new IMDResponse<List<EntTipoOperaciones>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));

            try
            {
                response = await _datTipoOperaciones.DObtenerTipoOperaciones();
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
