namespace AppMonederoCommand.Entities.Monedero;

public class EntTransaccion
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

    [JsonProperty("IdMonedero")]
    public Guid uIdMonedero { get; set; }
    [JsonProperty("Monto")]
    public decimal dMonto { get; set; }
    [JsonProperty("sOperacion")]
    public string? sOperacion { get; set; }
}