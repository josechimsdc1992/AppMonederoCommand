using System.Text.Json.Serialization;
namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseHTTP
{
    public class EntTicketQR
    {
        /* IMASD S.A.DE C.V
     =========================================================================================
     * Descripción: 
     * Historial de cambios:
     * ---------------------------------------------------------------------------------------
     *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
     * ---------------------------------------------------------------------------------------
     *      01        | 13/09/2023 | L.I. Oscar Luna        | Creación
     * ---------------------------------------------------------------------------------------
     *      02        | 23/01/2024 | L.I. Oscar Luna        | Ajuste a la nueva estructura de ticket
     * ---------------------------------------------------------------------------------------
     */

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
        public EntQR? ListEntQR { get; set; }

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

        [JsonProperty("BajaMonedero")]
        public bool bBajaMonedero { get; set; }
        [JsonPropertyName("Usado")]
        [JsonProperty("Usado")]
        public bool bUsado { get; set; }
        [JsonPropertyName("uIdSolicitud")]
        [JsonProperty("uIdSolicitud")]
        public Guid? uIdSolicitud { get; set; }

        [JsonPropertyName("claveApp")]
        [JsonProperty("claveApp")]
        public string? sClaveApp { get; set; }

        [JsonPropertyName("FechaServidor")]
        [JsonProperty("FechaServidor")]
        public DateTime? dtFechaServidor { get; set; }
    }
}
