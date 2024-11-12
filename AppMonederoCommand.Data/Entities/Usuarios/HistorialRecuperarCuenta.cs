namespace AppMonederoCommand.Data.Entities;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 25/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class HistorialRecuperarCuenta
{
    public Guid uIdHistorialRecuperarCuenta { get; set; }
    public string? sCorreo { get; set; }
    public string? sToken { get; set; }
    public bool bActivo { get; set; }
    public DateTime? dtFechaVencimiento { get; set; }
    public DateTime dtFechaCreacion { get; set; } = DateTime.Now;
    public DateTime? dtFechaModificacion { get; set; }
    public DateTime? dtFechaBaja { get; set; }

}
