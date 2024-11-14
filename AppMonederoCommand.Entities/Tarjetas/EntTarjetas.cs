using AppMonederoCommand.Entities.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Tarjetas
{
    public class EntTarjetas_ : EntAuditoria
    {
        public Guid? uIdTarjeta { get; set; }
        public long iNumeroTarjeta { get; set; }
        public long iNumeroMonedero { get; set; }
        public string? sNombreProveedor { get; set; }
        //public string? sTokenTarjeta { get; set; }
        //public string? sEstatusTarjeta { get; set; }
        public Guid? uIdEstatusTarjeta { get; set; }
        //public string? sTipoTarifa { get; set; }
        public Guid? uIdTipoTarifa { get; set; }
        public Guid? uIdSolicitud { get; set; }
        public string? sCCV { get; set; }
        public DateTime? dtFechaFabricacion { get; set; }
        public string? sFechaFabricacion { get; set; }
        public string sVigencia { get; set; }
        public DateTime? dtFechaValidez { get; set; }
        public bool bVendida { get; set; }
        public bool bAsociada { get; set; }
        public bool bInicializada { get; set; }
        public bool bMarcada { get; set; }
        public string? sTelefono { get; set; }
        public string? sFolio { get; set; }
        public int? iNumeroProveedor { get; set; }
        public string? sSKU { get; set; }
        public string? sPanHash { get; set; }
        public Guid? uIdUsuarioTarjeta { get; set; }
        public Guid uIdMonedero { get; set; }
        public EntMotivo? entMotivos { get; set; }
        public Guid? uIdMotivo { get; set; }

        [JsonIgnore]
        public Guid uIdUsuarioCreacion { get; set; }

        [JsonIgnore]
        public Guid uIdUsuarioModificacion { get; set; }

        [JsonIgnore]
        public Guid uIdUsuarioBaja { get; set; }
        public Guid? uIdDetalleSolicitud { get; set; }
        //public EntReadEstatusTarjeta entEstatusTarjeta { get; set; }
    }
}
