namespace AppMonederoCommand.Entities.Usuarios.Login
{
  public class EntLoginUsuario
  {
    /* IMASD S.A.DE C.V
  =========================================================================================
  * Descripción: 
  * Historial de cambios:
  * ---------------------------------------------------------------------------------------
  *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
  * ---------------------------------------------------------------------------------------
  *      1        | 24/08/2023 | L.I. Oscar Luna        | Creación        
  * ---------------------------------------------------------------------------------------
  */

    [JsonProperty("IdUsuario")]
    public Guid uIdUsuario { get; set; }

    [JsonProperty("Nombre")]
    public string sNombre { get; set; }

    [JsonProperty("ApellidoPaterno")]
    public string sApellidoPaterno { get; set; }

    [JsonProperty("ApellidoMaterno")]
    public string sApellidoMaterno { get; set; }

    [JsonProperty("Telefono")]
    public string sTelefono { get; set; }

    [JsonProperty("Correo")]
    public string sCorreo { get; set; }

    [JsonProperty("FechaNacimiento")]
    public DateTime? dtFechaNacimiento { get; set; }

    [JsonProperty("CURP")]
    public string? sCURP { get; set; }

    [JsonProperty("Genero")]
    public string cGenero { get; set; }

    [JsonProperty("Fotografia")]
    public string? sFotografia { get; set; }

    [JsonProperty("IdMonedero")]
    public Guid? uIdMonedero { get; set; }

    [JsonProperty("NoMonedero")]
    public string? sNoMonedero { get; set; }

    [JsonProperty("IdCiudad")]
    public int IdCiudad = 1;
    [JsonProperty("Migrado")]
    public bool? bMigrado { get; set; }
  }
}
