namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntBusMessCorreoValidacionAbono
    {
        public Guid uIdUsuario { get; set; }
        public string sCorreo { get; set; }
        public string sMonto { get; set; }
        public string sNombre { get; set; }
        public string sTipoTarjeta { get; set; }
        public string sNumeroTarjeta { get; set; }
        public string sFolio { get; set; }
        public DateTime dtFecha { get; set; }
        public string sConcepto { get; set; }


    }
}
