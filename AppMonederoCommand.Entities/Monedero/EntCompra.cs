namespace AppMonederoCommand.Entities.Monedero;

public class EntCompra
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *       1       | 19/07/2023 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    [JsonProperty("NombrePaquete")]
    public string? sNombrePaquete { get; set; }
    [JsonProperty("FechaOperacion")]
    public DateTime dtFechaOperacion { get; set; }
    [JsonProperty("Importe")]
    public decimal dImporte { get; set; }
}