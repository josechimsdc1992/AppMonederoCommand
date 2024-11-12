namespace AppMonederoCommand.Data.Queries.Catalogos
{
    using DatMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Catalogos.EntMotivo, AppMonederoCommand.Data.Entities.Catalogos.Motivo>;
    public class DatMotivos : IDatMotivos
    {
        private readonly TransporteContext _dbContext;
        private readonly ILogger<DatMotivos> _logger;
        private readonly IMapper _mapper;
        public DatMotivos(TransporteContext dbContext, ILogger<DatMotivos> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        [IMDMetodo(67823465917774, 67823465916997)]
        public async Task<IMDResponse<bool>> DAgregar(EntMotivo entMotivo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntMotivo entMotivo)", entMotivo));

            try
            {
                var newMotivo = DatMapper.MapEntity(entMotivo);
                _dbContext.Motivo.Add(newMotivo);
                if (await _dbContext.SaveChangesAsync() != 0)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetError("Error al agregar el motivo");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntMotivo entMotivo): {ex.Message}", entMotivo, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465919328, 67823465918551)]
        public async Task<IMDResponse<bool>> DActualizar(EntMotivo entMotivo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntMotivo entMotivo)", entMotivo));

            try
            {
                var motivo = await _dbContext.Motivo.FirstOrDefaultAsync(mot => mot.uIdMotivo == entMotivo.uIdMotivo);
                if (motivo != null)
                {
                    dynamic motivoUpdate = new ExpandoObject();
                    motivoUpdate.sMotivo = entMotivo.sMotivo;
                    motivoUpdate.bActivo = entMotivo.bActivo;
                    motivoUpdate.bBaja = entMotivo.bBaja;
                    motivoUpdate.sDescripcion = entMotivo.sDescripcion;
                    motivoUpdate.iTipo = entMotivo.iTipo;
                    motivoUpdate.bPermitirOperaciones = entMotivo.bPermitirOperaciones;
                    motivoUpdate.bPermitirReactivar = entMotivo.bPermitirReactivar;
                    motivoUpdate.bPermitirEditar = entMotivo.bPermitirEditar;

                    _dbContext.Entry(motivo).CurrentValues.SetValues(motivoUpdate);
                    if (await _dbContext.SaveChangesAsync() != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error al actualizar el motivo.", entMotivo, response));
                        response.SetError("Error al actualizar el motivo.");
                    }
                }
                else
                {
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"No se encontró el motivo a actualizar.", entMotivo, response));
                    response.SetError("No se encontró el motivo a actualizar");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntMotivo entMotivo): {ex.Message}", entMotivo, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465920882, 67823465920105)]
        public async Task<IMDResponse<bool>> DEliminar(Guid uIdMotivo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMotivo)", uIdMotivo));

            try
            {
                var motivo = await _dbContext.Motivo.FirstOrDefaultAsync(mot => mot.uIdMotivo == uIdMotivo);
                if (motivo != null)
                {
                    dynamic motivoUpdate = new ExpandoObject();
                    motivoUpdate.bActivo = false;
                    motivoUpdate.bBaja = true;

                    _dbContext.Entry(motivo).CurrentValues.SetValues(motivoUpdate);
                    if (await _dbContext.SaveChangesAsync() != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error al eliminar el motivo.", uIdMotivo, response));
                        response.SetError("Error al actualizar el motivo.");
                    }
                }
                else
                {
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"No se encontró el motivo a eliminar.", uIdMotivo, response));
                    response.SetError("No se encontró el motivo a eliminar");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMotivo): {ex.Message}", uIdMotivo, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465957401, 67823465958178)]
        public async Task<IMDResponse<EntMotivo>> DObtenerMotivo(Guid uIdMotivo)
        {
            IMDResponse<EntMotivo> response = new IMDResponse<EntMotivo>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                var motivo = await _dbContext.Motivo.Where(w => w.uIdMotivo == uIdMotivo).FirstOrDefaultAsync();
                if (motivo != null)
                {
                    EntMotivo entMotivo = _mapper.Map<EntMotivo>(motivo);
                    response.SetSuccess(entMotivo, Menssages.DatGetSuccessInfo);
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
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdMotivo, ex, response));
            }
            return response;
        }
    }
}
