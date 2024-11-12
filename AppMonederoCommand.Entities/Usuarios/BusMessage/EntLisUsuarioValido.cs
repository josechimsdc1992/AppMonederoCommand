namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntLisUsuarioValido
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("Telefono")]
        public string? sTelefono { get; set; }

        [JsonProperty("Correo")]
        public string sCorreo { get; set; }

        [JsonProperty("Verificado")]
        public bool bVerificado { get; set; }
    }
}
