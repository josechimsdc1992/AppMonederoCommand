namespace AppMonederoCommand.Entities.Boletos.RequestHTTP
{
    public class EntRequestHttpBoletoVirtual
    {
        /* IMASD S.A.DE C.V
      =========================================================================================
      * Descripción: 
      * Historial de cambios:
      * ---------------------------------------------------------------------------------------
      *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
      * ---------------------------------------------------------------------------------------
      *      1        | 14/09/2023 | L.I. Oscar Luna        | Creación
      * ---------------------------------------------------------------------------------------
      */

        [JsonProperty("monedero")]
        public Guid? monedero { get; set; }

        [JsonProperty("tarifa")]
        public Guid? tarifa { get; set; }

        [JsonProperty("saldo")]
        public decimal saldo { get; set; }

        [JsonProperty("cantidad")]
        public int cantidad { get; set; }

        [JsonProperty("claveApp")]
        public string? claveApp { get; set; }
    }
}
