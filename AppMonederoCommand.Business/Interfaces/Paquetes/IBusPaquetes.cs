namespace AppMonederoCommand.Business.Interfaces;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 03/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public interface IBusPaquetes
{
    Task<IMDResponse<dynamic>> BGetAll(string Token);
}
