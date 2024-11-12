namespace AppMonederoCommand.Entities.TarjetaUsuario
{
    public class EntOperacionesTarjetas
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

        [JsonProperty("Vincular")]
        public bool bVincular { get; set; }

        [JsonProperty("Visualizar")]
        public bool bVisualizar { get; set; }

        [JsonProperty("GenerarQR")]
        public bool bGenerarQR { get; set; }
    }
}
