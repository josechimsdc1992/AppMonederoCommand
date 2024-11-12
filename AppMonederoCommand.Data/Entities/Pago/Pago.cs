namespace AppMonederoCommand.Data.Entities.Pago
{
    public class Pago : IEntAuditoria
    {
        public Guid uIdUsuario { get; set; }
        public decimal dMonto { get; set; }
        public DateTime dtFechaPago { get; set; }
        public string sMetodoPago { get; set; }
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
