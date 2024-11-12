using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Auth
{
    public class EntDataStorage
    {
        public string? sToken { get; set; }
        public string? sRefreshToken { get; set; }
        public DateTime dtExpiresToken { get; set; }
        public DateTime dtRefreshTokenExpiryTime { get; set; }
    }
}
