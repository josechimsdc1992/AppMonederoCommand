namespace AppMonederoCommand.Entities.Monedero
{
    public class EntSaldo
    {
        [JsonProperty("Saldo")]
        public decimal dSaldo { get; set; }
        [JsonProperty("VigenciaTarjeta")]
        public string? sVigenciaTarjeta { get; set; }
        [JsonProperty("IdTipoTarifa")]
        public Guid uIdTipoTarifa { get; set; }
        [JsonProperty("TipoTarifa")]
        public string? sTipoTarifa { get; set; }
        [JsonProperty("TipoTarjeta")]
        public int iTipoTarjeta { get; set; }
        [JsonProperty("Operaciones")]
        public EntOperacionesMonedero? entOperacionesMonedero { get; set; }
        [JsonProperty("BajaMonedero")]
        public bool bBajaMonedero { get; set; }
    }
}
