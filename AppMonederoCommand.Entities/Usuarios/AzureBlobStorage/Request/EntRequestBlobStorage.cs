namespace AppMonederoCommand.Entities.Usuarios.AzureBlobStorage.Request
{
    public class EntRequestBlobStorage
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("Imagen")]
        public string sImagen { get; set; }
    }
}
