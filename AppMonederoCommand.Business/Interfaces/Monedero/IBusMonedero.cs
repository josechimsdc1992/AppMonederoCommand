namespace AppMonederoCommand.Business.Interfaces.Monedero;
/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 07/09/2023 | César Cárdenas         | Creación
* ---------------------------------------------------------------------------------------
*/
public interface IBusMonedero
{
    Task<IMDResponse<EntSaldo>> BGetSaldo(Guid uIdMonedero, string token, string? NumeroTarjeta, Guid? uIdUsuario, string? sIdAplicacion = null);
    Task<(IMDResponse<bool>, IMDResponse<AbonarSaldo>)> BAbonar(EntAbonar entAbonar, string token);
    Task<(IMDResponse<bool>, IMDResponse<TraspasoSaldoRequestModel>)> BTransferirSaldo(EntTransferirSaldo entTransferirSaldo, string token);
    Task<IMDResponse<dynamic>> BActualizaMonedero(EntRequestHTTPActualizaMonedero reqActualizaMonedero, string token);
    Task<IMDResponse<EntOperacionesMonedero>> BValidaMonedero(Guid uIdMonedero, string token);
    Task<IMDResponse<EntOperacionesMonedero>> BValidaMonederoV2(Guid uIdMonedero, string token, int? iTipoTarjeta = null);
    Task<IMDResponse<long>> BGeneraFolio(OperacionesMovimientosMonedero sOperacion);
    Task<IMDResponse<EntOperacionesPermitidasMonedero>> BEstatusMonedero(Guid uIdMonedero, int? iTipoTarjeta = null);
    Task<IMDResponse<EntOperacionesPermitidasMonedero>> BEstatusMonedero(EntMonederoRes monedero);
    Task<IMDResponse<EntOperacionesPermitidasTarjeta>> BValidaEstatusTarjeta(string sNumeroTarjeta, string token, int? iTipoTarjeta = null);
    Task<IMDResponse<EntMonederoRes>> BDatosMonedero(Guid uIdMonedero);
    Task<IMDResponse<List<EntOrden>>> BObtenerByListOrdenes(List<Guid>? uIdsOrdenes, string token);
    Task<IMDResponse<EntReadTipoTarifas>> BTipoTarifa(Guid uIdTipoTarifa);
    Task<IMDResponse<bool>> BMonederoCreacion(EntCreateReplicaMonederos entMonederoNotificacion);
    Task<IMDResponse<EntInfoMonedero>> BConsultarMonedero(Guid uIdMonedero);
    Task<IMDResponse<EntOperacionesPermitidasMonedero>> BEstatusMonedero(EntInfoMonedero entInfoMonedero, int? iTipoTarjeta = null);
}