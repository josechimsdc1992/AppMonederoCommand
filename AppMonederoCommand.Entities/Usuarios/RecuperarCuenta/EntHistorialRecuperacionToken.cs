namespace AppMonederoCommand.Entities.Usuarios.RecuperarCuenta
{
    public class EntHistorialRecuperacionToken
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 28/08/2023 | L.I. Oscar Luna        | Creación              
        * ---------------------------------------------------------------------------------------
        */

        [JsonProperty("IdHistorialRecuperacionToken")]
        public Guid uIdHistorialRecuperacionToken { get; set; }

        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("Correo")]
        public string sCorreo { get; set; }

        [JsonProperty("Token")]
        public string sToken { get; set; }

        [JsonProperty("Nombre")]
        public string sNombre { get; set; }


        [JsonProperty("FechaVencimiento")]
        [JsonIgnore]
        public DateTime dtFechaVencimiento { get; set; }

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
        #endregion

    }
}
