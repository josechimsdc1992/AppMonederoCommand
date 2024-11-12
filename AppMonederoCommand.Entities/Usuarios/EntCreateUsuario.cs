namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntCreateUsuario
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 20/07/2023 | L.I. Oscar Luna        | Creación
        *      2        | 02/08/2023 | L.I. Oscar Luna        | Correción  de entidad para objeto Json
        *      3        | 02/08/2023 | L.I. Oscar Luna        | Ajuste conforme a los datos solicitados
        *      4        | 29/08/2023 | César Cárdenas         | Se cambiaron parametros
        *      5        | 22/09/2023 |  L.I. Oscar Luna       | sCorreo acepete null
        * ---------------------------------------------------------------------------------------
        */


        [JsonProperty("Nombre")]
        public string sNombre { get; set; }

        [JsonProperty("ApellidoPaterno")]
        public string sApellidoPaterno { get; set; }

        [JsonProperty("ApellidoMaterno")]
        public string? sApellidoMaterno { get; set; }

        [JsonProperty("Telefono")]
        public string? sTelefono { get; set; }

        [JsonProperty("Correo")]
        public string? sCorreo { get; set; }

        [JsonProperty("Contrasena")]
        public string? sContrasena { get; set; }

        [JsonProperty("ConfirmaContrasenia")]
        public string? sConfirmaContrasenia { get; set; }

        [JsonProperty("FechaNacimiento")]
        public DateTime? dtFechaNacimiento { get; set; }

        [JsonProperty("CURP")]
        public string? sCURP { get; set; }

        [JsonProperty("Genero")]
        public string cGenero { get; set; }

        [JsonProperty("IdRedSocialGoogle")]
        public string? uIdRedSocialGoogle { get; set; }

        [JsonProperty("RedSocialGoogle")]
        public string? sRedSocialGoogle { get; set; }

        [JsonProperty("IdRedSocialFaceBook")]
        public string? uIdRedSocialFaceBook { get; set; }

        [JsonProperty("RedSocialFaceBook")]
        public string? sRedSocialFaceBook { get; set; }

        [JsonProperty("IdRedSocialApple")]
        public string? uIdRedSocialApple { get; set; }

        [JsonProperty("RedSocialApple")]
        public string? sRedSocialApple { get; set; }

        [JsonProperty("Fotografia")]
        public string? sFotografia { get; set; }

        [JsonProperty("FcmToken")]
        public string? sFcmToken { get; set; }

        [JsonProperty("InfoAppOS")]
        public string? sInfoAppOS { get; set; }
        
        [JsonProperty("ClavePais")]
        public string? sLada { get; set; }

        [JsonProperty("IdAplicacion")]
        public string? sIdAplicacion { get; set; }
    }
}
