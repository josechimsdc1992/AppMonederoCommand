using AppMonederoCommand.Entities.Usuarios.FirebaseToken;

namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntUsuario
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 20/07/2023 | L.I. Oscar Luna        | Creación
        *      2        | 02/08/2023 | L.I. Oscar Luna        | Se ignoran campos en el json de salida
        *      3        | 29/08/2023 | César Cárdenas         | Se cambiaron parametros
        *      4        | 22/09/2023 |  L.I. Oscar Luna       | sCorreo acepete null
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
        public string? sTelefono { get; set; }

        [JsonProperty("Correo")]
        public string? sCorreo { get; set; }

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

        [JsonProperty("IdMonedero")]
        public Guid? uIdMonedero { get; set; }

        [JsonProperty("NoMonedero")]
        public string? sNoMonedero { get; set; }

        [JsonProperty("Migrado")]
        public bool? bMigrado { get; set; }

        [JsonProperty("FechaVenciemientoContrasena")]
        [JsonIgnore]
        public DateTime? dtFechaVencimientoContrasena { get; set; }

        [JsonProperty("FirebaseTokens")]
        public List<EntFirebaseToken>? entFirebaseTokens { get; set; }

        [JsonProperty("ClavePais")]
        public string? sLada { get; set; }

        [JsonProperty("IdAplicacion")]
        public string? sIdAplicacion { get; set; }

        [JsonProperty("EstatusCuenta")]
        public int iEstatusCuenta { get; set; }

        [JsonProperty("PermiteEditar")]
        public bool bPermiteEditar { get; set; }

        #region Auditoria
        [JsonProperty("FechaCreacion")]
        [JsonIgnore]
        public DateTime dtFechaCreacion { get; set; }

        [JsonProperty("FechaModificacion")]
        [JsonIgnore]
        public DateTime? dtFechaModificacion { get; set; }

        [JsonProperty("FechaBaja")]
        [JsonIgnore]
        public DateTime? dtFechaBaja { get; set; }

        [JsonProperty("Activo")]
        [JsonIgnore]
        public bool bActivo { get; set; }

        [JsonProperty("Baja")]
        [JsonIgnore]
        public bool? bBaja { get; set; }

        [JsonProperty("IdUsuarioCreacion")]
        [JsonIgnore]
        public Guid? uIdUsuarioCreacion { get; set; }

        [JsonProperty("IdUsuarioModificacion")]
        [JsonIgnore]
        public Guid? uIdUsuarioModificacion { get; set; }

        [JsonProperty("IdUsuarioBaja")]
        [JsonIgnore]
        public Guid? uIdUsuarioBaja { get; set; }

        [JsonProperty("BajaMonedero")]
        public bool bBajaMonedero { get; set; }

        #endregion


    }
}
