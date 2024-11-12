namespace AppMonederoCommand.Entities;

public class EntPagoMercadoPago
{
    [JsonProperty("IdPagoMercadoPago")]
    public Guid? uIdPagoMercadoPago { get; set; }
    [JsonProperty("IdOrden")]
    public Guid uIdOrden { get; set; }
    [JsonProperty("IdPago")]
    public string? sIdPago { get; set; }
    [JsonProperty("Estatus")]
    public string? sEstatus { get; set; }
    [JsonProperty("ReferenciaExterna")]
    public string? sReferenciaExterna { get; set; }
    [JsonProperty("TipoPago")]
    public string? sTipoPago { get; set; }
    [JsonProperty("IdOrdenComercial")]
    public string? sIdOrdenComercial { get; set; }
    [JsonProperty("IdSitio")]
    public string? sIdSitio { get; set; }
    [JsonProperty("ModoProcesamiento")]
    public string? sModoProcesamiento { get; set; }
    [JsonProperty("IdCuentaComercial")]
    public string? sIdCuentaComercial { get; set; }
}
