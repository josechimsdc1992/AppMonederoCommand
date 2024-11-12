namespace AppMonederoCommand.Entities;

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
public class EntUsuarioActualizaTelefono
{
    [JsonProperty("IdUsuario")]
    public Guid uIdUsuario { get; set; }

    [JsonProperty("Telefono")]
    public string? sTelefono { get; set; }

    [JsonProperty("Correo")]
    public string? sCorreo { get; set; }

    [JsonProperty("CURP")]
    public string? sCURP { get; set; }

    [JsonProperty("CodigoVerificacion")]
    public string? sCodigoVerificacion { get; set; }

    [JsonProperty("CuentaVerificada")]
    public bool bCuentaVerificada { get; set; }

}
