namespace AppMonederoCommand.Entities;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntUbicacionFavorita
{
    [JsonProperty("Id")]
    public Guid uIdUbicacionFavorita { get; set; }

    [JsonProperty("Etiqueta")]
    public string? sEtiqueta { get; set; }

    [JsonProperty("Direccion")]
    public string? sDireccion { get; set; }

    [JsonProperty("Lat")]
    public float fLatitud { get; set; }

    [JsonProperty("Lon")]
    public float fLongitud { get; set; }

}
