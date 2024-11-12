namespace AppMonederoCommand.Entities;

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
public class EntNotificacionesRes
{
    public HttpStatusCode HttpCode { get; set; }
    public bool HasError { get; set; }
    public string? Message { get; set; }
    public long ErrorCode { get; set; }
    public List<EntNotificacionesLis> Result { get; set; } = new List<EntNotificacionesLis>();
}
