namespace AppMonederoCommand.Entities.Usuarios.EliminarCuenta
{
    public class EntEliminaCuentaRequest
    {
        [JsonProperty("Code")]
        public string sCode { get; set; }

        [JsonProperty("EncryptedData")]
        public string sEncryptedData { get; set; }
    }
}
