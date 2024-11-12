namespace AppMonederoCommand.Entities.Monedero
{
    public class EntOperacionesMonedero
    {
        [JsonProperty("TodasOperaciones")]
        public bool bTodasOperaciones { get; set; }
        [JsonProperty("Detalles")]
        public bool bDetalles { get; set; }
        [JsonProperty("Movimientos")]
        public bool bMovimientos { get; set; }
        [JsonProperty("Recarga")]
        public bool bRecarga { get; set; }
        [JsonProperty("Traspasos")]
        public bool bTraspasos { get; set; }
        [JsonProperty("GenerarQR")]
        public bool bGenerarQR { get; set; }
        [JsonProperty("VerTarjetas")]
        public bool bVerTarjetas { get; set; }
    }
}
