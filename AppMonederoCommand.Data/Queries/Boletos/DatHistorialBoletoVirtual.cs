using BusMapperBoletoVirtual = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Data.Entities.Boletos.HistorialBoletoVirtual, AppMonederoCommand.Entities.Boletos.EntBoletos.EntHistorialBoletosVirtuales>;
using DbMapperBoletoVirtual = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Boletos.EntBoletos.EntHistorialBoletosVirtuales, AppMonederoCommand.Data.Entities.Boletos.HistorialBoletoVirtual>;

namespace AppMonederoCommand.Data.Queries.Boletos
{

    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 13/09/2023 | L.I. Oscar Luna       | Creación
    * ---------------------------------------------------------------------------------------
    */
    public class DatHistorialBoletoVirtual : IDatHistorialBoletoVirtual
    {
        protected TransporteContext _dbContext { get; }
        private readonly ILogger<DatHistorialBoletoVirtual> _logger;

        public DatHistorialBoletoVirtual(TransporteContext dbContext, ILogger<DatHistorialBoletoVirtual> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #region Métodos Service Default
        #endregion
        [IMDMetodo(67823463078616, 67823463077839)]
        public async Task<IMDResponse<List<EntHistorialBoletosVirtuales>>> DSave(List<EntHistorialBoletosVirtuales> rangeItems)
        {
            IMDResponse<List<EntHistorialBoletosVirtuales>> response = new IMDResponse<List<EntHistorialBoletosVirtuales>>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(List<EntHistorialBoletosVirtuales> rangeItems)", rangeItems));

            try
            {
                var tikets = DbMapperBoletoVirtual.MapList(rangeItems);
                tikets.ForEach(item => item.uIdHistorialBoletoVirtual = Guid.NewGuid());
                _dbContext.HistorialBoletoVirtual.AddRange(tikets);
                int i = await _dbContext.SaveChangesAsync();

                if (i == 0)
                {
                    response.SetError(Menssages.DatNoSave);
                }
                else
                {
                    response.SetSuccess(rangeItems);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(List<EntHistorialBoletosVirtuales> rangeItems): {ex.Message}", rangeItems, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463091048, 67823463090271)]
        public async Task<IMDResponse<List<EntHistorialBoletosVirtuales>>> DGetListaBoleto(Guid iKey)
        {
            IMDResponse<List<EntHistorialBoletosVirtuales>> response = new IMDResponse<List<EntHistorialBoletosVirtuales>>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid iKey)", iKey));

            try
            {
                var query = await _dbContext.HistorialBoletoVirtual.Where(row => row.uIdUsuario == iKey && row.dtFechaVencimiento > DateTime.UtcNow && row.bActivo == true).ToListAsync();
                if (query.Count > 0)
                {
                    var listaHistorialBoletos = BusMapperBoletoVirtual.MapList(query);

                    response.SetSuccess(listaHistorialBoletos);
                }
                else
                {
                    response.SetNotFound(null, Menssages.DatNoExistBoleto);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid iKey): {ex.Message}", iKey, ex, response));
            }
            return response;
        }
    }
}
