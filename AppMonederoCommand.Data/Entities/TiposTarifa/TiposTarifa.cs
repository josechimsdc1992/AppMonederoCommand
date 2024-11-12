using AppMonederoCommand.Data.Entities.Tarjeta;

namespace AppMonederoCommand.Data.Entities.TiposTarifa
{
    public class TiposTarifa
    {
        public Guid uIdTipoTarifa { get; set; }
        public string sTipoTarifa { get; set; }
        public string sClaveTipoTarifa { get; set; }
        public int iTipoTarjeta { get; set; }

        public ICollection<EntTarjetas> lstTarjetas { get; set; }
    }
}
