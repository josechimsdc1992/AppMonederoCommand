namespace AppMonederoCommand.Entities.Tarjetas
{
    public class EntVincularTarjeta
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *       0       | 17/07/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        */
        [JsonProperty("NumeroTarjeta")]
        [JsonRequired]
        public string? sNumeroTarjeta { get; set; }
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }
        [JsonProperty("ClaveVerificacion")]
        public int iClaveVerificacion { get; set; }
    }
}
