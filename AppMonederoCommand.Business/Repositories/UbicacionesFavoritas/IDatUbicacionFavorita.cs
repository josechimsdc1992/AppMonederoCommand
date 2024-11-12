namespace AppMonederoCommand.Business.Repositories;

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
public interface IDatUbicacionFavorita
{
    Task<IMDResponse<List<EntGetAllUbicacionFavorita>>> DGetAllByIdUsuario(Guid uIdUsario);

    Task<IMDResponse<EntUbicacionFavorita>> DSave(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario);

    Task<IMDResponse<bool>> DUpdate(EntAddUbicacionFavorita ubicacionFavorita, Guid uIdUsuario);

    Task<IMDResponse<bool>> DDelete(Guid iKey, Guid uIdUsuario);
}
