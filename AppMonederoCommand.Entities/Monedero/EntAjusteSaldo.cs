namespace AppMonederoCommand.Entities.Monedero
{
    public class EntAjusteSaldo
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
        [JsonProperty("IdTipoMovimiento")]
        public Guid uIdTipoMovimiento { get; set; }
        [JsonProperty("Importe")]
        public decimal dImporte { get; set; }
        [JsonProperty("Latitud")]
        public float fLatitud { get; set; }
        [JsonProperty("Longitud")]
        public float fLongitud { get; set; }
    }
}
