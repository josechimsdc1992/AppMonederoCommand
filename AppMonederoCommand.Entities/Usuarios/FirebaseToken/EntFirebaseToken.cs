namespace AppMonederoCommand.Entities.Usuarios.FirebaseToken
{
    public class EntFirebaseToken
    {

        /* IMASD S.A.DE C.V
      =========================================================================================
      * Descripción: 
      * Historial de cambios:
      * ---------------------------------------------------------------------------------------
      *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
      * ---------------------------------------------------------------------------------------
      *      1        | 18/08/2023 | L.I. Oscar Luna        | Creación
      * ---------------------------------------------------------------------------------------
      */


        [JsonProperty("IdFirebaseToken")]
        public Guid uIdFirebaseToken { get; set; }

        [JsonProperty("IdUsuario ")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("FcmToken")]
        public string sFcmToken { get; set; }

        [JsonProperty("InfoAppOS")]
        public string? sInfoAppOS { get; set; }

        [JsonProperty("IdAplicacion")]
        public string? sIdAplicacion { get; set; }

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
