namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntLisReenvioCodigo
    {
        [JsonProperty("Correo")]
        public string sCorreo { get; set; }

        [JsonProperty("Telefono")]
        public string? sTelefono { get; set; }

        [JsonProperty("CodigoVerificacion")]
        public string sCodigoVerificacion { get; set; }
    }
}
