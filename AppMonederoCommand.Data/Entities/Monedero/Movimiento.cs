namespace AppMonederoCommand.Data.Entities.Monedero
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 19/07/2023 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public class Movimiento : IEntAuditoria
    {
        public Guid uId { get; set; }
        public Guid uIdMonedero { get; set; }
        public Guid uIdTipoMovimiento { get; set; }
        public string sTipoMovimiento { get; set; }
        public decimal dSaldoAnterior { get; set; }
        public decimal dImporte { get; set; }
        public decimal dSaldoActual { get; set; }
        public DateTime dtFechaOperacion { get; set; }
        public Guid uIdReferencia { get; set; }
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
