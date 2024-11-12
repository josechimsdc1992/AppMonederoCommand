namespace AppMonederoCommand.Entities.Usuarios.ActualizaUsuario
{
    public class EntUpdateUsuarioRedSocial
    {
        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 22/08/2023 | Neftalí Rodríguez      | Creación
       * ---------------------------------------------------------------------------------------
       *      2        | 01/09/2023 | César Cárdenas         | Actulización de campos
       * ---------------------------------------------------------------------------------------
       */

        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("Nombre")]
        public string? sNombre { get; set; }

        [JsonProperty("Correo")]
        public string? sCorreo { get; set; }

        [JsonProperty("ApellidoPaterno")]
        public string? sApellidoPaterno { get; set; }

        [JsonProperty("ApellidoMaterno")]
        public string? sApellidoMaterno { get; set; }

        [JsonProperty("Telefono")]
        public string? sTelefono { get; set; }

        [JsonProperty("Contrasena")]
        [JsonIgnore]
        public string? sContrasena { get; set; }

        [JsonProperty("CodigoVerificacion")]
        [JsonIgnore]
        public string? sCodigoVerificacion { get; set; }


        [JsonProperty("CuentaVerificada")]
        [JsonIgnore]
        public bool bCuentaVerificada { get; set; }

        [JsonProperty("FechaNacimiento")]
        public DateTime? dtFechaNacimiento { get; set; }

        [JsonProperty("CURP")]
        public string? sCURP { get; set; }

        [JsonProperty("Genero")]
        public string? cGenero { get; set; }

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

        [JsonProperty("IdUsuarioModificacion")]
        public Guid? uIdUsuarioModificacion { get; set; }

        [JsonProperty("FechaModificacion")]
        public DateTime? dtFechaModificacion { get; set; }

        [JsonProperty("baja")]
        [JsonIgnore]
        public bool? bBaja { get; set; }

        [JsonProperty("ClavePais")]
        public string? sLada { get; set; }

        [JsonProperty("IdAplicacion")]
        public string? sIdAplicacion { get; set; }

        [JsonProperty("EstatusCuenta")]
        public int iEstatusCuenta { get; set; }
    }
}
