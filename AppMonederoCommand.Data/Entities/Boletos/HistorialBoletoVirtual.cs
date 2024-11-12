namespace AppMonederoCommand.Data.Entities.Boletos
{
    public class HistorialBoletoVirtual
    {
        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 13/09/2023 | L.I. Oscar Luna        | Creación
       * ---------------------------------------------------------------------------------------
       */

        public Guid uIdHistorialBoletoVirtual { get; set; }
        public Guid uIdUsuario { get; set; }
        public Guid uIdTicket { get; set; }
        public string sBoleto { get; set; }
        public string sTipoTarifa { get; set; }
        public DateTime? dtFechaOperacion { get; set; }
        public DateTime? dtFechaVencimiento { get; set; }

        #region Auditoria        
        public DateTime dtFechaCreacion { get; set; }
        public DateTime? dtFechaModificacion { get; set; }
        public DateTime? dtFechaBaja { get; set; }
        public bool bActivo { get; set; }
        public bool? bBaja { get; set; }
        public Guid? uIdUsuarioCreacion { get; set; }
        public Guid? uIdUsuarioModificacion { get; set; }
        public Guid? uIdUsuarioBaja { get; set; }
        #endregion
    }
}
