using IMD.Utils.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Business.Interfaces.Tarjeta
{
    public interface IBusTarjetas:IBusBase<EntReadTarjetas, EntReadTarjetas, EntReadTarjetas>
    {

        Task<IMDResponse<EntReadTarjetas>> BGetByNumTarjeta(long plTarjeta);
    }
}
