namespace AppMonederoCommand.Data.Entities.TarjetaUsuario
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 17/07/2023 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public class TarjetaUsuario
    {
        public Guid uIdTarjetaUsuario { get; set; }
        public Guid uIdTarjeta { get; set; }
        public Guid uIdMonedero { get; set; }
        public Guid uIdUsuario { get; set; }
        public decimal dSaldo { get; set; }
        public string? sNumeroTarjeta { get; set; }
        public long? iNoMonedero { get; set; }
        public string? sTipoTarifa { get; set; }
        public Guid uIdTipoTarifa { get; set; }
        public string? sFechaVigencia { get; set; }
        public int iActivo { get; set; }
        public string? sMotivoBaja { get; set; }
        public string? sMotivoBloqueo { get; set; }
        public int iTipoTarjeta { get; set; }

        //Auditoria
        public DateTime dtFechaCreacion { get; set; } = DateTime.UtcNow;
        public Guid? uIdUsuarioCreacion { get; set; }
        public DateTime dtFechaModificacion { get; set; }
        public DateTime dtFechaBaja { get; set; }
        public bool bActivo { get; set; } = true;
        public bool bBaja { get; set; }
        public Guid? uIdUsuarioModificacion { get; set; }
        public Guid? uIdUsuarioBaja { get; set; }
    }
}
