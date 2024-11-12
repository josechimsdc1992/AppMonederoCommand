namespace AppMonederoCommand.Data.Queries.TipoTarifa
{
    public class DatTipoTarifas : IDatTipoTarifa
    {
        private readonly TransporteContext _dbContext;
        private readonly ILogger<DatTipoTarifas> _logger;
        private readonly IMapper _mapper;

        public DatTipoTarifas(TransporteContext dbContext, ILogger<DatTipoTarifas> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;   
        }

        [IMDMetodo(67823465930983, 67823465931760)]
        public async Task<IMDResponse<EntReplicaTipoTarifas>> DSave(EntReplicaTipoTarifas newItem)
        {
            IMDResponse<EntReplicaTipoTarifas> response = new IMDResponse<EntReplicaTipoTarifas>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                var mapTiposTarifa = _mapper.Map<TiposTarifa>(newItem);

                _dbContext.TiposTarifa.Add(mapTiposTarifa);
                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(newItem, Menssages.DatAddSuccessInfo);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatNoAddInfo);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                        $"Error en {metodo.sNombre}({metodo.sParametros}): Registró no agregado", newItem, response));
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", newItem, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465932537, 67823465933314)]
        public async Task<IMDResponse<bool>> DUpdate(EntReplicaTipoTarifas entity)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})"));

            try
            {
                var dbmodel = await _dbContext.TiposTarifa.FirstOrDefaultAsync(i => i.uIdTipoTarifa == entity.uIdTipoTarifa);

                dbmodel.sTipoTarifa = entity.sTipoTarifa;
                dbmodel.sClaveTipoTarifa = entity.sClaveTipoTarifa;
                dbmodel.iTipoTarjeta = entity.iTipoTarjeta;

                _dbContext.Attach(dbmodel);
                _dbContext.Entry(dbmodel).Property(x => x.sTipoTarifa).IsModified = true;
                _dbContext.Entry(dbmodel).Property(x => x.sClaveTipoTarifa).IsModified = true;
                _dbContext.Entry(dbmodel).Property(x => x.iTipoTarjeta).IsModified = true;

                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(true, Menssages.DatUpdateSucces);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatUpdateFailed);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                        $"Error en {metodo.sNombre}({metodo.sParametros}): Registró no Actualizado", entity, response));
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entity, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465934091, 67823465934868)]
        public async Task<IMDResponse<bool>> DDelete(Guid iKey)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})"));

            try
            {
                var entity = await _dbContext.TiposTarifa.FindAsync(iKey);
                _dbContext.TiposTarifa.Remove(entity);
                var exec = await _dbContext.SaveChangesAsync();
                if (exec > 0)
                {
                    response.SetSuccess(false, Menssages.DatDeleteSucess);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatDeleteFailed);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                        $"Error en {metodo.sNombre}({metodo.sParametros}): Registró no Eliminado", iKey, response));

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", iKey, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465935645, 67823465936422)]
        public async Task<IMDResponse<EntReplicaTipoTarifas>> DObtenerTipoTarifa(Guid uIdTipoTarifa)
        {
            IMDResponse<EntReplicaTipoTarifas> response = new IMDResponse<EntReplicaTipoTarifas>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                var tipoTarifa = await _dbContext.TiposTarifa.Where(w => w.uIdTipoTarifa == uIdTipoTarifa).FirstOrDefaultAsync();
                if (tipoTarifa != null)
                {
                    EntReplicaTipoTarifas entTipoTarifa = _mapper.Map<EntReplicaTipoTarifas>(tipoTarifa);
                    response.SetSuccess(entTipoTarifa, Menssages.DatGetSuccessInfo);
                }
                else
                {
                    response.SetError(Menssages.DatNoGetRegister);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdTipoTarifa, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465991589, 67823465992366)]
        public async Task<IMDResponse<List<EntReplicaTipoTarifas>>> DGetAll()
        {
            IMDResponse<List<EntReplicaTipoTarifas>> response = new IMDResponse<List<EntReplicaTipoTarifas>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                var lstTipoTarifas = await _dbContext.TiposTarifa.AsNoTracking().OrderBy(i => i.sTipoTarifa).ToListAsync();
                if (lstTipoTarifas != null)
                {
                    var lstEntTipoTarifas = _mapper.Map<List<EntReplicaTipoTarifas>>(lstTipoTarifas);
                    response.SetSuccess(lstEntTipoTarifas, Menssages.DatGetSuccessInfo);
                }
                else
                {
                    response.SetError(Menssages.DatNoExistRegister);
                }
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
