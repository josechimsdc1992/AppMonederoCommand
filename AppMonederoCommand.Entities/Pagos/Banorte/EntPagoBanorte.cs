namespace AppMonederoCommand.Entities;

public class EntPagoBanorte
{
    [JsonProperty("IdOrden")]
    public Guid uIdOrden { get; set; }
    [JsonProperty("ChypherData")]
    public string sChypherData { get; set; }
}

