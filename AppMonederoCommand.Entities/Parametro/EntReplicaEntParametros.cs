using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Parametro
{
    public class EntReplicaEntParametros
    {
        public Guid? uIdParametro { get; set; }
        public string sNombre { get; set; }
        public string sValor { get; set; }
        public DateTime dtFechaCreacion { get; set; }
        public DateTime? dtFechaModificacion { get; set; }
        public DateTime? dtFechaBaja { get; set; }
        public bool bActivo { get; set; }
        public bool bBaja { get; set; }
        public Guid? uIdUsuarioCreacion { get; set; }
        public Guid? uIdUsuarioModificacion { get; set; }
        public Guid? uIdUsuarioBaja { get; set; }
        public bool? bEncriptado { get; set; }
        public string? sDescripcion { get; set; }
    }
}
