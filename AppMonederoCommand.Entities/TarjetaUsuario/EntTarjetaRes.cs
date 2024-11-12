namespace AppMonederoCommand.Entities.TarjetaUsuario
{
    public class EntTarjetaRes
    {
        public Guid IdTarjeta { get; set; }
        public long NumeroTarjeta { get; set; }
        public long NumeroMonedero { get; set; }
        public string? NombreFabricante { get; set; }
        public string? TokenTarjeta { get; set; }
        public string? EstatusTarjeta { get; set; }
        public Guid? idEstatusTarjeta { get; set; }
        public string? TipoTarifa { get; set; }
        public Guid idTipoTarifa { get; set; }
        public Guid? idSolicitud { get; set; }
        public string? CCV { get; set; }
        public DateTime FechaFabricacion { get; set; }
        public string? FechaFabricacionTexto { get; set; }
        public string? FechaVigencia { get; set; }
        public bool Vendida { get; set; }
        public bool Asociada { get; set; }
        public bool? Inicializada { get; set; }
        public bool Baja { get; set; }
        public bool Activo { get; set; }
        public bool marcada { get; set; }
        public string? Telefono { get; set; }
        public Guid? IdComercio { get; set; }
        public Guid IdMonedero { get; set; }
        public Guid? uIdComercioTarjetas { get; set; }
        public Motivos? entMotivos { get; set; }
    }
    public class Motivos
    {
        public Guid? IdMotivo { get; set; }
        public string? Nombre { get; set; }
        public bool? PermitirOperaciones { get; set; }
        public bool? PermitirReactivar { get; set; }
    }
}

