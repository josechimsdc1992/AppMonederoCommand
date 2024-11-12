namespace AppMonederoCommand.Entities;

public class EntOrdenPaypalRequest
{
    [JsonProperty("IdUsuario")]
    public Guid uIdUsuario { get; set; }
    [JsonProperty("Monto")]
    public decimal dMonto { get; set; }
    [JsonProperty("Concepto")]
    public string? sConcepto { get; set; }
}
