using Microsoft.AspNetCore.Http;

namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntBusMessCorreo
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }
        [JsonProperty("Mensaje")]
        public string sMensaje { get; set; }
        [JsonProperty("CorreoElectronico")]
        public string sCorreoElectronico { get; set; }
        [JsonProperty("Html")]
        public bool bHtml { get; set; }
        [JsonProperty("Files")]
        public IFormFileCollection? fFiles { get; set; }
    }
}
