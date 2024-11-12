namespace AppMonederoCommand.Business.Interfaces;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 08/01/2024 | Neftalí Rodríguez      | Creación
* ---------------------------------------------------------------------------------------
*/

public interface IBusBanorte
{
    Task<IMDResponse<dynamic>> BRegistrarPago(EntPagoBanorte entPagoBanorte, string token);
    Task<IMDResponse<bool>> BRegistrarPagoCancelado(EntPagoCanceladoBanorte entPagoCanceladoBanorte, string token);
}

