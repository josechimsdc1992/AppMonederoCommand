namespace AppMonederoCommand.Entities;

public class EntPagoPayPal
{
    [JsonProperty("IdPagoPayPal")]
    public Guid? uIdPagoPayPal { get; set; }
    [JsonProperty("IdOrden")]
    public Guid uIdOrden { get; set; }
    [JsonProperty("IdReferencia")]
    public Guid? uIdReferencia { get; set; }
    [JsonProperty("IdPagador")]
    public string? sIdPagador { get; set; }
    [JsonProperty("NombreCompleto")]
    public string? sNombreCompleto { get; set; }
    [JsonProperty("Correo")]
    public string? sCorreo { get; set; }
    [JsonProperty("Estatus")]
    public string? sEstatus { get; set; }
    [JsonProperty("FechaPago")]
    public DateTime dtFechaPago { get; set; }
    [JsonProperty("IdPago")]
    public string? sIdPago { get; set; }
}

