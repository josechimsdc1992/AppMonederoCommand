using System.Text.Json.Serialization;

namespace AppMonederoCommand.Entities.Usuarios.Http.Response
{
    public class EntResponseHttpMonederoC
    {
        [JsonPropertyName("idMonedero")]
        [JsonProperty("idMonedero")]
        public Guid IdMonedero { get; set; }

        [JsonPropertyName("numMonedero")]
        [JsonProperty("numMonedero")]
        public long NumMonedero { get; set; }

        [JsonPropertyName("idTiposMonedero")]
        [JsonProperty("idTiposMonedero")]
        public Guid IdTiposMonedero { get; set; }

        [JsonPropertyName("idTipoTarifa")]
        [JsonProperty("idTipoTarifa")]
        public Guid IdTipoTarifa { get; set; }

        [JsonPropertyName("telefono")]
        [JsonProperty("telefono")]
        public string? Telefono { get; set; }

        [JsonPropertyName("fechaVigencia_")]
        [JsonProperty("fechaVigencia_")]
        public string? FechaVigencia_ { get; set; }

        #region Base
        [JsonPropertyName("activo")]
        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonPropertyName("baja")]
        [JsonProperty("baja")]
        public bool Baja { get; set; }

        [JsonPropertyName("fechaCreacion")]
        [JsonProperty("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonPropertyName("fechaModificacion")]
        [JsonProperty("fechaModificacion")]
        public DateTime? FechaModificacion { get; set; }

        [JsonPropertyName("fechaBaja")]
        [JsonProperty("fechaBaja")]
        public DateTime? FechaBaja { get; set; }

        [JsonPropertyName("idUsuarioCreacion")]
        [JsonProperty("idUsuarioCreacion")]
        public Guid IdUsuarioCreacion { get; set; }

        [JsonPropertyName("idUsuarioModificacion")]
        [JsonProperty("idUsuarioModificacion")]
        public Guid? IdUsuarioModificacion { get; set; }

        [JsonPropertyName("idUsuarioBaja")]
        [JsonProperty("idUsuarioBaja")]
        public Guid? IdUsuarioBaja { get; set; }

        #endregion
    }
}
