namespace AppMonederoCommand.Entities.DTO
{
    public class EntCreateReplicaMonederos
    {
        public Guid IdEstadoDeCuenta { get; set; }
        public Guid IdMonedero { get; set; }
        public string NumeroMonedero { get; set; }
        public Guid IdTipoTarifa { get; set; }
        public Guid IdUltimaOperacion { get; set; }
        public string TipoTarifa { get; set; }
        public decimal Saldo { get; set; }
        public Guid IdEstatusMonedero { get; set; }
        public string Estatus { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public bool Baja { get; set; }
        public DateTime? FechaUltimoAbono { get; set; }
        public DateTime? FechaUltimaOperacion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVigencia { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Correo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FechaVigencia_ { get; set; }
        public Guid IdTipoMonedero { get; set; }
        public string TipoMonedero { get; set; }
        public Guid? uIdMotivo { get; set; }
    }
}
