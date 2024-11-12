namespace AppMonederoCommand.Entities;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 03/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntPaquete
{
    [JsonProperty("IdPaquete")]
    public Guid? uIdPaquete { get; set; }

    [JsonProperty("Nombre")]
    public string? sNombre { get; set; }

    [JsonProperty("Descripcion")]
    public string? sDescripcion { get; set; }

    [JsonProperty("Precio")]
    public float fPrecio { get; set; }

    [JsonProperty("Importe")]
    public float fImporte { get; set; }
}
