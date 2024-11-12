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
public interface IBusPayPal
{
    Task<IMDResponse<dynamic>> BRegistrarPago(EntPagoPayPal entPagoPayPal, string token);
}