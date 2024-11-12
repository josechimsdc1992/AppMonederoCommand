namespace AppMonederoCommand.Business.Repositories.Catalogos
{
    public interface IDatTipoOperaciones
    {
        Task<IMDResponse<bool>> DAgregar(EntTipoOperaciones entTipoOperaciones);
        Task<IMDResponse<bool>> DActualizar(EntTipoOperaciones entTipoOperaciones);
        Task<IMDResponse<bool>> DEliminar(Guid uIdTipoOperacion);
        Task<IMDResponse<List<EntTipoOperaciones>>> DObtenerTipoOperaciones();
    }
}
