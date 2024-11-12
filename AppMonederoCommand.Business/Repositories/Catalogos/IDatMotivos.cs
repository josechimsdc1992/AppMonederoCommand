namespace AppMonederoCommand.Business.Repositories.Catalogos
{
    public interface IDatMotivos
    {
        Task<IMDResponse<bool>> DAgregar(EntMotivo entMotivo);
        Task<IMDResponse<bool>> DActualizar(EntMotivo entMotivo);
        Task<IMDResponse<bool>> DEliminar(Guid uIdMotivo);
        Task<IMDResponse<EntMotivo>> DObtenerMotivo(Guid uIdMotivo);
    }
}
