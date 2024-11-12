namespace AppMonederoCommand.Business.Interfaces.Tarifa
{
    public interface IBusTipoTarifa
    {
        Task<IMDResponse<EntReplicaTipoTarifas>> BObtenerTipoTarifa(Guid uIdTipoTarifa);
        Task<IMDResponse<List<EntReplicaTipoTarifas>>> BGetAll();
    }
}
