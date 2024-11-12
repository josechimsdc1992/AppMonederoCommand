namespace AppMonederoCommand.Entities.Usuarios.ActualizaUsuario
{
    public class EntUpdateUsuarioByMonedero
    {
        public Guid uIdUsuario { get; set; }
        public string sNombre { get; set; }
        public string sApellidoPaterno { get; set; }
        public string? sApellidoMaterno { get; set; }
        public string? sTelefono { get; set; }
        public string? sCorreo { get; set; }
        public DateTime? dtFechaNacimiento { get; set; }
        public string? sCURP { get; set; }
        public string cGenero { get; set; }
        public DateTime? dtFechaModificacion { get; set; }
        public Guid? uIdMonedero { get; set; }
        public Guid? uIdUsuarioModificacion { get; set; }
        public int? iEstatusCuentaApp { get; set; }
    }
}