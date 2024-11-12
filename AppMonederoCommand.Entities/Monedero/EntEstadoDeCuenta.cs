using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Monedero
{
    public class EntEstadoDeCuenta
    {
        public Guid uIdEstadoDeCuenta { get; set; }
        public Guid uIdMonedero { get; set; }
        public Guid uIdTipoTarifa { get; set; }
        public Guid uIdUltimaOperacion { get; set; }
        public Guid uIdEstatus { get; set; }
        public long iNumeroMonedero { get; set; }
        public decimal dSaldo { get; set; }
        public string sTipoTarifa { get; set; }
        public Guid uIdTipoMonedero { get; set; }
        public string sTipoMonedero { get; set; }
        public long? iNumTarjeta { get; set; }
        public string? sTelefono { get; set; }
        public string? sPanHash { get; set; }
        public string sEstatus { get; set; }
        public bool bActivo { get; set; }
        public bool bBaja { get; set; }
        public DateTime? dtFechaUltimaOperacion { get; set; }
        public Guid? uIdMotivo { get; set; }
        public DateTime? dtFechaUltimoAbono { get; set; }
        public DateTime dtFechaCreacion { get; set; }
        public DateTime dtFechaBaja { get; set; }
        public string? sNombre { get; set; }
        public string? sApellidoPaterno { get; set; }
        public string? sApellidoMaterno { get; set; }
        public string? sCorreo { get; set; }
        public DateTime? dtFechaNacimiento { get; set; }
        public string? sFechaVigencia { get; set; }
    }
}
