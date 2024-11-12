namespace AppMonederoCommand.Data.Entities;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class UsuarioActualizaTelefono
{
    public Guid uIdUsuario { get; set; }
    public string? sTelefono { get; set; }
    public string? sCorreo { get; set; }
    public string? sCURP { get; set; }
    public string? sCodigoVerificacion { get; set; }
    public bool BVerificado { get; set; }
    public DateTime dtFechaCreacion { get; set; } = DateTime.Now;
    public DateTime? dtFechaModificacion { get; set; }
}
