namespace AppMonederoCommand.Entities.DB
{
    [Table("ESTADODECUENTA", Schema = "APP")]
    public class EstadoDeCuentaReplica
    {
        [Key]
        [Column("UIDESTADODECUENTA", TypeName = "VARCHAR2(50)")]
        [Required]
        public Guid IdEstadoDeCuenta { get; set; }

        [Column("UIDMONEDERO", TypeName = "VARCHAR2(50)")]
        [Required]
        public Guid IdMonedero { get; set; }

        [Column("UIDTIPOTARIFA", TypeName = "VARCHAR2(50)")]
        [Required]
        public Guid IdTipoTarifa { get; set; }

        [Column("UIDULTIMAOPERACION", TypeName = "VARCHAR2(50)")]
        [Required]
        public Guid IdUltimaOperacion { get; set; }

        [Column("UIDESTATUS", TypeName = "VARCHAR2(50)")]
        [Required]
        public Guid IdEstatus { get; set; }

        [Column("INUMMONEDERO")]
        [Required]
        public long NumeroMonedero { get; set; }

        [Column("DSALDO", TypeName = "DECIMAL")]
        [Required]
        public decimal Saldo { get; set; }

        [Column("STIPOTARIFA", TypeName = "VARCHAR2(20)")]
        [Required]
        public string TipoTarifa { get; set; }

        [Column("UIDTIPOMONEDERO", TypeName = "VARCHAR2(50)")]
        public Guid? IdTipoMonedero { get; set; }

        [Column("STIPOMONEDERO", TypeName = "VARCHAR2(20)")]
        public string? TipoMonedero { get; set; }

        [Column("INUMTARJETA", TypeName = "NUMBER(16)")]
        public long? NumTarjeta { get; set; }

        [Column("STELEFONO", TypeName = "VARCHAR2(20)")]
        public string? Telefono { get; set; }

        [Column("SPANHASH", TypeName = "VARCHAR2(100)")]
        public string? PanHash { get; set; }

        [Column("SESTATUS", TypeName = "VARCHAR2(20)")]
        [Required]
        public string Estatus { get; set; }

        [Column("BACTIVO")]
        [Required]
        public bool Activo { get; set; }

        [Column("BBAJA")]
        [Required]
        public bool Baja { get; set; }

        [Column("DTFECHAULTIMAOPERACION")]
        public DateTime? FechaUltimaOperacion { get; set; }

        [Column("UIDMOTIVO", TypeName = "VARCHAR2(50)")]
        public Guid? uIdMotivo { get; set; }

        [Column("DTFECHAULTIMOABONO")]
        public DateTime? FechaUltimoAbono { get; set; }

        [Column("DTFECHACREACION")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Column("DTFECHABAJA")]
        public DateTime? FechaBaja { get; set; }

        [Column("SNOMBRE", TypeName = "VARCHAR2(200)")]
        public string? Nombre { get; set; }

        [Column("SAPELLIDOPATERNO", TypeName = "VARCHAR2(200)")]
        public string? ApellidoPaterno { get; set; }

        [Column("SAPELLIDOMATERNO", TypeName = "VARCHAR2(200)")]
        public string? ApellidoMaterno { get; set; }

        [Column("SCORREO", TypeName = "VARCHAR2(200)")]
        public string? Correo { get; set; }

        [Column("DTFECHANACIMIENTO", TypeName = "DATE")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("SFECHAVIGENCIA", TypeName = "VARCHAR2(200)")]
        [Required]
        public string? FechaVigencia_ { get; set; }

    }
}
