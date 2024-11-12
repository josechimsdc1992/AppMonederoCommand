namespace AppMonederoCommand.Business.Interfaces;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 06/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public interface IBusMercadoPago
{
    Task<IMDResponse<dynamic>> BRegistrarPago(EntPagoMercadoPago entPagoMercadoPago, string token);
    Task<IMDResponse<bool>> BRegistrarPagoMercadoPagoCancelado(EntPagoMercadoPagoCancelado entPagoMercadoPagoCancelado, string token);
}
