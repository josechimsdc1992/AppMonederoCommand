namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntUsuarioApp
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 07/06/2024 | David Realpozo         | Creación
        * ---------------------------------------------------------------------------------------
        *      2        | 16/07/2024 | Neftali Rodriguez      | Se agrega propiedad Nombre
        * ---------------------------------------------------------------------------------------
        */

        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("NombreCompleto")]
        public string sNombreCompleto { get; set; }

        [JsonProperty("Nombre")]
        public Nombre? Nombre { get; set; }

        [JsonProperty("Lada")]
        public string? sLada { get; set; }

        [JsonProperty("Telefono")]
        public string? sTelefono { get; set; }

        [JsonProperty("Correo")]
        public string? sCorreo { get; set; }

        [JsonProperty("CuentaVerificada")]
        public bool bCuentaVerificada { get; set; }

        [JsonProperty("FechaNacimiento")]
        public DateTime? dtFechaNacimiento { get; set; }

        [JsonProperty("CURP")]
        public string? sCURP { get; set; }

        [JsonProperty("Genero")]
        public string cGenero { get; set; }

        [JsonProperty("EsGoogle")]
        public bool bEsGoogle { get; set; }

        [JsonProperty("EsFacebook")]
        public bool bEsFacebook { get; set; }

        [JsonProperty("EsApple")]
        public bool bEsApple { get; set; }

        [JsonProperty("Fotografia")]
        public string? sFotografia { get; set; }

        [JsonProperty("Monedero")]
        public bool bMonedero { get; set; }

        [JsonProperty("IdMonedero")]
        public Guid? uIdMonedero { get; set; }

        [JsonProperty("Migrado")]
        public bool bMigrado { get; set; }

        [JsonProperty("IdAplicacion")]
        public string? sIdAplicacion { get; set; }

        [JsonProperty("EstatusCuenta")]
        public string sEstatusCuenta { get; set; }

        #region Auditoria
        [JsonProperty("FechaCreacion")]
        public DateTime dtFechaCreacion { get; set; }

        [JsonProperty("FechaModificacion")]
        public DateTime? dtFechaModificacion { get; set; }

        [JsonProperty("FechaBaja")]
        public DateTime? dtFechaBaja { get; set; }

        [JsonProperty("Activo")]
        public bool bActivo { get; set; }

        [JsonProperty("Baja")]
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

    public class Nombre
    {
        [JsonProperty("Nombre")]
        public string sNombre { get; set; }
        [JsonProperty("ApellidoPaterno")]
        public string sApellidoPaterno { get; set; }
        [JsonProperty("ApellidoMaterno")]
        public string? sApellidoMaterno { get; set; }
    }
}
