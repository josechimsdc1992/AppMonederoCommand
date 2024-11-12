using System.Text.Json.Serialization;
namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseQR
{
    public class EntQR
    {
        [JsonPropertyName("baseQrCode")]
        [JsonProperty("baseQrCode")]
        public string sBaseQrCode { get; set; }

        [JsonPropertyName("signatures")]
        [JsonProperty("signatures")]
        public List<EntSignatureQR> EntSignatures { get; set; }

        [JsonPropertyName("date")]
        [JsonProperty("date")]
        public DateTime dtDate { get; set; }

        [JsonPropertyName("panHash")]
        [JsonProperty("panHash")]
        public string sPanHash { get; set; }


    }
}
