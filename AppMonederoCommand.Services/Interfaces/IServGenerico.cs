namespace AppMonederoCommand.Services.Interfaces;

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
public interface IServGenerico
{
    Task<IMDResponse<object>> SGetPath(string pUrlBase, string pUrlContoller, object pParam, string? token);
    Task<IMDResponse<object>> SGetPath(string pUrlBase, string pUrl, string? token);
    Task<IMDResponse<object>> SPostBody(string pUrlBase, string pUrlContoller, object pParam, string? token, Dictionary<string, string>? headers = null);
    Task<IMDResponse<object>> SPutBody(string pUrlBase, string pUrlContoller, object pParam, string? token);
    Task<IMDResponse<object>> SDeletePath(string pUrlBase, string pUrlContoller, object pParam, string? token);
    Task<IMDResponse<object>> SPutBodyModeroC(string pUrlBase, string pUrlContoller, object pParam, string? token);
    Task<IMDResponse<object>> SGetBody(string pUrlBase, string pUrlContoller, object pParam, string? token);
}
