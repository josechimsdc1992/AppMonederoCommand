using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Config
{
    public class IMDServiceConfig
    {
        public string Seguridad_Host { get; set; }
        public string Seguridad_Login { get; set; }
        public string Seguridad_Refresh { get; set; }
        public string Seguridad_UserName { get; set; }
        public string Seguridad_Password { get; set; }
       
        public string MonederoC_Host { get; set; }
        public string MonederoC_Abonar { get; set; }

    }
}
