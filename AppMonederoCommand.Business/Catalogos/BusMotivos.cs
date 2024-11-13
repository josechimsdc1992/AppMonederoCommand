namespace AppMonederoCommand.Business.Catalogos
{
    public class BusMotivos : IBusMotivos
    {
        private readonly ILogger<BusMotivos> _logger;
        private readonly IDatMotivos _datMotivos;

        public BusMotivos(ILogger<BusMotivos> logger, IDatMotivos datMotivos)
        {
            _logger = logger;
            _datMotivos = datMotivos;
        }

        [IMDMetodo(67823465922436, 67823465921659)]
        public async Task<IMDResponse<bool>> BAgregar(EntMotivo entMotivo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntMotivo entMotivo)", entMotivo));
            try
            {
                entMotivo.bPermitirEditar = entMotivo.bPermitirEditar ?? false;
                entMotivo.bPermitirOperaciones = entMotivo.bPermitirOperaciones ?? false;
                entMotivo.bPermitirReactivar = entMotivo.bPermitirReactivar ?? false;

                response = await _datMotivos.DAgregar(entMotivo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntMotivo entMotivo): {ex.Message}", entMotivo, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465923990, 67823465923213)]
        public async Task<IMDResponse<bool>> BActualizar(EntMotivo entMotivo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntMotivo entMotivo)", entMotivo));

            try
            {
                entMotivo.bPermitirEditar = entMotivo.bPermitirEditar ?? false;
                entMotivo.bPermitirOperaciones = entMotivo.bPermitirOperaciones ?? false;
                entMotivo.bPermitirReactivar = entMotivo.bPermitirReactivar ?? false;

                response = await _datMotivos.DActualizar(entMotivo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntMotivo entMotivo): {ex.Message}", entMotivo, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465925544, 67823465924767)]
        public async Task<IMDResponse<bool>> BEliminar(Guid uIdMotivo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMotivo)", uIdMotivo));

            try
            {
                response = await _datMotivos.DEliminar(uIdMotivo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMotivo): {ex.Message}", uIdMotivo, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465969833, 67823465970610)]
        public async Task<IMDResponse<EntMotivo>> BObtenerMotivo(Guid uIdMotivo)
        {
            IMDResponse<EntMotivo> response = new IMDResponse<EntMotivo>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                response = await _datMotivos.DObtenerMotivo(uIdMotivo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdMotivo, ex, response));
            }
            return response;
        }
        [IMDMetodo(67823465969833, 67823465970610)]
        public async Task<IMDResponse<List<EntMotivo>>> BObtenerTodos()
        {
            IMDResponse<List<EntMotivo>> response = new IMDResponse<List<EntMotivo>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                response = await _datMotivos.DObtenerTodos();
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
