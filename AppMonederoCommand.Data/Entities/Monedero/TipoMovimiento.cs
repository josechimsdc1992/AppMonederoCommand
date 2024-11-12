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
    public class TipoMovimiento : IEntAuditoria
    {
        public Guid uIdTipoMovimiento { get; set; }
        public string sNombre { get; set; }
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
