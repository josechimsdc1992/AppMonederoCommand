namespace AppMonederoCommand.Entities.Usuarios.RefreshToken
{
    public class EntRefreshTokenResponse
    {

        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 24/08/2023 | L.I. Oscar Luna        | Creación        
        * ---------------------------------------------------------------------------------------
        */


        [JsonProperty("AccessToken")]
        public string sAccessToken { get; set; }

        [JsonProperty("RefreshToken")]
        public string sRefreshToken { get; set; }

        [JsonProperty("FechaExpiracion")]
        public DateTime dtFechaExpiracion { get; set; }

    }
}
