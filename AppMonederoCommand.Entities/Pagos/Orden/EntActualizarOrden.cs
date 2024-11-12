namespace AppMonederoCommand.Entities;

public class EntActualizarOrden
{
    [JsonProperty("IdOrden")]
    public Guid uIdOrden { get; set; }
    [JsonProperty("IdUsuario")]
    [JsonIgnore]
    public Guid uIdUsuario { get; set; }
    [JsonProperty("Referencia")]
    public string sReferencia { get; set; }
}
