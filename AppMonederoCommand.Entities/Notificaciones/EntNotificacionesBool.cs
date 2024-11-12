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
public class EntNotificacionesBool
{
    public HttpStatusCode HttpCode { get; set; }
    public bool HasError { get; set; }
    public string? Message { get; set; }
    public long ErrorCode { get; set; }
    public bool Result { get; set; }
}
