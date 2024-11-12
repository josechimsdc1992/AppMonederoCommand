namespace AppMonederoCommand.Entities.Usuarios.JWTEntities
{
    public class EntAutorizacionResponse
    {

        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 04/08/2023 | L.I. Oscar Luna        | Creación
        *          
        * ---------------------------------------------------------------------------------------
        */
        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("Nombre")]
        public string sNombre { get; set; }

        [JsonProperty("ApellidoPaterno")]
        public string sApellidoPaterno { get; set; }

        [JsonProperty("ApellidoMaterno")]
        public string? sApellidoMaterno { get; set; }

        [JsonProperty("Telefono")]
        public string sTelefono { get; set; }

        [JsonProperty("Correo")]
        public string sCorreo { get; set; }

        [JsonProperty("FechaNacimineto")]
        public DateTime? dtFechaNacimineto { get; set; }

        [JsonProperty("Genero")]
        public string cGenero { get; set; }

        [JsonProperty("RedSocial")]
        public string? uIdRedSocial { get; set; }

        [JsonProperty("TokenJWT")]
        public string sTokenJWT { get; set; }

        [JsonProperty("FechaExpiracionToken")]
        public DateTime dtFechaExpiracionToken { get; set; }


        [JsonProperty("RefreshTokenJWT")]
        public string sRefreshTokenJWT { get; set; }
    }
}
