using System.Text.Json.Serialization;

namespace AppMonederoCommand.Entities.TicketsTour
{
    public class EntResponTiposTicketsTour
    {
        [JsonPropertyName("IdTipoTicketsTour")]
        public string IdTipoTicketsTour { get; set; }

        [JsonPropertyName("Nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("Descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }
    }
}
