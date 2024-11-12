namespace AppMonederoCommand.Business.Tarifa
{
    public class BusTipoTarifa : IBusTipoTarifa
    {
        private readonly ILogger<BusTipoTarifa> _logger;
        private readonly IDatTipoTarifa _datTipoTarifa;

        public BusTipoTarifa(ILogger<BusTipoTarifa> logger, IDatTipoTarifa datTipoTarifa)
        {
            _logger = logger;
            _datTipoTarifa = datTipoTarifa;
        }

        [IMDMetodo(67823465943415, 67823465944192)]
        public async Task<IMDResponse<EntReplicaTipoTarifas>> BObtenerTipoTarifa(Guid uIdTipoTarifa)
        {
            IMDResponse<EntReplicaTipoTarifas> response = new IMDResponse<EntReplicaTipoTarifas>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                response = await _datTipoTarifa.DObtenerTipoTarifa(uIdTipoTarifa);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdTipoTarifa, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465993143, 67823465993920)]
        public async Task<IMDResponse<List<EntReplicaTipoTarifas>>> BGetAll()
        {
            IMDResponse<List<EntReplicaTipoTarifas>> response = new IMDResponse<List<EntReplicaTipoTarifas>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                response = await _datTipoTarifa.DGetAll();
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", ex, response));
            }
            return response;
        }

    }
}
