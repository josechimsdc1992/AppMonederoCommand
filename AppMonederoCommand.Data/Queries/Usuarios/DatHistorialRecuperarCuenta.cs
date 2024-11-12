namespace AppMonederoCommand.Data.Queries;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 25/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class DatHistorialRecuperarCuenta : IDatHistorialRecuperarCuenta
{
    protected TransporteContext _dbContext { get; }
    private readonly ILogger<DatHistorialRecuperarCuenta> _logger;
    private readonly IMapper _mapper;

    public DatHistorialRecuperarCuenta(TransporteContext dbContext, ILogger<DatHistorialRecuperarCuenta> logger, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [IMDMetodo(67823462681569, 67823462682346)]
    public async Task<IMDResponse<EntHistorialRecuperarCuenta>> DGetByCorreo(string sCorreo)
    {
        IMDResponse<EntHistorialRecuperarCuenta> response = new IMDResponse<EntHistorialRecuperarCuenta>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sCorreo)", sCorreo));

        try
        {
            var historialRecuperarCuenta = await _dbContext.HistorialRecuperarCuenta.FirstOrDefaultAsync(row => row.sCorreo == sCorreo && row.bActivo == true);

            if (historialRecuperarCuenta != null)
            {
                EntHistorialRecuperarCuenta model = _mapper.Map<EntHistorialRecuperarCuenta>(historialRecuperarCuenta);
                response.SetSuccess(model);
            }
            else
            {
                response.SetNotFound(new EntHistorialRecuperarCuenta(), Menssages.DatRegisterNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo)", sCorreo));
            }

        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo): {ex.Message}", sCorreo, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462681569, 67823462682346)]
    public async Task<IMDResponse<EntHistorialRecuperarCuenta>> DGetByCorreoAndToken(string sCorreo, string sToken)
    {
        IMDResponse<EntHistorialRecuperarCuenta> response = new IMDResponse<EntHistorialRecuperarCuenta>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sCorreo, string sToken)", sCorreo, sToken));
        try
        {

            var historialRecuperarCuenta = await _dbContext.HistorialRecuperarCuenta.FirstOrDefaultAsync(row => row.sCorreo == sCorreo && row.sToken == sToken);

            if (historialRecuperarCuenta != null)
            {
                EntHistorialRecuperarCuenta model = _mapper.Map<EntHistorialRecuperarCuenta>(historialRecuperarCuenta);
                response.SetCreated(model);
            }
            else
            {
                response.SetNotFound(new EntHistorialRecuperarCuenta(), Menssages.DatRegisterNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo, string sToken)", sCorreo, sToken, response));
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sCorreo, string sToken): {ex.Message}", sCorreo, sToken, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462677684, 67823462676907)]
    public async Task<IMDResponse<EntHistorialRecuperarCuenta>> DSave(EntHistorialRecuperarCuenta newItem)
    {
        IMDResponse<EntHistorialRecuperarCuenta> response = new IMDResponse<EntHistorialRecuperarCuenta>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntHistorialRecuperarCuenta newItem)", newItem));
        try
        {

            HistorialRecuperarCuenta historialRecuperarCuenta = _mapper.Map<HistorialRecuperarCuenta>(newItem);
            _dbContext.HistorialRecuperarCuenta.Add(historialRecuperarCuenta);
            int i = await _dbContext.SaveChangesAsync();

            if (i == 0)
            {
                response.SetError(Menssages.DatTokenNoSave);
            }
            else
            {
                EntHistorialRecuperarCuenta newModel = _mapper.Map<EntHistorialRecuperarCuenta>(historialRecuperarCuenta);
                response.SetCreated(newModel);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntHistorialRecuperarCuenta newItem): {ex.Message}", newItem, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462679238, 67823462678461)]
    public async Task<IMDResponse<bool>> DUpdate(EntHistorialRecuperarCuenta entity)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntHistorialRecuperarCuenta entity)", entity));
        try
        {

            var historialRecuperarCuenta = await _dbContext.HistorialRecuperarCuenta.FirstOrDefaultAsync(row => row.sCorreo == entity.sCorreo);

            if (historialRecuperarCuenta != null)
            {
                historialRecuperarCuenta.bActivo = false;
                historialRecuperarCuenta.dtFechaModificacion = DateTime.Now;

                _dbContext.Attach(historialRecuperarCuenta);

                _dbContext.Entry(historialRecuperarCuenta).Property(x => x.bActivo).IsModified = true;
                _dbContext.Entry(historialRecuperarCuenta).Property(x => x.dtFechaModificacion).IsModified = true;

                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(true, Menssages.DatTokenUsed);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatAccountNoRecovered);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                        $"Error en {metodo}(Guid iKey): " + Menssages.DatAccountNoRecovered, entity.sCorreo ?? "", response));
                }
            }
            else
            {
                response.SetNotFound(false, Menssages.DatRegisterNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid iKey): Cuenta no recuperada", entity.sCorreo ?? "", response));
            }



        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntHistorialRecuperarCuenta entity): {ex.Message}", entity, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462683900, 67823462683123)]
    public async Task<IMDResponse<bool>> DDelete(Guid iKey)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid iKey)", iKey));
        try
        {

            var historialRecuperarCuenta = await _dbContext.HistorialRecuperarCuenta.FindAsync(iKey);

            if (historialRecuperarCuenta != null)
            {
                historialRecuperarCuenta.bActivo = false;
                historialRecuperarCuenta.dtFechaBaja = DateTime.Now;

                _dbContext.Attach(historialRecuperarCuenta);

                _dbContext.Entry(historialRecuperarCuenta).Property(x => x.bActivo).IsModified = true;
                _dbContext.Entry(historialRecuperarCuenta).Property(x => x.dtFechaBaja).IsModified = true;

                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(true, "Token utilizado");
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatAccountNoRecovered);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                        $"Error en {metodo}(Guid iKey): " + Menssages.DatAccountNoRecovered, iKey, response));
                }
            }
            else
            {
                response.SetNotFound(false, Menssages.DatRegisterNoExist);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid iKey): Cuenta no recuperada", iKey, response));
            }

        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoInformacion;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid iKey): {ex.Message}", iKey, ex, response));
        }
        return response;
    }
}
