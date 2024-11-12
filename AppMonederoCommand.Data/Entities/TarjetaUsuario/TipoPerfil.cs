namespace AppMonederoCommand.Data.Entities.Tarjetas
{
    public class TipoPerfil : IEntAuditoria
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      0        | 17/07/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        */
        public Guid uIdPerfil { get; set; }
        public string sNombre { get; set; }
        public string sDescripcion { get; set; }

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
