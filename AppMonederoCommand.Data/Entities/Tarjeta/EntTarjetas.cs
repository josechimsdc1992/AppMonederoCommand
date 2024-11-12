using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Data.Entities.Tarjeta
{
    public class EntTarjetas : EntAuditoria
    {
        public Guid uIdTarjeta { get; set; }
        public long iNumeroTarjeta { get; set; }
        public long iNumeroMonedero { get; set; }
        public string? sNombreProveedor { get; set; }
        public string sCCV { get; set; }
        public string? sTelefono { get; set; }
        public DateTime dtFechaFabricacion { get; set; }
        public string? sVigencia { get; set; }
        public bool bVendida { get; set; }
        public bool bAsociada { get; set; }
        public bool bInicializada { get; set; }
        public Guid uIdMonedero { get; set; }
        public string? sFolio { get; set; }
        public int iNumeroProveedor { get; set; }
        //public bool bPermitirOperaciones { get; set; }
        //public bool bPermitirReactivar { get; set; }

        // Clave foránea para la relación uno a muchos
        public Guid uIdSolicitud { get; set; }
        //public EntSolicitudes entSolicitudes { get; set; }
        public Guid uIdTipoTarifa { get; set; }
        //public EntTipoTarifa entTipoTarifa { get; set; }
        public Guid uIdEstatusTarjeta { get; set; }
        //public EntEstatusTarjeta entEstatusTarjeta { get; set; }
        public Guid? uIdMotivo { get; set; }
        public Guid? uIdUsuarioTarjeta { get; set; }
        //public EntDBPUsuariosTarjetas? entUsuarioTarjeta { get; set; }
        public string? sSKU { get; set; }
        public string? sPanHash { get; set; }
        public Motivo? entMotivos { get; set; }
        public Guid? uIdDetalleSolicitud { get; set; }
        //public EntDetalleSolicitud? entDetalleSolicitud { get; set; }
        public DateTime? dtFechaValidez { get; set; }
        // Propiedad de navegación para la relación uno a muchos
        //public ICollection<EntBitacoraEstatus> lstBitacoraEstatus { get; set; }
        //public ICollection<EntComercioTarjetas> lstComercioTarjetas { get; set; }
        //public ICollection<EntBitacoraUsuarios> lstBitacoraUsuarios { get; set; }
    }
}
