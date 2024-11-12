namespace AppMonederoCommand.Entities.Usuarios.EliminarCuenta
{
    public class EntCodigoCuentaRequest
    {
        [JsonProperty("Correo")]
        public string sCorreo { get; set; }

        [JsonProperty("Comentario")]
        public string sComentario { get; set; }

    }
}
