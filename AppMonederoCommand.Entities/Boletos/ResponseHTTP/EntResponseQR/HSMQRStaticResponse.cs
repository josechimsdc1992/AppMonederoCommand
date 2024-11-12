namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseQR
{
    public class HSMQRStaticResponse
    {
        public Signature signature { get; set; }
        public string baseQrCode { get; set; }
        public DateTime date { get; set; }
        public string panHash { get; set; }
    }
}
