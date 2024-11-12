namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntUsuarioAppInfo
    {
        [JsonProperty("IdFirebaseToken")]
        public Guid uIdFirebaseToken { get; set; }

        [JsonProperty("Token")]
        public string? sFcmToken { get; set; }

        [JsonProperty("InfoApp")]
        public string? sInfoAppOS { get; set; }

        [JsonProperty("FechaCreacion")]
        public DateTime dtFechaCreacion { get; set; }

        [JsonProperty("FechaModificacion")]
        public DateTime? dtFechaModificacion { get; set; }
    }
}
