namespace AppMonederoCommand.Entities;

public class EntOrdenRequest
{

    [JsonProperty("OpcionPago")]
    public int iOpcionPago { get; set; }

    [JsonProperty("IdUsuario")]
    public Guid uIdUsuario { get; set; }
    [JsonProperty("Monto")]
    public decimal dMonto { get; set; }
    [JsonProperty("Concepto")]
    public string? sConcepto { get; set; }

    [JsonProperty("IdPaquete")]
    public Guid uIdPaquete { get; set; }

    [JsonProperty("IdMonedero")]
    public Guid uIdMonedero { get; set; }

    [JsonProperty("EmailUsuario")]
    public string? sEmailUsuario { get; set; }

    [JsonProperty("IdAplicacion")]
    public string? sIdAplicacion { get; set; }

    [JsonProperty("InfoWeb")]
    public EntPagosInfoWebComprador? entPagosWebInfoComprador { get; set; }
}

