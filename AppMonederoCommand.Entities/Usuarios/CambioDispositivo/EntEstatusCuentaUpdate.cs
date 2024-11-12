namespace AppMonederoCommand.Entities.Usuarios.CambioDispositivo
{
    public class EntEstatusCuentaUpdate
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("IdTipoTarifa")]
        public Guid uIdTipoTarifa { get; set; }

        [JsonProperty("EstatusCuenta")]
        public int iEstatusCuenta { get; set; }
    }
}
