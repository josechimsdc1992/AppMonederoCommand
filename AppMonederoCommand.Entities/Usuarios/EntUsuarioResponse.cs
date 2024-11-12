namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntUsuarioResponse
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 18/08/2023 | L.I. Oscar Luna        | Creación        
        * ---------------------------------------------------------------------------------------
        */


        [JsonProperty("ReintentosSegundos")]
        public string sReintentosSegundos { get; set; }

        [JsonProperty("MaxReintentos")]
        public string sMaxReintentos { get; set; }
    }
}
