namespace AppMonederoCommand.Business.Interfaces.Usuarios
{
    public interface IBusUsuariosWeb
    {
        Task<IMDResponse<bool>> BUpdateUsuarioByMonedero(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero);
    }
}
