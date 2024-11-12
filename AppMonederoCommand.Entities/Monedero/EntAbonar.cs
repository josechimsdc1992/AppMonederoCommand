namespace AppMonederoCommand.Entities.Monedero
{
    public class EntAbonar
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *       1       | 27/09/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        */
        [Required(ErrorMessage = "El IdMonedero es obligatorio.")]
        [JsonProperty("Monedero")]
        public Guid uIdMonedero { get; set; }
        [JsonProperty("Paquete")]
        public Guid uIdPaquete { get; set; }
        [Required(ErrorMessage = "El Importe es obligatorio.")]
        [JsonProperty("Importe")]
        public decimal dImporte { get; set; }
        [JsonProperty("NumeroTarjeta")]
        public string? sNumeroTarjeta { get; set; }
        [JsonProperty("IdOperacion")]
        public Guid uIdOperacion { get; set; }
        [JsonProperty("Referencia")]
        public string sReferencia { get; set; }

    }

    public class AbonarSaldo
    {
        public decimal monto { get; set; }
        public Guid idMonedero { get; set; }
        public string operacion { get; set; } = "Abonar";
        public Guid idOperacion { get; set; }
        public DateTime fechaOperacion { get; set; } = DateTime.UtcNow;
        public Guid idTipoOperacion { get; set; }
        public Guid? idPaquete { get; set; }
        public string? observaciones { get; set; }
        public long folioVenta { get; set; }
    }
}
