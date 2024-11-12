namespace AppMonederoCommand.Business.BusUsuarios
{
    public class BusUsuariosWeb : IBusUsuariosWeb
    {
        private readonly IBusTarjetaUsuario _busTarjetaUsuario;
        private readonly IBusMonedero _busMonedero;
        private readonly IDatUsuario _datUsuario;
        private readonly ILogger<BusUsuariosWeb> _logger;

        public BusUsuariosWeb(IBusTarjetaUsuario busTarjetaUsuario, IBusMonedero busMonedero, ILogger<BusUsuariosWeb> logger, IDatUsuario datUsuario)
        {
            _busTarjetaUsuario = busTarjetaUsuario;
            _busMonedero = busMonedero;
            _logger = logger;
            _datUsuario = datUsuario;
        }


        [IMDMetodo(67823465574340, 67823465573563)]
        public async Task<IMDResponse<List<EntTarjetaUsuario>>> BGetListaTarjetas(Guid uIdUsuario, Guid uIdMonedero, string sToken)
        {
            IMDResponse<List<EntTarjetaUsuario>> response = new IMDResponse<List<EntTarjetaUsuario>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario, Guid uIdMonedero, string sToken)", uIdUsuario, uIdMonedero, sToken));

            try
            {
                List<EntTarjetaUsuario> listaTarjetasUsuario = new List<EntTarjetaUsuario>();

                var monedero = await _busMonedero.BDatosMonedero(uIdMonedero);

                if (!monedero.HasError)
                {
                    EntTarjetaUsuario tarjeta = new EntTarjetaUsuario
                    {
                        uIdTarjeta = monedero.Result.idMonedero,
                        uIdMonedero = monedero.Result.idMonedero,
                        dSaldo = monedero.Result.saldo,
                        sNumeroTarjeta = monedero.Result.numMonedero.ToString(),
                        iNoMonedero = monedero.Result.numMonedero,
                        sTipoTarifa = monedero.Result.tarifa,
                        uIdTipoTarifa = monedero.Result.idTipoTarifa,
                        sFechaVigencia = monedero.Result.fechaVigencia!,
                        bActivo = monedero.Result.activo,
                        sMotivoBloqueo = "TARJETA INTELIGENTE",
                        bMonederoVirtual = true
                    };

                    listaTarjetasUsuario.Add(tarjeta);
                }

                var listaTarjetas = await _busTarjetaUsuario.BTarjetas(uIdUsuario, uIdMonedero, sToken);

                if (!listaTarjetas.HasError)
                {
                    listaTarjetas.Result.ForEach(x =>
                    {
                        x.sMotivoBloqueo = "TARJETA";
                        listaTarjetasUsuario.Add(x);
                    });
                }

                response.SetSuccess(listaTarjetasUsuario);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(67823465574340, $"Error en {metodo}(Guid uIdUsuario, Guid uIdMonedero, string sToken): {ex.Message}", uIdUsuario, uIdMonedero, sToken, ex, response));
            }
            return response;
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
