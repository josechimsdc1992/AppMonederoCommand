namespace AppMonederoCommand.Data.Entities;

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
public class UbicacionFavorita
{
    public Guid uIdUbicacionFavorita { get; set; }
    public string? sEtiqueta { get; set; }
    public string? sDireccion { get; set; }
    public decimal fLongitud { get; set; }
    public decimal fLatitud { get; set; }
    public bool bActivo { get; set; } = true;
    public bool bBaja { get; set; } = false;
    public Guid uIdUsuarioCreacion { get; set; }
    public Guid? uIdUsuarioModificacion { get; set; }
    public Guid? uIdUsuarioBaja { get; set; }
    public DateTime dtFechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? dtFechaModificacion { get; set; }
    public DateTime? dtFechaBaja { get; set; }
}

