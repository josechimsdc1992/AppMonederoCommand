namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseQR
{
    public class HSMQRDynamicResponse
    {
        public string baseQrCode { get; set; }
        public List<Signature> signatures { get; set; }
        public DateTime date { get; set; }
        public string panHash { get; set; }
    }
}
