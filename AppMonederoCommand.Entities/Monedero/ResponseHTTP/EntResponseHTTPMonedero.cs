namespace AppMonederoCommand.Entities.Monedero.ResponseHTTP
{
    //Eliminar al implementar el saldo de monedero 
    public class EntResponseHTTPMonedero
    {
        public Guid iId { get; set; }
        public decimal dSaldo { get; set; }
        public DateTime dtFechaCreacioin { get; set; }
        public DateTime dtFechaActializacion { get; set; }
        public DateTime dtFechaEliminacion { get; set; }
        public bool bActivo { get; set; }
        public Guid iIdCreadoPor { get; set; }
        public Guid iIdActualizadoPor { get; set; }
        public Guid iIdEliminadoPor { get; set; }
    }
}
