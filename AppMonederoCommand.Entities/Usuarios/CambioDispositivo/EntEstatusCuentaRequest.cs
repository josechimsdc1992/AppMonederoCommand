namespace AppMonederoCommand.Entities.Usuarios.CambioDispositivo
{
    public class EntEstatusCuentaRequest
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("IdTipoTarifa")]
        public Guid uIdTipoTarifa { get; set; }
    }
}
