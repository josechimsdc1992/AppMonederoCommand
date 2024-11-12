namespace AppMonederoCommand.Data.Queries.Catalogos
{
    public class DatTipoOperaciones : IDatTipoOperaciones
    {
        private readonly TransporteContext _dbContext;
        private readonly ILogger<DatTipoOperaciones> _logger;
        private readonly IMapper _mapper;
        public DatTipoOperaciones(TransporteContext dbContext, ILogger<DatTipoOperaciones> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        [IMDMetodo(67823466035101, 67823466035878)]
        public async Task<IMDResponse<bool>> DAgregar(EntTipoOperaciones entTipoOperaciones)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entTipoOperaciones));
            try
            {
                var newTipoOperaciones = _mapper.Map<TipoOperaciones>(entTipoOperaciones);
                _dbContext.TipoOperaciones.Add(newTipoOperaciones);
                if (await _dbContext.SaveChangesAsync() != 0)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetError("Error al agregar el tipo operación.");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entTipoOperaciones, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466036655, 67823466037432)]
        public async Task<IMDResponse<bool>> DActualizar(EntTipoOperaciones entTipoOperaciones)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entTipoOperaciones));
            try
            {
                var dbmodel = await _dbContext.TipoOperaciones.FirstOrDefaultAsync(i => i.uIdTipoOperacion == entTipoOperaciones.uIdTipoOperacion);

                if (dbmodel != null)
                {
                    dbmodel.sNombre = entTipoOperaciones.sNombre;
                    dbmodel.sClave = entTipoOperaciones.sClave;
                    dbmodel.iModulo = entTipoOperaciones.iModulo;

                    _dbContext.Attach(dbmodel);
                    _dbContext.Entry(dbmodel).Property(x => x.sNombre).IsModified = true;
                    _dbContext.Entry(dbmodel).Property(x => x.sClave).IsModified = true;
                    _dbContext.Entry(dbmodel).Property(x => x.iModulo).IsModified = true;

                    if (await _dbContext.SaveChangesAsync() != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.ErrorCode = metodo.iCodigoError;
                        response.SetError("No se actualizó la información.");
                        _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                            $"Error en {metodo.sNombre}({metodo.sParametros}): {"No se actualizó la información."}", entTipoOperaciones, response));
                    }
                }
                else
                {
                    response.SetNotFound(false, "No se encontró el registro.");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entTipoOperaciones, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466038209, 67823466038986)]
        public async Task<IMDResponse<bool>> DEliminar(Guid uIdTipoOperacion)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", uIdTipoOperacion));
            try
            {
                var dbmodel = await _dbContext.TipoOperaciones.FirstOrDefaultAsync(i => i.uIdTipoOperacion == uIdTipoOperacion);
                if (dbmodel != null)
                {
                    dynamic tipoOperacionUpdate = new ExpandoObject();
                    tipoOperacionUpdate.bActivo = false;
                    tipoOperacionUpdate.bBaja = true;

                    _dbContext.Entry(dbmodel).CurrentValues.SetValues(tipoOperacionUpdate);
                    if (await _dbContext.SaveChangesAsync() != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error al eliminar el tipo operación.", uIdTipoOperacion, response));
                        response.SetError("Error al actualizar el tipo operación.");
                    }
                }
                else
                {
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"No se encontró el tipo operación a eliminar.", uIdTipoOperacion, response));
                    response.SetError("No se encontró el tipo operación a eliminar");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdTipoOperacion, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466039763, 67823466040540)]
        public async Task<IMDResponse<List<EntTipoOperaciones>>> DObtenerTipoOperaciones()
        {
            IMDResponse<List<EntTipoOperaciones>> response = new IMDResponse<List<EntTipoOperaciones>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                var lstTipoOperaciones = await _dbContext.TipoOperaciones.AsNoTracking().OrderBy(i => i.sNombre).ToListAsync();
                if (lstTipoOperaciones != null)
                {
                    var lstEntTipoOperaciones = _mapper.Map<List<EntTipoOperaciones>>(lstTipoOperaciones);
                    response.SetSuccess(lstEntTipoOperaciones, Menssages.DatGetSuccessInfo);
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
