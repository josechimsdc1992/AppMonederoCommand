using AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseHTTP;
using AppMonederoCommand.Entities.Replicas;
using AppMonederoCommand.Entities.TipoOperaciones;
using AppMonederoCommand.Entities.TipoTarifa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Config
{
    public class IMDParametroConfig
    {
        public string Parametro_Error_Generico { get; set; }
        public string PARAMETRO_APP_TRANSFERIR_DESCRIPCION { get; set; }
        public string PARAMETRO_APP_TRANSFERIR_GUID { get; set; }
        public string PARAMETRO_APP_ABONAR_DESCRIPCION { get; set; }
        public string PARAMETRO_APP_ABONAR_GUID { get; set; }
        public List<EntTipoOperaciones> TipoOperaciones { get; set; }
        public List<EntReplicaTipoTarifas> TipoTarifas { get; set; }
    }
}
