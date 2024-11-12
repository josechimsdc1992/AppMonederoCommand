namespace AppMonederoCommand.Entities;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 21/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntAddUbicacionFavorita
{
    [JsonProperty("Id")]
    public Guid? uIdUbicacionFavorita { get; set; }

    [JsonProperty("Etiqueta")]
    public string? sEtiqueta { get; set; }

    [JsonProperty("Direccion")]
    public string? sDireccion { get; set; }

    [JsonProperty("Lat")]
    public decimal fLatitud { get; set; }

    [JsonProperty("Lon")]
    public decimal fLongitud { get; set; }
}
