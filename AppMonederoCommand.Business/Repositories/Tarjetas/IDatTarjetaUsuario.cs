namespace AppMonederoCommand.Business.Repositories.Tarjetas
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
    public interface IDatTarjetaUsuario
    {
        Task<IMDResponse<bool>> DSave(EntCreateTarjeta entCreateTarjeta);
        Task<IMDResponse<List<EntTarjetaUsuario>>> DTarjetas(Guid uIdUsuario);
        Task<IMDResponse<bool>> DTarjeta(Guid uIdTarjeta);
        Task<IMDResponse<bool>> DDesvincularTarjeta(EntDesvincularTarjeta entDesvincularTarjeta);
        Task<IMDResponse<bool>> DVincularTarjeta(EntCreateTarjeta entVincularTarjeta);
        Task<IMDResponse<bool>> DUpdateSaldo(Guid uIdMonedero, decimal dSaldo);
        Task<IMDResponse<EntTarjetaUsuario>> DGetTarjetaByID(Guid uIdUsuario, Guid uIdTarjeta);
        Task<IMDResponse<EntTarjetaUsuario>> DGetTarjetaByIdMonedero(Guid uIdMonedero);
        Task<IMDResponse<EntTarjetaUsuario>> DGetTarjetaByNumTarjeta(string sNumTarjeta);
    }
}