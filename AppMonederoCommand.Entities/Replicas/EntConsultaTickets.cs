using AppMonederoCommand.Entities.Enums;

namespace AppMonederoCommand.Entities.Replicas
{
    public class EntConsultaTickets
    {
        public Guid? uIdMonedero { get; set; }
        public string? claveApp { get; set; }
        public Guid? uIdSolicitud { get; set; }
        public bool bUsado { get; set; }
        public bool bVigente { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
        public eOpcionesTicket sOpcion { get; set; }
    }
}
