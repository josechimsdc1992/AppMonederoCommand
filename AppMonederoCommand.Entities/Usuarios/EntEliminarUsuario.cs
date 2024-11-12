namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntEliminarUsuario
    {
        public Guid uIdUsuario { get; set; }
        public bool bCuentaVerificada { get; set; }
        public string? sTelefono { get; set; }
        public string? sContrasena { get; set; }
        public string? sCURP { get; set; }
        public string? sFotografia { get; set; }
        public bool bActivo { get; set; }
        public bool bBaja { get; set; }
        public Guid uIdUsuarioBaja { get; set; }
        public DateTime? dtFechaBaja { get; set; }
    }
}
