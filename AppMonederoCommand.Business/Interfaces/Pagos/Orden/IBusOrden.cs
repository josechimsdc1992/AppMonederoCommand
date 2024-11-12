namespace AppMonederoCommand.Business.Interfaces;

public interface IBusOrden
{
    Task<IMDResponse<dynamic>> BCrearOrden(EntOrdenRequest entOrden, string token);
    Task<IMDResponse<dynamic>> BObtener(Guid uIdOrden, string token);
    Task<IMDResponse<bool>> BActualizar(EntActualizarOrden entActualizarOrden, string token);
    Task<IMDResponse<dynamic>> BCrearOrdenCodi(EntOrdenCodiRequest entOrden, string token);
    Task<IMDResponse<dynamic?>> BObtenerByReferencia(string sReferencia, string token);
    Task<IMDResponse<bool>> BActualizarForWebhooks(EntOrden entActualizarOrden, string token);
    Task<IMDResponse<EntDatosTarjeta>> BGetDatosByNumTarjeta(string sNumMonedero, string token);
    Task<IMDResponse<List<string>>> BOpcionesPago(string token);
}
