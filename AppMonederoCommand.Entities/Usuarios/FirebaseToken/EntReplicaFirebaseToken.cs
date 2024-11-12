namespace AppMonederoCommand.Entities.Usuarios.FirebaseToken
{
    public class EntReplicaFirebaseToken
    {
        public Guid uIdFirebaseToken { get; set; }
        public Guid uIdUsuario { get; set; }
        public string sFcmToken { get; set; }
        public string? sNombre { get; set; }
        public string? sApellidoPaterno { get; set; }
        public string? sApellidoMaterno { get; set; }
        public string? sTelefono { get; set; }
        public string? sCorreo { get; set; }
        public string? sIdAplicacion { get; set; }
        public DateTime dtFechaCreacion { get; set; }
        public DateTime? dtFechaModificacion { get; set; }
        public Guid? uIdMonedero { get; set; }
    }
}
