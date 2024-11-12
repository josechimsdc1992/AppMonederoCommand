namespace AppMonederoCommand.Business.Interfaces.Boletos
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 12/09/2023 | L.I. Oscar Luna       | Creación
    * ---------------------------------------------------------------------------------------
    */
    public interface IBusBoleto
    {
        Task<IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>>> BCreateBoletoQR(EntRequestBoletoVirtual pSolicitud);
        Task<IMDResponseQR<List<EntTicketQR>>> BListaBoleto(Guid uIdMonedero, string? sNumeroTarjeta, string? ClaveApp, Guid? uIdSolicitud);
        Task<IMDResponse<List<EntResponseHttpBoletoVirtualQR>>> BHttpGeneraBoletoVirtual(EntRequestHttpBoletoVirtual reqBoletoVirtual);
        Task<IMDResponse<dynamic>> BHttpListaBoletoVirtual(EntRequestHttpListaBoletoVirtual reqListaBoletoVirtual, eOpcionesTicket opcion);
        Task<IMDResponse<dynamic>> BSaldoMonedero(Guid uIdMonedero);
        Task<IMDResponseQR<bool>> BCancelarBoletos(Guid uIdMonedero, string ClaveApp, Guid uIdSolicitud);
        Task<IMDResponseQR<List<EntResponseHttpBoletoVirtualQR>>> BRegenerar(EntRequestBoletoVirtual entRequestBoletoVirtual);
        Task<IMDResponse<bool>> BValidaMonedero(EntInfoMonedero entInfoMonedero);
    }
}
