namespace AppMonederoCommand.Entities.Usuarios.JWTEntities
{
    public class EntRefreshTokenRequest
    {
        /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 08/08/2023 | L.I. Oscar Luna        | Creación
    * ---------------------------------------------------------------------------------------
    */

        [JsonProperty("TokenExpirado")]
        public string? sTokenExpirado { get; set; }

        [JsonProperty("RefreshToken")]
        public string? sRefreshToken { get; set; }
    }
}
