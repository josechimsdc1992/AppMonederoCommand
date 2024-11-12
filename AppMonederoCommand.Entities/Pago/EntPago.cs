namespace AppMonederoCommand.Entities.Pago
{
    public class EntPago
    {
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }
        [JsonProperty("Monto")]
        public decimal dMonto { get; set; }
    }
}
