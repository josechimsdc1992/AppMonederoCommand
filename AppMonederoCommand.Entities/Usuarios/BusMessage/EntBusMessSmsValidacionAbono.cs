namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntBusMessSmsValidacionAbono
    {
        public Guid uIdUsuario { get; set; }
        public string sNumeroTelefono { get; set; }
        public string sMonto { get; set; }
        public string sTipoTarjeta { get; set; }
        public string sNumeroTarjeta { get; set; }
    }
}
