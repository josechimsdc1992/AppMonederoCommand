namespace AppMonederoCommand.Entities.Replicas
{
    public class EntReplicaEstadoCuenta
    {
        public Guid IdEstadoDeCuenta { get; set; }
        public int IdMonedero { get; set; }
        public string NumeroMonedero { get; set; }
        public Guid IdTipoTarifa { get; set; }
        public Guid IdUltimaOperacion { get; set; }
        public string TipoTarifa { get; set; }
        public decimal Total { get; set; }
        public Guid IdEstatusTransaccion { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public bool Baja { get; set; }
        public DateTime FechaUltimoAbono { get; set; }
        public DateTime FechaUltimaOperacion { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
