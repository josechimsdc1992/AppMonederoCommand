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
public class EntNotificacionesLis
{
    public Guid IdHistorial { get; set; }
    public string? Destinatario { get; set; }
    public string? Titulo { get; set; }
    public string? Contenido { get; set; }
    public bool Enviado { get; set; }
    public DateTime Fecha { get; set; }
    public string? Metodo { get; set; }
    public bool Leido { get; set; }
}
