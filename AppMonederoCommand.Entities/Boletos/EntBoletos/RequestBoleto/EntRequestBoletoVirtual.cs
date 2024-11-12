namespace AppMonederoCommand.Entities.Boletos.EntBoletos.RequestBoleto
{
    public class EntRequestBoletoVirtual
    {
        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 14/09/2023 | L.I. Oscar Luna        | Creación
       * ---------------------------------------------------------------------------------------
       */

        [JsonProperty("IdUsuario")]
        public Guid? uIdUsuario { get; set; }

        [JsonProperty("IdMonedero")]
        public Guid uIdMonedero { get; set; }

        [JsonProperty("IdTarifa")]
        public Guid? uIdTarifa { get; set; }

        [JsonProperty("Cantidad")]
        public int iCantidad { get; set; }

        [JsonProperty("NumeroTarjeta")]
        public string? sNumeroTarjeta { get; set; }

        [JsonProperty("IdAplicacion")]
        public string sIdAplicacion { get; set; }
        [JsonProperty("IdSolicitud")]
        public Guid? uIdSolicitud { get; set; }

    }
}
