namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntBusMessPushValidacionAbono
    {
        public Guid uIdUsuario { get; set; }
        public List<string> lstTokens { get; set; }
        public string sMonto { get; set; }
        public string sTipoTarjeta { get; set; }
        public string sNumeroTarjeta { get; set; }
    }
}
