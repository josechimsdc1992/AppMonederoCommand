using AppMonederoCommand.Business.Repositories.Monedero;
using AppMonederoCommand.Entities.Monedero.Enums;

namespace AppMonederoCommand.Data.Queries.Monedero.Folio
{
    public class DatFolio : IDatFolio
    {
        protected TransporteContext _dbContext { get; }
        private readonly ILogger<DatFolio> _logger;
        public DatFolio(TransporteContext dbContext, ILogger<DatFolio> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IMDResponse<long>> DGetFolio(OperacionesMovimientosMonedero sOperacion)
        {
            IMDResponse<long> response = new IMDResponse<long>();

            string metodo = nameof(this.DGetFolio);
            _logger.LogInformation(IMDSerializer.Serialize(67823463735181, $"Inicia {metodo}({sOperacion})"));

            try
            {
                FormattableString query = null;
                if (sOperacion == OperacionesMovimientosMonedero.Traspaso)
                {
                    query = $"SELECT secuencia_folios.NEXTVAL FROM DUAL";
                }
                else if (sOperacion == OperacionesMovimientosMonedero.VentSaldo)
                {
                    query = $"SELECT secuencia_folios_recargas.NEXTVAL FROM DUAL";
                }
                else
                {
                    response.SetError("No se obtuvo el folio.");
                    return response;
                }

                var Folio = _dbContext.Database.SqlQuery<long>(query).AsEnumerable().FirstOrDefault();

                response.SetSuccess(Folio);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463735958;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463735958, $"Error en {metodo}(): {ex.Message}", sOperacion, ex, response));
            }
            return response;
        }

    }
}
