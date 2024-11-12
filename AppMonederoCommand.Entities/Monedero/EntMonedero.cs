namespace AppMonederoCommand.Entities.Monedero
{
    public class EntMonedero
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *       1       | 19/07/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        */
        [JsonProperty("IdMonedero")]
        public Guid uIdMonedero { get; set; }
        [JsonProperty("Saldo")]
        public decimal dSaldo { get; set; }

    }
}
