namespace AppMonederoCommand.Entities.Usuarios.EliminarCuenta
{
    public class EntCodigoCuentaResponse
    {
        [JsonProperty("Correo")]
        public string sCorreo { get; set; }

        [JsonProperty("EncryptedData")]
        public string sEncryptedData { get; set; }
    }
}
