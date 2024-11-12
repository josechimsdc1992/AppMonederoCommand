using System.Text.Json.Serialization;
namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP
{
    public class EntResponseHttpBoletoVirtual
    {
        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 13/09/2023 | L.I. Oscar Luna        | Creación
       * ---------------------------------------------------------------------------------------
       */
        [JsonPropertyName("IdTicket")]
        [JsonProperty("IdTicket")]
        public Guid uIdTicket { get; set; }

        [JsonPropertyName("Base64String")]
        [JsonProperty("Base64String")]
        public string sBase64String { get; set; }


        [JsonPropertyName("FechaGeneración")]
        [JsonProperty("FechaGeneración")]
        public DateTime? dtFechaGeneracion { get; set; }

        [JsonPropertyName("FechaVigencia")]
        [JsonProperty("FechaVigencia")]
        public DateTime? dtFechaVigencia { get; set; }


        [JsonPropertyName("TipoTarifa")]
        [JsonProperty("TipoTarifa")]
        public int? iTipoTarifa { get; set; }

    }
}
