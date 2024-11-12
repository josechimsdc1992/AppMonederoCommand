namespace AppMonederoCommand.Entities.Usuarios.Login
{
    public class EntLoginResponse
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
        [JsonProperty("Usuario")]
        public EntLoginUsuario entUsuario { get; set; }


        [JsonProperty("Token")]
        public EntLoginToken entToken { get; set; }

        [JsonProperty("UrlMapa")]
        public string? sUrlMapa { get; set; }

        [JsonProperty("RequiereCode")]
        public bool? bRequiereCode { get; set; }
    }
}
