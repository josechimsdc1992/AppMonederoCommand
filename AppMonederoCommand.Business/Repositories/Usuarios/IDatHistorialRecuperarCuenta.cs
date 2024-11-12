namespace AppMonederoCommand.Business.Repositories;

public interface IDatHistorialRecuperarCuenta
{
    Task<IMDResponse<EntHistorialRecuperarCuenta>> DGetByCorreo(string sCorreo);

    Task<IMDResponse<EntHistorialRecuperarCuenta>> DGetByCorreoAndToken(string sCorreo, string sToken);

    Task<IMDResponse<EntHistorialRecuperarCuenta>> DSave(EntHistorialRecuperarCuenta newItem);

    Task<IMDResponse<bool>> DUpdate(EntHistorialRecuperarCuenta entity);

    Task<IMDResponse<bool>> DDelete(Guid iKey);

}
