namespace AppMonederoCommand.Entities.Sender
{
    public class EntNotificacionMultiplePush
    {
        [JsonProperty("Tokens")]
        public List<EntToken> lstTokens { get; set; }
        [JsonProperty("Titulo")]
        public string sTitulo { get; set; }
        [JsonProperty("Mensaje")]
        public string sMensaje { get; set; }
        [JsonProperty("ImagenURL")]
        public string? sImagenURL { get; set; }
    }
    public class EntToken
    {
        public Guid uIdUsuario { get; set; }
        public string sToken { get; set; }
    }
}
