namespace AppMonederoCommand.Data.Queries;
public class DatUbicacionFavorita : IDatUbicacionFavorita
{
    protected TransporteContext _dbContext { get; }
    private readonly ILogger<DatUbicacionFavorita> _logger;
    private readonly IMapper _mapper;

    public DatUbicacionFavorita(TransporteContext dbContext, ILogger<DatUbicacionFavorita> logger, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [IMDMetodo(67823462460124, 67823462459347)]
    public async Task<IMDResponse<List<EntGetAllUbicacionFavorita>>> DGetAllByIdUsuario(Guid uIdUsuario)
    {
        IMDResponse<List<EntGetAllUbicacionFavorita>> response = new IMDResponse<List<EntGetAllUbicacionFavorita>>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();

        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

        try
        {
            var listaUbicaciones = await _dbContext.UbicacionFavorita.Where(row => row.uIdUsuarioCreacion == uIdUsuario && row.bActivo == true).ToListAsync();
            if (listaUbicaciones != null)
            {
                var result = _mapper.Map<List<EntGetAllUbicacionFavorita>>(listaUbicaciones.ToList());

                response.SetSuccess(result, Menssages.DatGetLocatiosSucces);
            }
            else
            {
                response.GetNotFound(Menssages.DatNoExistFavoriteUbications);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462534716, 67823462533939)]
    public async Task<IMDResponse<EntUbicacionFavorita>> DSave(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario)
    {
        IMDResponse<EntUbicacionFavorita> response = new IMDResponse<EntUbicacionFavorita>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EndAddUbicacionFavorita ubicacionFavorita)", ubicacionFavorita));

        try
        {
            UbicacionFavorita newItem = _mapper.Map<UbicacionFavorita>(ubicacionFavorita);
            newItem.uIdUsuarioCreacion = uIdUsuario;
            _dbContext.UbicacionFavorita.Add(newItem);
            int i = await _dbContext.SaveChangesAsync();

            if (i == 0)
            {
                response.SetError(Menssages.DatLocationNoRegistered);
            }
            else
            {
                EntUbicacionFavorita newModel = _mapper.Map<EntUbicacionFavorita>(newItem);
                response.SetCreated(newModel);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EndAddUbicacionFavorita ubicacionFavorita): {ex.Message}", ubicacionFavorita, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462581336, 67823462580559)]
    public async Task<IMDResponse<bool>> DDelete(Guid iKey, Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid iKey, Guid uIdUsuario)", iKey, uIdUsuario));

        try
        {

            var entity = await _dbContext.UbicacionFavorita.FindAsync(iKey);

            if (entity != null)
            {
                entity.bActivo = false;
                entity.bBaja = true;
                entity.dtFechaBaja = DateTime.UtcNow;
                entity.uIdUsuarioBaja = uIdUsuario;

                _dbContext.Attach(entity);

                _dbContext.Entry(entity).Property(x => x.bActivo).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.bBaja).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.dtFechaBaja).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.uIdUsuarioBaja).IsModified = true;

                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(true, Menssages.DatDeleteSucess);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatDeleteFailed);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion,
                        $"Error en {metodo}(Guid iKey): " + Menssages.DatDeleteFailed, iKey, response));
                }
            }
            else
            {
                response.SetNotFound(false, Menssages.DatLocationFavoriteNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Error en {metodo}(Guid iKey): Registro no eliminado", iKey, response));
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

    [IMDMetodo(67823462592214, 67823462591437)]
    public async Task<IMDResponse<bool>> DUpdate(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario)", ubicacionFavorita, uIdUsuario));

        try
        {

            var entity = await _dbContext.UbicacionFavorita.FindAsync(ubicacionFavorita.uIdUbicacionFavorita);

            if (entity != null)
            {
                entity.sEtiqueta = ubicacionFavorita.sEtiqueta;
                entity.sDireccion = ubicacionFavorita.sDireccion;
                entity.fLatitud = ubicacionFavorita.fLatitud;
                entity.fLongitud = ubicacionFavorita.fLongitud;
                entity.dtFechaModificacion = DateTime.UtcNow;
                entity.uIdUsuarioModificacion = uIdUsuario;

                _dbContext.Attach(entity);

                _dbContext.Entry(entity).Property(x => x.sEtiqueta).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.sDireccion).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.fLatitud).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.fLongitud).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.dtFechaModificacion).IsModified = true;
                _dbContext.Entry(entity).Property(x => x.uIdUsuarioModificacion).IsModified = true;

                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(true, Menssages.DatUpdateSucces);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatUpdateFailed);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion,
                        $"Error en {metodo}(Guid iKey): " + Menssages.DatUpdateFailed, ubicacionFavorita.uIdUbicacionFavorita!, response));
                }
            }
            else
            {
                response.SetNotFound(false, Menssages.DatLocationFavoriteNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Error en {metodo}(Guid iKey): Registro no actualizado", ubicacionFavorita.uIdUbicacionFavorita!, response));
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario): {ex.Message}", ubicacionFavorita, uIdUsuario, ex, response));
        }
        return response;
    }
}