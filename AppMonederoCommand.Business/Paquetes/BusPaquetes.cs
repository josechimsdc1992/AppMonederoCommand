namespace AppMonederoCommand.Business;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 03/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class BusPaquetes : IBusPaquetes
{

    private readonly ILogger<BusUbicacionFavorita> _logger;
    private string URLBase;
    private string endPointGetAllPaquetes;
    private readonly IServGenerico _servGenerico;
    private readonly IMapper _mapper;
    private readonly IBusParametros _busParametros;

    public BusPaquetes(ILogger<BusUbicacionFavorita> logger, IServGenerico servGenerico, IMapper mapper, IBusParametros busParametros)
    {
        URLBase = Environment.GetEnvironmentVariable("URLBASE_PAQUETES") ?? string.Empty;
        endPointGetAllPaquetes = Environment.GetEnvironmentVariable("ENDPOINT_GET_PAQUETES_APP") ?? string.Empty;
        _servGenerico = servGenerico;
        _logger = logger;
        _mapper = mapper;
        _busParametros = busParametros;
    }

    [IMDMetodo(67823463271312, 67823463270535)]
    public async Task<IMDResponse<dynamic>> BGetAll(string token)
    {
        IMDResponse<dynamic> response = new IMDResponse<dynamic>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}"));

        try
        {
            var uProductoId = _busParametros.BObtener("APP_PRODUCTO_ID").Result.Result.sValor;
            var paquetesResponse = await _servGenerico.SGetPath(URLBase, endPointGetAllPaquetes, uProductoId, token);

            if (paquetesResponse.HasError)
            {
                return response.GetResponse(paquetesResponse);
            }

            var obj = JsonSerializer.Deserialize<EntPaquetesProductosResponse>(paquetesResponse.Result.ToString()!);

            List<EntPaquete> entPaquetes = new List<EntPaquete>();
            if (obj!.lstPaquetes.Count > 0) {
                obj!.lstPaquetes.ForEach(item => {
                    entPaquetes.Add(new EntPaquete()
                    {
                        fImporte = item.fImporte,
                        fPrecio = item.fPrecioUnitario,
                        sDescripcion = item.sDescripcionPaquete,
                        sNombre = item.sNombre,
                        uIdPaquete = item.uIdPaquete
                    });
                });
            }

            response.SetSuccess(entPaquetes, Menssages.BusCompleteCorrect);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}: {ex.Message}", ex, response));
        }
        return response;
    }
}
