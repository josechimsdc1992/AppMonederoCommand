namespace AppMonederoCommand.Entities.Sugerencia;
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

public class EntSugerencia
{
    [JsonProperty("Tipo")]
    [JsonRequired]
    public int iTipo { get; set; }
    [JsonProperty("Comentario")]
    [JsonRequired]
    public string? sComentario { get; set; }
    [JsonProperty("Email")]
    [JsonRequired]
    public string? sEmail { get; set; }
    [JsonProperty("Nombre")]
    [JsonRequired]
    public string? sNombre { get; set; }
    [JsonProperty("Fecha")]
    [JsonRequired]
    public DateTime dtFecha { get; set; }
    [JsonProperty("Unidad")]
    public string? sUnidad { get; set; }
    [JsonProperty("InfraTipo")]
    public string? sInfraTipo { get; set; }
    [JsonProperty("InfraUbicacion")]
    public string? sInfraUbicacion { get; set; }
    [JsonProperty("Ruta")]
    public string? sRuta { get; set; }
    [JsonProperty("IdRuta")]
    public int? iIdRuta { get; set; }
}
