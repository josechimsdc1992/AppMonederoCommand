namespace AppMonederoCommand.Business.Interfaces.Catalogos
{
    public interface IBusMotivos
    {
        Task<IMDResponse<bool>> BAgregar(EntMotivo entMotivo);
        Task<IMDResponse<bool>> BActualizar(EntMotivo entMotivo);
        Task<IMDResponse<bool>> BEliminar(Guid uIdMotivo);
        Task<IMDResponse<EntMotivo>> BObtenerMotivo(Guid uIdMotivo);
        Task<IMDResponse<List<EntMotivo>>> BObtenerTodos();
    }
}
