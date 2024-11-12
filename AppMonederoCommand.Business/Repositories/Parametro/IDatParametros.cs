namespace AppMonederoCommand.Business.Repositories.Parametro
{
    public interface IDatParametros
    {
        Task<IMDResponse<bool>> DAgregar(EntParametros entParametros);
        Task<IMDResponse<List<EntParametros>>> DObtener();
        Task<IMDResponse<EntParametros>> DObtener(string sNombre);
        Task<IMDResponse<bool>> DActualizar(EntActualizarParametros entParametros);
        Task<IMDResponse<List<EntParametros>>> DObtenerByClaves(List<string> claves);
    }
}
