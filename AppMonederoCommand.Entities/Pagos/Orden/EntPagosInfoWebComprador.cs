namespace AppMonederoCommand.Entities.Pagos.Orden
{
    public class EntPagosInfoWebComprador
    {
        public Guid? IdPagosWebInfoComprador { get; set; }
        public Guid? IdOrden { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string? Telefono { get; set; }
        public Guid? IdUsuario { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}

