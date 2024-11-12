namespace AppMonederoCommand.Business.Interfaces;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public interface IBusUbicacionFavorita
{
    Task<IMDResponse<dynamic>> BGetAll(Guid uIdUsuario, string? sIdAplicacion = null);

    Task<IMDResponse<EntUbicacionFavorita>> BCreate(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario);

    Task<IMDResponse<bool>> BUpdate(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario);

    Task<IMDResponse<bool>> BDelete(Guid iKey, Guid uIdUsuario);
}
