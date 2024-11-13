namespace AppMonederoCommand.Business.Interfaces.Tarjeta;
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
public interface IBusTarjetaUsuario
{
    Task<IMDResponse<List<EntTarjetaUsuario>>> BTarjetas(Guid uIdUsuario, Guid uIdMonedero, string token, string? sIdAplicacion = null);
 
    Task<IMDResponse<EntOperacionesTarjetas>> BValidaTarjetaV2(string sNumeroTarjeta, string token, int? iTipoTarjeta = null);
    Task<IMDResponse<EntOperacionesPermitidasTarjeta>> BValidaEstatusTarjeta(EntTarjetaRes tarjeta);
    Task<IMDResponse<EntOperacionesPermitidasTarjeta>> BValidaEstatusTarjeta(string sNumeroTarjeta, string token, int? iTipoTarjeta = null);
    Task<IMDResponse<EntTarjetaUsuario>> BGetTarjetaByIdMonedero(Guid uIdMonedero);
    Task<IMDResponse<EntDatosTarjeta>> BGetDatosByNumTarjeta(string sNumeroTarjeta, string token);
    Task<IMDResponse<List<EntTarjetaUsuario>>> BGetTarjetasByUsuarioApp(Guid uIdUsuario);
    Task<IMDResponse<EntTarjetaUsuario>> BGetTarjetaByNumTarjeta(string sNumTarjeta);
}
