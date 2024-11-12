namespace AppMonederoCommand.Data.Queries;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class DatUsuarioActualizaTelefono : IDatUsuarioActualizaTelefono
{
    protected TransporteContext _dbContext { get; }
    private readonly ILogger<DatUsuarioActualizaTelefono> _logger;
    private readonly IMapper _mapper;

    public DatUsuarioActualizaTelefono(TransporteContext dbContext, ILogger<DatUsuarioActualizaTelefono> logger, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [IMDMetodo(67823463470224, 67823463469447)]
    public async Task<IMDResponse<EntUsuarioActualizaTelefono>> DGetByIdUsuario(Guid uIdUsuario)
    {

        IMDResponse<EntUsuarioActualizaTelefono> response = new IMDResponse<EntUsuarioActualizaTelefono>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario, uIdUsuario));

        try
        {

            var model = await _dbContext.UsuarioActualizaTelefono.FindAsync(uIdUsuario);

            if (model != null)
            {
                EntUsuarioActualizaTelefono entity = _mapper.Map<EntUsuarioActualizaTelefono>(model);
                response.SetSuccess(entity, Menssages.DatCompleteSucces);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid iKey): {ex.Message}", uIdUsuario, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463471778, 67823463471001)]
    public async Task<IMDResponse<bool>> DSave(EntUsuarioActualizaTelefonoRequest usuario, Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EndAddUbicacionFavorita ubicacionFavorita)", usuario));

        try
        {

            UsuarioActualizaTelefono newItem = _mapper.Map<UsuarioActualizaTelefono>(usuario);
            newItem.uIdUsuario = uIdUsuario;
            _dbContext.UsuarioActualizaTelefono.Add(newItem);
            int i = await _dbContext.SaveChangesAsync();

            if (i == 0)
            {
                response.SetError(Menssages.DatOcurredError);
            }
            else
            {
                response.SetSuccess(true, Menssages.DatCompleteSucces);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EndAddUbicacionFavorita ubicacionFavorita): {ex.Message}", usuario, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463473332, 67823463472555)]
    public async Task<IMDResponse<bool>> DUpdate(EntUsuarioActualizaTelefonoRequest entUsuario, Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntUsuarioActualizaTelefonoRequest usuario, Guid uIdUsuario)", entUsuario, uIdUsuario));

        try
        {

            var model = await _dbContext.UsuarioActualizaTelefono.FindAsync(uIdUsuario);

            if (model != null)
            {
                model.sCodigoVerificacion = entUsuario.sCodigoVerificacion;
                model.sCorreo = entUsuario.sCorreo;
                model.sTelefono = entUsuario.sTelefono;
                model.dtFechaCreacion = DateTime.Now;

                _dbContext.Attach(model);

                _dbContext.Entry(model).Property(x => x.sCodigoVerificacion).IsModified = true;
                _dbContext.Entry(model).Property(x => x.sCorreo).IsModified = true;
                _dbContext.Entry(model).Property(x => x.sTelefono).IsModified = true;
                _dbContext.Entry(model).Property(x => x.dtFechaCreacion).IsModified = true;

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
                        $"Error en {metodo}(Guid uIdUsuario): " + Menssages.DatUpdateFailed, uIdUsuario, response));
                }
            }
            else
            {
                response.SetNotFound(false, Menssages.DatRegisterNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Error en {metodo}(Guid uIdUsuario): Registro no existe", uIdUsuario, response));
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntUsuarioActualizaTelefonoRequest usuario, Guid uIdUsuario): {ex.Message}", entUsuario, uIdUsuario, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463474886, 67823463474109)]
    public async Task<IMDResponse<bool>> DVerificado(Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

        try
        {
            var model = await _dbContext.UsuarioActualizaTelefono.FindAsync(uIdUsuario);

            if (model != null)
            {
                model.BVerificado = true;
                model.dtFechaModificacion = DateTime.UtcNow;

                _dbContext.Attach(model);

                _dbContext.Entry(model).Property(x => x.BVerificado).IsModified = true;
                _dbContext.Entry(model).Property(x => x.dtFechaModificacion).IsModified = true;
                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(true, Menssages.DatVerificateSucces);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatVerificateFailed);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion,
                        $"Error en {metodo}(Guid uIdUsuario): " + Menssages.DatVerificateFailed, uIdUsuario, response));
                }
            }
            else
            {
                response.SetNotFound(false, Menssages.DatRegisterNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Error en {metodo}(Guid uIdUsuario): Registro no verificado", uIdUsuario, response));
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
        }
        return response;

        throw new NotImplementedException();
    }
}
