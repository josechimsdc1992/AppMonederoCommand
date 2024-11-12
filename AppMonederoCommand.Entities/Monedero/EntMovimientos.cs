namespace AppMonederoCommand.Entities.Monedero
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
    public class EntMovimientos
    {
        public List<Operaciones>? Operaciones { get; set; }
        public EntPaginacion? Paginacion { get; set; }
    }

    public class Operaciones
    {
        [JsonProperty("ImporteGeneral")]
        public decimal dImporteGeneral { get; set; }

        [JsonProperty("IdOperacion")]
        public string uIdOperacion { get; set; }

        [JsonProperty("Operacion")]
        public string? sOperacion { get; set; }

        [JsonProperty("TipoMovimiento")]
        public string? sTipoMovimiento { get; set; }

        [JsonProperty("FechaOperacion")]
        public string dtFechaOperacion { get; set; }

        [JsonProperty("Importe")]
        public decimal dImporte { get; set; }

        [JsonProperty("NumeroMonedero")]
        public string iNumeroMonedero { get; set; }

        [JsonProperty("TipoTarjeta")]
        public int iTipoTarjeta { get; set; }

        [JsonProperty("Concepto")]
        public string sConcepto { get; set; }

        [JsonProperty("isVentaSaldo")]
        public bool BIsVentaSaldo { get; set; }

        [JsonProperty("NombreTarifa")]
        public string sNombreTarifa { get; set; }
    }
}
