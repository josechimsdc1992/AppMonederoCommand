namespace AppMonederoCommand.Business.Interfaces;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 11/01/2024 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public interface IBusCodi
{
    Task<IMDResponse<dynamic?>> BRegistrarPago(EntPagoCodi EntPagoCodi, string token);
}
