using System.Text.Json.Serialization;
namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseHTTP
{
    public class EntTicket
    {
        /* IMASD S.A.DE C.V
     =========================================================================================
     * Descripción: 
     * Historial de cambios:
     * ---------------------------------------------------------------------------------------
     *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
     * ---------------------------------------------------------------------------------------
     *      01        | 05/10/2023 | L.I. Oscar Luna        | Creación
     * ---------------------------------------------------------------------------------------
     */
        public string IdTicket { get; set; }
        public Guid IdComercio { get; set; }
        public double Tarifa { get; set; }
        public string sCadena { get; set; }
        public string Token { get; set; }
        public double SaldoCuenta { get; set; }
        public int NumSequencial { get; set; }
        public bool Usado { get; set; }
        public DateTime dtFechaUsado { get; set; }
        public string IdTipo { get; set; }
        public string IdMonedero { get; set; }
        public string IdTarifa { get; set; }
        [JsonPropertyName("FechaGeneración")]
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaVigencia { get; set; }
        public bool Activo { get; set; }
        public EntTipoTicket entTipo { get; set; }
        public EntMonedero? entMonedero { get; set; }
        public EntTarifa entTarifa { get; set; }
        public string? ruta { get; set; }
        public HSMQRDynamicResponse qr { get; set; }
        public HSMQRStaticResponse? qrEstatico { get; set; }
        public string? FirmaHSM { get; set; }
        public Guid? uIdSolicitud { get; set; }
        public string claveApp { get; set; }
    }
}
