namespace AppMonederoCommand.Business.Repositories.TipoTarifa
{
    public interface IDatTipoTarifa
    {
        Task<IMDResponse<EntReplicaTipoTarifas>> DSave(EntReplicaTipoTarifas newItem);
        Task<IMDResponse<bool>> DUpdate(EntReplicaTipoTarifas entity);
        Task<IMDResponse<bool>> DDelete(Guid iKey);
        Task<IMDResponse<EntReplicaTipoTarifas>> DObtenerTipoTarifa(Guid uIdTipoTarifa);
        Task<IMDResponse<List<EntReplicaTipoTarifas>>> DGetAll();
    }
}
