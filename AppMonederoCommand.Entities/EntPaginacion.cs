namespace AppMonederoCommand.Entities
{
    public class EntPaginacion
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 06/09/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        */
        [JsonProperty("NumPag")]
        public int iNumPag { get; set; }
        [JsonProperty("NumReg")]
        public int iNumReg { get; set; }
    }
}