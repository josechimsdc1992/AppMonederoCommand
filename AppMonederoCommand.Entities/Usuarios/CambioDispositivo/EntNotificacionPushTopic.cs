namespace AppMonederoCommand.Entities.Usuarios.CambioDispositivo
{
    public class EntNotificacionPushTopic
    {
        [JsonProperty("Titulo")]
        public string sTitulo { get; set; }

        [JsonProperty("Mensaje")]
        public string sMensaje { get; set; }

        [JsonProperty("ImagenURL")]
        public string? sImagenURL { get; set; }

        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("TopicUsuario")]
        public EntTopicUsuario entTopicUsuario { get; set; }
    }

    public class EntTopicUsuario
    {
        [JsonProperty("Topic")]
        public string sTopic { get; set; }

        [JsonProperty("Data")]
        public string sData { get; set; }

        [JsonProperty("ActionCode")]
        public string sActionCode { get; set; }
    }
}
