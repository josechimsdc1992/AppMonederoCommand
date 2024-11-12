using System.Text.Json.Serialization;

namespace AppMonederoCommand.Entities.TicketsTour
{
    public class EntResponLadas
    {
        [JsonPropertyName("IdLada")]
        public string IdLada { get; set; }

        [JsonPropertyName("Pais")]
        public string Pais { get; set; }

        [JsonPropertyName("Country")]
        public string Country { get; set; }

        [JsonPropertyName("Lada")]
        public string Lada { get; set; }

        [JsonPropertyName("Tag")]
        public string Tag { get; set; }
    }
}
