namespace AppMonederoCommand.Entities.Boletos.EntBoletos.RequestBoleto
{
    public class EntRequestConsumeBoleto
    {
        [JsonProperty("IdTicket")]
        public Guid uIdTicket { get; set; }
    }
}
