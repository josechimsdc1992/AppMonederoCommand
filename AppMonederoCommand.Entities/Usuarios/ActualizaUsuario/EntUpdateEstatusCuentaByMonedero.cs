namespace AppMonederoCommand.Entities.Usuarios.ActualizaUsuario
{
    public class EntUpdateEstatusCuentaByMonedero
    {
        public Guid uIdUsuario { get; set; }
        public Guid? uIdMonedero { get; set; }
        public int? iEstatusCuentaApp { get; set; }
    }
}
