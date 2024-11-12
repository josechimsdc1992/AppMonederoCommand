namespace AppMonederoCommand.Data.Entities.Pago
{
    public class Tarjeta : IEntAuditoria
    {
        public Guid uIdTarjeta { get; set; }
        public Guid uIdUsuario { get; set; }
        public string sNumeroTarjeta { get; set; }
        public string sNombreTitular { get; set; }
        public string sFechaVencimiento { get; set; }
        public string sNumeroSeguridad { get; set; }
        public string sTipoTarjeta { get; set; }
        public bool bEstatusTarjeta { get; set; } = true;
        public bool bDefault { get; set; }
        public DateTime dtFechaRegistro { get; set; }
        //Auditoria
        public DateTime dtFechaCreacion { get; set; }
        public Guid uIdUsuarioCreacion { get; set; }
        public DateTime dtFechaModificacion { get; set; }
        public DateTime dtFechaBaja { get; set; }
        public bool bActivo { get; set; }
        public bool bBaja { get; set; }
        public Guid uIdUsuarioModificacion { get; set; }
        public Guid uIdUsuarioBaja { get; set; }

    }
}
