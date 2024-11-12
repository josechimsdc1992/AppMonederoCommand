using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Config
{
    public class IMDServiceConfig
    {
        public string Tarifa { get; set; }
        public string Seguridad_Host { get; set; }
        public string Seguridad_Login { get; set; }
        public string Seguridad_Refresh { get; set; }
        public string Seguridad_UserName { get; set; }
        public string Seguridad_Password { get; set; }
        public string HSM_Host { get; set; }
        public string HSM_FirmarQREstatico { get; set; }
        public string HSM_FirmarQRDinamico { get; set; }
        public bool HSM_DUMMY { get; internal set; }
        public string Catalogos_Host { get; set; }
        public string Catalogos_TipoTarifas { get; set; }
        public string MonederoQ_Host { get; set; }
        public string MonederoQ_VistaMonedero { get; set; }
        public string Sincronizador_Host { get; set; }
        public string Sincronizador_MaxTarifa { get; set; }
    }
}
