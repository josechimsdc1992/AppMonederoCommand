namespace AppMonederoCommand.Entities;
public class EntPagoCanceladoBanorte
{
    [JsonProperty("IdOrden")]
    public Guid uIdOrden { get; set; }

    [JsonProperty("status3D")]
    public string? sEstatus3D { get; set; }

    [JsonProperty("eci")]
    public string? sECI { get; set; }

    [JsonProperty("id")]
    public string? sId { get; set; }

    [JsonProperty("message")]
    public string? sTextoAdicional { get; set; }

    [JsonProperty("numeroControl")]
    public string? sNumeroControl { get; set; }

    [JsonProperty("resultadoPayw")]
    public string? sResultadoPayworks { get; set; }
}