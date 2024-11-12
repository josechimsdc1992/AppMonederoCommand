using System.Text.Json.Serialization;
namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseQR
{
    public class EntResponseHttpBoletoVirtualQR
    {

        [JsonPropertyName("IdTicket")]
        [JsonProperty("IdTicket")]
        public Guid uIdTicket { get; set; }

        [JsonPropertyName("FechaGeneracion")]
        [JsonProperty("FechaGeneracion")]
        public DateTime dtFechaGeneracion { get; set; }

        [JsonPropertyName("FechaVigencia")]
        [JsonProperty("FechaVigencia")]
        public DateTime dtFechaVigencia { get; set; }

        [JsonPropertyName("QR")]
        [JsonProperty("QR")]
        public EntQR ListEntQR { get; set; }

        [JsonPropertyName("TipoTarifa")]
        [JsonProperty("TipoTarifa")]
        public int? iTipoTarifa { get; set; }

        [JsonPropertyName("sTipoTarifa")]
        [JsonProperty("sTipoTarifa")]
        public string? sTipoTarifa { get; set; }

        [JsonPropertyName("ClaveTarifa")]
        [JsonProperty("ClaveTarifa")]
        [Newtonsoft.Json.JsonIgnore]
        public int? iClaveTarifa { get; set; }

        [JsonPropertyName("NombreTarifa")]
        [JsonProperty("NombreTarifa")]
        [Newtonsoft.Json.JsonIgnore]
        public string? sNombreTarifa { get; set; }

        [JsonPropertyName("uIdSolicitud")]
        [JsonProperty("uIdSolicitud")]
        public Guid? uIdSolicitud { get; set; }

        [JsonPropertyName("ClaveApp")]
        [JsonProperty("ClaveApp")]
        public string? sClaveApp { get; set; }

        [JsonPropertyName("FechaServidor")]
        [JsonProperty("FechaServidor")]
        public DateTime? dtFechaServidor { get; set; }


    }
}
