using AppMonederoCommand.Entities.Catalogos;
using AppMonederoCommand.Entities.TipoTarifa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Tarjetas
{
    public class EntReadTarjetas
    {
        [JsonProperty("IdTarjeta")]
        public Guid? uIdTarjeta { get; set; }

        [JsonProperty("NumeroTarjeta")]
        public string iNumeroTarjeta { get; set; }

        [JsonProperty("NumeroMonedero")]
        public long iNumeroMonedero { get; set; }

        [JsonProperty("NombreProveedor")]
        public string? sNombreProveedor { get; set; }

        /*[JsonProperty("EstatusTarjeta")]
        public string? sEstatusTarjeta { get; set; }*/

        [JsonProperty("idEstatusTarjeta")]
        public Guid? uIdEstatusTarjeta { get; set; }

        //[JsonProperty("TipoTarifa")]
        //public string? sTipoTarifa { get; set; }

        [JsonProperty("idTipoTarifa")]
        public Guid? uIdTipoTarifa { get; set; }

        [JsonProperty("idSolicitud")]
        public Guid? uIdSolicitud { get; set; }

        [JsonProperty("idMotivo")]
        public Guid? uIdMotivo { get; set; }

        [JsonProperty("CCV")]
        public string? sCCV { get; set; }

        [JsonProperty("FechaFabricacion")]
        public DateTime? dtFechaFabricacion { get; set; }

        /*[JsonProperty("FechaFabricacionTexto")]
        public string? sFechaFabricacion { get; set; }*/

        [JsonProperty("FechaVigencia")]
        public string? sVigencia { get; set; }


        [JsonProperty("Vendida")]
        public bool bVendida { get; set; }

        [JsonProperty("Asociada")]
        public bool bAsociada { get; set; }

        [JsonProperty("Inicializada")]
        public bool bInicializada { get; set; }

        [JsonProperty("Baja")]
        public bool bBaja { get; set; }
        [JsonProperty("Activo")]
        public bool bActivo { get; set; }

        [JsonProperty("marcada")]
        public bool bMarcada { get; set; }
        [JsonProperty("Telefono")]
        public string? sTelefono { get; set; }
        /*[JsonProperty("IdComercio")]
        public Guid? IdComercio { get; set; }*/

        [JsonProperty("IdMonedero")]
        public Guid? uIdMonedero { get; set; }

        /*[JsonProperty("uIdComercioTarjetas")]
        public Guid? uIdComercioTarjetas { get; set; }*/

        [JsonProperty("Folio")]
        public string? sFolio { get; set; }

        [JsonProperty("NumeroProveedor")]
        public int iNumeroProveedor { get; set; }

        [JsonProperty("IdUsuarioTarjeta")]
        public Guid? uIdUsuarioTarjeta { get; set; }

        [JsonProperty("PanHash")]
        public string? sPanHash { get; set; }

        [JsonProperty("SKU")]
        public string? sSKU { get; set; }

        [JsonProperty("FechaValidez")]
        public DateTime? dtFechaValidez { get; set; }

        [JsonIgnore]
        public Guid uIdUsuarioCreacion { get; set; }

        [JsonIgnore]
        public Guid uIdUsuarioModificacion { get; set; }

        [JsonIgnore]
        public Guid uIdUsuarioBaja { get; set; }

        [JsonProperty("IdDetalleSolicitud")]
        public Guid? uIdDetalleSolicitud { get; set; }
        //public EntReadDetalleSolicitud? entDetalleSolicitud { get; set; }
        //public EntReadUsuariosTarjetas? entUsuarioTarjeta { get; set; }
        public EntReadTipoTarifas? entTipoTarifa { get; set; }
        public EntMotivo? entMotivos { get; set; }
        //public EntReadEstatusTarjeta? entEstatusTarjeta { get; set; }
        //public ICollection<EntReadComercioTarjetas>? lstComercioTarjetas { get; set; }
    }
}
