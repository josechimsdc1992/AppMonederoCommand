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
    public class Transferencia : IEntAuditoria
    {
        public Guid uIdTransferencia { get; set; }
        public Guid uIdMonederoOrigen { get; set; }
        public Guid uIdMonederoDestino { get; set; }
        public decimal dImporte { get; set; }
        public float fLatitud { get; set; }
        public float fLongitud { get; set; }
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
