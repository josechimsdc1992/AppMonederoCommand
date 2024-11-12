namespace AppMonederoCommand.Entities.Usuarios.CambioDispositivo
{
    public class EntDispositivoCuentaUpdate
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("IdAplicacion")]
        public string sIdAplicacion { get; set; }

        [JsonProperty("EstatusCuenta")]
        public int iEstatusCuenta { get; set; }
    }
}
