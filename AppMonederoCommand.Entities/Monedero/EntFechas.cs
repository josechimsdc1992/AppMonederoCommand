namespace AppMonederoCommand.Entities.Monedero
{
    public class EntFechas
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
        [JsonProperty("FechaInicio")]
        public DateTime dtFechaInicio { get; set; }
        [JsonProperty("FechaFin")]
        public DateTime dtFechaFin { get; set; }

    }
}
