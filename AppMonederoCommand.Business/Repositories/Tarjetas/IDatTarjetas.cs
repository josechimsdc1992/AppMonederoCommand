using AppMonederoCommand.Data.Entities.Tarjeta;
using IMD.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Business.Repositories.Tarjetas
{
    public interface IDatTarjetas: IDatBase<EntReadTarjetas>
    {
        Task<IMDResponse<EntReadTarjetas>> DGetByNumTarjeta(long plTarjeta);
        Task<IMDResponse<EntReadTarjetas>> DGetByuIdMonedero(Guid uidMonedero);
    }
}
