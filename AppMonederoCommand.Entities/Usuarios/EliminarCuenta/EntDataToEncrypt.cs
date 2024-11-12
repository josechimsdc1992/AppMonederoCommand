namespace AppMonederoCommand.Entities.Usuarios.EliminarCuenta
{
    public class EntDataToEncrypt
    {
        public Guid uIdUsuario { get; set; }
        public DateTime dtFechaVigencia { get; set; }
        public string sCode { get; set; }
    }
}
