namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntValidaCuenta
    {
        public Guid uIdUsuario { get; set; }
        public string? sIdAplicacionBD { get; set; }
        public int iEstatusCuenta { get; set; }
        public string sIdAplicacionRequest { get; set; }
        public string? sCorreo { get; set; }
        public bool bTipoLogin { get; set; }
        public bool bRedSocialGoogle { get; set; }
    }
}
