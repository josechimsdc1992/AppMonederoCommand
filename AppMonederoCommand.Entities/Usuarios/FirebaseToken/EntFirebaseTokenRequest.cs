namespace AppMonederoCommand.Entities.Usuarios.FirebaseToken
{
    public class EntFirebaseTokenRequest
    {
        [JsonProperty("FcmToken")]
        public string sFcmToken { get; set; }

        [JsonProperty("InfoAppOS")]
        public string? sInfoAppOS { get; set; }

        [JsonProperty("IdAplicacion")]
        public string? sIdAplicacion { get; set; }
    }
}