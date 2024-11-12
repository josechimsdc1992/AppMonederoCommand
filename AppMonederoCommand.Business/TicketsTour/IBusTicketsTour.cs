namespace AppMonederoCommand.Business.TicketsTour
{
    public interface IBusTicketsTour
    {
        Task<IMDResponse<EntPagination<EntProductosTicketsTour>>> BGetOrderProductos(EntRequestFilterTourTikets entFilters);
        Task<IMDResponse<EntLstProductosTicketsTour>> BGetProductos();
        Task<IMDResponse<bool>> BPostAsiganrProducto(EntRequestAsignarProducto pEntity);
        Task<IMDResponse<bool>> BPostValidarCupon(EntRequestValidarCupon pEntity);
        Task<IMDResponse<EntResponseGetQr>> BObtenerQR(object pEntity);
        Task<IMDResponse<bool>> BAddQR(object pEntity);
        Task<IMDResponse<List<EntResponLadas>>> BGetLadas();
        Task<IMDResponse<List<EntResponTiposTicketsTour>>> BGetTiposTicketsTour();
    }
}
