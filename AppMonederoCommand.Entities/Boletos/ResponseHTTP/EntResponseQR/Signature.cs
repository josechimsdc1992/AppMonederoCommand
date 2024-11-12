namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseQR
{
    public class Signature
    {
        public string data { get; set; }
        public DateTime validFrom { get; set; }
        public DateTime validUntil { get; set; }
    }
}
