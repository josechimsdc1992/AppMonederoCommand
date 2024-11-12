using System.Text.Json.Serialization;
namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseQR
{
    public class EntSignatureQR
    {
        [JsonPropertyName("data")]
        [JsonProperty("data")]
        public string sData { get; set; }

        [JsonPropertyName("validFrom")]
        [JsonProperty("validFrom")]
        public DateTime dtValidFrom { get; set; }

        [JsonPropertyName("validUntil")]
        [JsonProperty("validUntil")]
        public DateTime dtValidUntil { get; set; }
    }
}
