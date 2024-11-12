namespace AppMonederoCommand.Business.Repositories.Monedero
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 18/08/2009 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public interface IDatMonedero
    {
        Task<IMDResponse<decimal>> DSaldo(Guid uIdMonedero);
        Task<IMDResponse<List<EntMovimientos>>> DConsultarMovimientos(EntBusquedaMovimientos filtros, Guid uIdMonedero);
        Task<IMDResponse<decimal>> DSaveMovimiento(EntTransferirSaldo item);
        Task<IMDResponse<AppMonederoCommand.Entities.Monedero.EntMonedero>> DMonedero(Guid uIdMonedero);
        Task<IMDResponse<bool>> DMonederoCreacion(EntCreateReplicaMonederos entMonederoNotificacion);
        Task<IMDResponse<EntInfoMonedero>> DConsultarMonedero(Guid uIdMonedero);
        Task<IMDResponse<EntEstadoDeCuenta>> DGetByIdMonedero(Guid iKey);
        Task<IMDResponse<EntEstadoDeCuenta>> DGetByNumMonedero(long entity);
        Task<IMDResponse<bool>> DUpdate(List<EntEstadoDeCuenta> entity);
        Task<IMDResponse<bool>> DUpdate(EntEstadoDeCuenta entity);
    }
}