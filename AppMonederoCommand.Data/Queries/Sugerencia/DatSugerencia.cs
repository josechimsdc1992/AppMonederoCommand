namespace AppMonederoCommand.Data.Queries.Sugerencia;
/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 23/08/2023 | Ing. César Cárdenas    | Creación
* ---------------------------------------------------------------------------------------
*/

public class DatSugerencia : IDatSugerencia
{
    protected TransporteContext _dbContext { get; }
    private readonly ILogger<DatUbicacionFavorita> _logger;
    private readonly IMapper _mapper;

    public DatSugerencia(TransporteContext dbContext, ILogger<DatUbicacionFavorita> logger, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    [IMDMetodo(67823462601538, 67823462600761)]
    public async Task<IMDResponse<bool>> DSave(EntAddSugerencia entSugerencia)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntAddSugerencia entSugerencia)", entSugerencia));

        try
        {
            Sugerencias newItem = _mapper.Map<Sugerencias>(entSugerencia);
            _dbContext.Sugerencias.Add(newItem);
            int i = await _dbContext.SaveChangesAsync();

            if (i == 0)
            {
                response.SetError(Menssages.DatNoAddSuggestion);
            }
            else
            {
                response.SetSuccess(true);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntAddSugerencia entSugerencia): {ex.Message}", entSugerencia, ex, response));
        }
        return response;
    }

}
