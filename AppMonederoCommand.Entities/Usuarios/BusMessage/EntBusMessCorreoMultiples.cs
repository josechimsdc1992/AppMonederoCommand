namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntBusMessCorreoMultiples
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }
        [JsonProperty("Mensaje")]
        public string sMensaje { get; set; }
        [JsonProperty("CorreosElectronicos")]
        public List<string> lstCorreosElectronicos { get; set; }
        [JsonProperty("Html")]
        public bool bHtml { get; set; }
        [JsonProperty("Remitente")]
        public int iRemitente { get; set; }
    }
}
