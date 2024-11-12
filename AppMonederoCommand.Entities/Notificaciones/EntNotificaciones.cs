namespace AppMonederoCommand.Entities.Notificaciones;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 21/08/2023 | Ing. César Cárdenas    | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntNotificaciones
{
    [JsonProperty("Id")]
    public Guid uId { get; set; }
    [JsonProperty("Titulo")]
    public string? sTitulo { get; set; }
    [JsonProperty("Mensaje")]
    public string? sMensaje { get; set; }
    [JsonProperty("FechaNotificacion")]
    public DateTime dtFechaNotificacion { get; set; }
    [JsonProperty("Leido")]
    public bool bLeido { get; set; }
}
