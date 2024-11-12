namespace AppMonederoCommand.Entities.Tarjetas
{
    public class EntTarjetaUsuario
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *       0       | 05/09/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        *       1       | 21/10/2023 | César Cárdenas         | Actualización
        * ---------------------------------------------------------------------------------------
        */
        [JsonProperty("IdTarjeta")]
        public Guid uIdTarjeta { get; set; }
        [JsonProperty("IdMonedero")]
        public Guid uIdMonedero { get; set; }
        [JsonProperty("Saldo")]
        public decimal dSaldo { get; set; }
        [JsonProperty("NumeroTarjeta")]
        public string? sNumeroTarjeta { get; set; }
        [JsonProperty("NoMonedero")]
        public long iNoMonedero { get; set; }
        [JsonProperty("NombreTipoTarifa")]
        public string? sTipoTarifa { get; set; }
        [JsonProperty("IdTipoTarifa")]
        public Guid uIdTipoTarifa { get; set; }
        [JsonProperty("FechaVigencia")]
        public string sFechaVigencia { get; set; }
        [JsonProperty("Activo")]
        public bool bActivo { get; set; }
        [JsonProperty("MotivoBaja")]
        public string? sMotivoBaja { get; set; }
        [JsonProperty("MotivoBloqueo")]
        public string? sMotivoBloqueo { get; set; }
        [JsonProperty("TipoTarjeta")]
        public int iTipoTarjeta { get; set; }
        [JsonProperty("Operaciones")]
        public EntOperacionesTarjetas? entOperaciones { get; set; }

        [JsonProperty("BajaMonedero")]
        public bool bBajaMonedero { get; set; }

        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("MonederoVirtual")]
        public bool bMonederoVirtual { get; set; }
    }
}