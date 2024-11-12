namespace AppMonederoCommand.Business.Interfaces.Usuarios
{
    public interface IBusUsuariosWeb
    {
        Task<IMDResponse<List<EntTarjetaUsuario>>> BGetListaTarjetas(Guid uIdUsuario, Guid uIdMonedero, string sToken);
        Task<IMDResponse<bool>> BUpdateUsuarioByMonedero(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero);
    }
}
