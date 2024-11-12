namespace AppMonederoCommand.Entities.Usuarios.Login
{
    public class EntLogin
    {

        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 28/07/2023 | L.I. Oscar Luna        | Creación
       * ---------------------------------------------------------------------------------------
       */

        [JsonProperty("Username")]
        public string sUsername { get; set; }

        [JsonProperty("Password")]
        public string? sPassword { get; set; }

        [JsonProperty("IdNetwork")]
        public string? uIdNetwork { get; set; }

        [JsonProperty("Newtwork")]
        public string? sNewtwork { get; set; }

        [JsonProperty("GrantType")]
        public string sGrantType { get; set; }

        [JsonProperty("FcmToken")]
        public string uFcmToken { get; set; }

        [JsonProperty("InfoAppOS")]
        public string? sInfoAppOS { get; set; }

        [JsonProperty("IdAplicacion")]
        public string? sIdAplicacion { get; set; }
    }
}
