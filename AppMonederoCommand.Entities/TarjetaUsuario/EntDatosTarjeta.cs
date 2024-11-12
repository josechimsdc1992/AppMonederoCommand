namespace AppMonederoCommand.Entities.TarjetaUsuario
{
    public class EntDatosTarjeta
    {
        public string NumeroTarjeta { get; set; }
        public Guid uIdMonedero { get; set; }
        public decimal dSaldo { get; set; }
        public int iTipoTarjeta { get; set; }
        public string sEstatus { get; set; }
    }
}

