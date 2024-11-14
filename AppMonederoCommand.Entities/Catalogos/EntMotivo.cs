
using AppMonederoCommand.Entities.Tarjetas;
using AppMonederoCommand.Entities.TarjetaUsuario;

namespace AppMonederoCommand.Entities.Catalogos
{
    public class EntMotivo
    {
        public Guid uIdMotivo { get; set; }
        public string? sMotivo { get; set; }
        public bool? bActivo { get; set; }
        public bool? bBaja { get; set; }
        public string? sDescripcion { get; set; }
        public int? iTipo { get; set; }
        public bool? bPermitirOperaciones { get; set; }
        public bool? bPermitirReactivar { get; set; }
        public bool? bPermitirEditar { get; set; }
        public IEnumerable<EntTarjetas_> lstTarjetas { get; set; }
    }
}
