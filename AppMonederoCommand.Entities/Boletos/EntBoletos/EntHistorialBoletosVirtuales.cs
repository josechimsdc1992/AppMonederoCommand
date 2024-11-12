namespace AppMonederoCommand.Entities.Boletos.EntBoletos
{
    public class EntHistorialBoletosVirtuales
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 13/09/2023 | L.I. Oscar Luna        | Creación
        * ---------------------------------------------------------------------------------------
        */

        [JsonProperty("IdHistorialBoletoVirtual")]
        public Guid uIdHistorialBoletoVirtual { get; set; }

        [JsonProperty("IdUsuario")]
        public Guid? uIdUsuario { get; set; }

        [JsonProperty("IdTicket")]
        public Guid uIdTicket { get; set; }

        [JsonProperty("Boleto")]
        public string sBoleto { get; set; }

        [JsonProperty("TipoTarifa")]
        public string sTipoTarifa { get; set; }

        [JsonProperty("FechaOperacion")]
        [JsonIgnore]
        public DateTime? dtFechaOperacion { get; set; }

        [JsonProperty("FechaVencimiento")]
        [JsonIgnore]
        public DateTime? dtFechaVencimiento { get; set; }

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
