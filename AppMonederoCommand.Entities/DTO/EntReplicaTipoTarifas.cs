using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.DTO
{
    public class EntReplicaTipoTarifas
    {
        public Guid uIdTipoTarifa { get; set; }
        public string sTipoTarifa { get; set; }
        public string sClaveTipoTarifa { get; set; }
        public int iTipoTarjeta { get; set; }
    }
}
