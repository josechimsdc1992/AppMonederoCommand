namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntActualizarEstatusCuenta
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("EstatusCuenta")]
        public int iEstatusCuenta { get; set; }

        [JsonProperty("AccionCuenta")]
        public int iAccionCuenta { get; set; }
    }
}
