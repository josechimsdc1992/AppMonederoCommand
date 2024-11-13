namespace AppMonederoCommand.Business.BusUsuarios
{
    public class BusUsuariosWeb : IBusUsuariosWeb
    {
        private readonly IBusMonedero _busMonedero;
        private readonly IDatUsuario _datUsuario;
        private readonly ILogger<BusUsuariosWeb> _logger;

        public BusUsuariosWeb( IBusMonedero busMonedero, ILogger<BusUsuariosWeb> logger, IDatUsuario datUsuario)
        {
            _busMonedero = busMonedero;
            _logger = logger;
            _datUsuario = datUsuario;
        }

        [IMDMetodo(67823465638054, 67823465637277)]
        public async Task<IMDResponse<bool>> BUpdateUsuarioByMonedero(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero)", entUpdateUsuarioByMonedero));

            try
            {
                if (entUpdateUsuarioByMonedero.uIdMonedero == null)
                {
                    response.SetError("El id de monedero no puede ser nulo");
                    return response;
                }

                response = await _datUsuario.DUpdateUsuarioByMonedero(entUpdateUsuarioByMonedero);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero): {ex.Message}", entUpdateUsuarioByMonedero, ex, response));
            }
            return response;
        }

    }
}
