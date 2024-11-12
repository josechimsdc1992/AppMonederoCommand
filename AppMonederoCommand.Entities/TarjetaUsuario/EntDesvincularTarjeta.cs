namespace AppMonederoCommand.Entities.Tarjetas
{
    public class EntDesvincularTarjeta
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
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }
        [JsonProperty("IdTarjeta")]
        public Guid uIdTarjeta { get; set; }
    }
}
