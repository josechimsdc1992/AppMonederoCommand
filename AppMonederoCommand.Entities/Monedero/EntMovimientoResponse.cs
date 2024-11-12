namespace AppMonederoCommand.Entities.Monedero
{
    public class EntMovimientoResponse
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *       1       | 26/07/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        */
        public HttpStatusCode httpCode { get; set; }
        public bool hasError { get; set; }
        public int errorCode { get; set; }
        public string? message { get; set; }
        public EntPaginationVista<Movimiento>? result { get; set; }
    }

    public class Movimiento
    {
        public Guid IdMovimientos { get; set; }
        public Guid IdOperacion { get; set; }
        public Guid IdTipoOperacion { get; set; }
        public Guid IdTipoMovimiento { get; set; }
        public Guid IdEstatusTransaccion { get; set; }
        public Guid IdMonedero { get; set; }
        public long NumeroMonedero { get; set; }
        public decimal Monto { get; set; }
        public string? TipoMovimiento { get; set; }
        public string? Operacion { get; set; }
        public string? Motivo { get; set; }
        public DateTime FechaOperacion { get; set; }
        public bool Canelacion { get; set; }
        public long folioMovimiento { get; set; }

    }
}
