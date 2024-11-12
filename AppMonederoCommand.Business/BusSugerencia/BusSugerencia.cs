namespace AppMonederoCommand.Business.BusSugerencia;
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

public class BusSugerencia : IBusSugerencia
{
    private readonly ILogger<BusSugerencia> _logger;
    private readonly IDatSugerencia _datSugerencia;
    private readonly IMapper _mapper;
    private readonly IBusUsuario _busUsuario;
    private readonly IBusParametros _busParametros;
    private readonly IBusLenguaje _lenguaje;

    public BusSugerencia(ILogger<BusSugerencia> logger, IDatSugerencia datSugerencia, IMapper mapper, IBusUsuario busUsuario, IBusParametros busParametros, IBusLenguaje lenguaje)
    {
        _logger = logger;
        _datSugerencia = datSugerencia;
        _mapper = mapper;
        _busUsuario = busUsuario;
        _busParametros = busParametros;
        _lenguaje = lenguaje;
    }

    [IMDMetodo(67823462603869, 67823462604646)]
    public async Task<IMDResponse<bool>> BCreate(EntSugerencia sugerenciaJson, Guid uIdUsuario)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(dynamic sugerenciaJson)", sugerenciaJson));

        try
        {
            switch (sugerenciaJson.iTipo)
            {
                case 1:
                    if (sugerenciaJson.sUnidad == string.Empty || sugerenciaJson.sUnidad is null)
                    {
                        response.SetError(Menssages.BusFieldUnid);
                        return response;
                    }
                    break;
                case 2:
                    if (sugerenciaJson.sInfraTipo == string.Empty || sugerenciaJson.sInfraTipo is null)
                    {
                        response.SetError(Menssages.BusFieldInfraType);
                        return response;
                    }
                    if (sugerenciaJson.sInfraUbicacion == string.Empty || sugerenciaJson.sInfraUbicacion is null)
                    {
                        response.SetError(Menssages.BusFieldInfraType);
                        return response;
                    }
                    break;
                case 3:
                    if (sugerenciaJson.sRuta == string.Empty || sugerenciaJson.sRuta is null)
                    {
                        response.SetError(Menssages.BusFieldRoute);
                        return response;
                    }
                    if (sugerenciaJson.iIdRuta is null)
                    {
                        response.SetError(Menssages.BusFieldRoute);
                        return response;
                    }
                    break;
                case 4:

                    break;
                case 5:

                    break;
                default:
                    response.SetError(Menssages.BusFielType);
                    break;
            }

            EntAddSugerencia newItem = _mapper.Map<EntAddSugerencia>(sugerenciaJson);

            response = await _datSugerencia.DSave(newItem);

            if (response.HasError != true)
            {
                string? sCoreos = _busParametros.BObtener("APP_CORREOS_SUGERENCIAS").Result.Result.sValor;
                if (sCoreos != null)
                {
                    List<string> lstCorreos = sCoreos.Split(',').ToList();
                    string sPlantilla = string.Empty;

                    sPlantilla = _lenguaje.BusSetLanguajeSugerencias();
                    if (!string.IsNullOrEmpty(sPlantilla))
                    {
                        string sTipoSugerencia = string.Empty;
                        switch (newItem.iTipo)
                        {
                            case (int)TipoSugerencia.suggestionBox_bus:
                                sTipoSugerencia = TipoSugerencia.suggestionBox_bus.GetDescription();
                                break;
                            case (int)TipoSugerencia.suggestionBox_infra:
                                sTipoSugerencia = TipoSugerencia.suggestionBox_infra.GetDescription();
                                break;
                            case (int)TipoSugerencia.suggestionBox_service:
                                sTipoSugerencia = TipoSugerencia.suggestionBox_service.GetDescription();
                                break;
                            case (int)TipoSugerencia.suggestionBox_app:
                                sTipoSugerencia = TipoSugerencia.suggestionBox_app.GetDescription();
                                break;
                            case (int)TipoSugerencia.suggestionBox_other:
                                sTipoSugerencia = TipoSugerencia.suggestionBox_other.GetDescription();
                                break;
                        }

                        sPlantilla = sPlantilla.Replace("{Tipo}", sTipoSugerencia);
                        sPlantilla = sPlantilla.Replace("{Fecha}", newItem.dtFecha.ToString());
                        sPlantilla = sPlantilla.Replace("{Reportado}", newItem.sNombre);
                        sPlantilla = sPlantilla.Replace("{Contacto}", newItem.sEmail);
                        if (!string.IsNullOrEmpty(newItem.sUnidad))
                        {
                            sPlantilla = sPlantilla.Replace("{Unidad}", newItem.sUnidad);
                            sPlantilla = sPlantilla.Replace("displayUnidad", "block");
                        }
                        else
                        {
                            sPlantilla = sPlantilla.Replace("displayUnidad", "none");
                        }
                        if (!string.IsNullOrEmpty(newItem.sInfraTipo))
                        {
                            sPlantilla = sPlantilla.Replace("{InfraTipo}", newItem.sInfraTipo);
                            sPlantilla = sPlantilla.Replace("displayInfra", "block");
                        }
                        else
                        {
                            sPlantilla = sPlantilla.Replace("displayInfra", "none");
                        }
                        if (!string.IsNullOrEmpty(newItem.sRuta))
                        {
                            sPlantilla = sPlantilla.Replace("{Ruta}", newItem.sRuta);
                            sPlantilla = sPlantilla.Replace("displayRuta", "block");
                        }
                        else
                        {
                            sPlantilla = sPlantilla.Replace("displayRuta", "none");
                        }
                        sPlantilla = sPlantilla.Replace("{Comentario}", newItem.sComentario);
                    }

                    EntBusMessCorreoMultiples entBusMessCorreoMultiples = new EntBusMessCorreoMultiples();
                    entBusMessCorreoMultiples.uIdUsuario = uIdUsuario;
                    entBusMessCorreoMultiples.sMensaje = sPlantilla;
                    entBusMessCorreoMultiples.lstCorreosElectronicos = lstCorreos;
                    entBusMessCorreoMultiples.bHtml = true;
                    entBusMessCorreoMultiples.iRemitente = lstCorreos.Count;

                    await _busUsuario.EnviarMultipleCorreo(entBusMessCorreoMultiples);
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823462604646;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(dynamic sugerenciaJson): {ex.Message}", sugerenciaJson, ex, response));
        }
        return response;
    }

}
