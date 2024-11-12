namespace AppMonederoCommand.Entities.Sugerencia;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 23/08/2023 | César Cárdenas         | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntAddSugerencia
{
    public Guid uIdUsuario { get; set; }
    public DateTime dtFechaRegistro { get; set; }
    public int iTipo { get; set; }
    public string? sComentario { get; set; }
    public string? sEmail { get; set; }
    public string? sNombre { get; set; }
    public DateTime dtFecha { get; set; }
    public string? sUnidad { get; set; }
    public string? sInfraTipo { get; set; }
    public string? sInfraUbicacion { get; set; }
    public string? sRuta { get; set; }
    public int? iIdRuta { get; set; }
    public Guid uIdCreadoPor { get; set; }
    public DateTime dtFechaCreacion { get; set; }
}

