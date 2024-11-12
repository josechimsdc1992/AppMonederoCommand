namespace AppMonederoCommand.Entities.Monedero
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *       1       | 19/07/2023 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public class EntTransferirSaldo
    {
        [JsonProperty("IdTipoMovimiento")]
        public Guid uIdTipoMovimiento { get; set; }
        [JsonProperty("IdMonederoOrigen")]
        public Guid uIdMonederoOrigen { get; set; }
        [JsonProperty("IdMonederoDestino")]
        public Guid uIdMonederoDestino { get; set; }
        [JsonProperty("Importe")]
        public decimal dImporte { get; set; }
        [JsonProperty("NumeroTarjetaOrigen")]
        public string? sNumeroTarjetaOrigen { get; set; }
        [JsonProperty("NumeroTarjetaDestino")]
        public string? sNumeroTarjetaDestino { get; set; }

    }

    public class TraspasoSaldoRequestModel
    {
        public decimal MontoTransferencia { get; set; }
        public Guid IdMonederoOrigen { get; set; }
        public Guid IdMonederoDestino { get; set; }
        public string? Operacion { get; set; }
        public Guid IdOperacion { get; set; }
        public DateTime FechaOperacion { get; set; }
        public Guid IdTipoOperacion { get; set; }
        public string? Observaciones { get; set; }
        public long folioMov { get; set; }
    }
}
