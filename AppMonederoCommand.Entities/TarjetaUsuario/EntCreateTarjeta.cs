namespace AppMonederoCommand.Entities.Tarjetas
{
    public class EntCreateTarjeta
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *       0       | 17/07/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        *       1       | 21/10/2023 | César Cárdenas         | Actualización
        * ---------------------------------------------------------------------------------------
        */
        public Guid uIdTarjetaUsuario { get; set; }
        public Guid uIdTarjeta { get; set; }
        public Guid uIdMonedero { get; set; }
        public Guid uIdUsuario { get; set; }
        public decimal dSaldo { get; set; }
        public long? sNumeroTarjeta { get; set; }
        public long? iNoMonedero { get; set; }
        public string? sTipoTarifa { get; set; }
        public Guid uIdTipoTarifa { get; set; }
        public string? sFechaVigencia { get; set; }
        public int iActivo { get; set; }
        public string? sMotivoBaja { get; set; }
        public string? sMotivoBloqueo { get; set; }
        public int iTipoTarjeta { get; set; }
    }
}