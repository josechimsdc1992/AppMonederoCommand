namespace AppMonederoCommand.Business.Repositories.Jwt
{
  /* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 04/08/2023 | L.I. Oscar Luna        | Creación
* ---------------------------------------------------------------------------------------
*/
  public interface IDatHistorialRefreshToken
  {
    Task<IMDResponse<bool>> DGet(EntRefreshTokenRequest pRefreshTokenRequest, Guid idUsuario);
    Task<IMDResponse<bool>> DUpdateDesactivarToken(Guid uIdUsuario);
    Task<IMDResponse<EntHistorialRefreshToken>> DSave(EntHistorialRefreshToken newItem);
  }
}
