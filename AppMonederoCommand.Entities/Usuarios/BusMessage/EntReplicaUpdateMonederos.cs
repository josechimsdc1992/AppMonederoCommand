using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntReplicaUpdateMonederos
    {
        public Guid IdMonedero { get; set; }
        public Guid? IdTipoTarifa { get; set; }
        public string TipoTarifa { get; set; }
        public string? NumTelefono { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Correo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FechaVigencia { get; set; }
    }
}