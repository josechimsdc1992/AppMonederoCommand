namespace AppMonederoCommand.Business.Interfaces;

public interface IBusHistorialRecuperarCuenta
{
    Task<IMDResponse<EntHistorialRecuperarCuenta>> BGetByCorreoAndToken(string sCorreo, string sToken);
    Task<IMDResponse<EntHistorialRecuperarCuenta>> BSave(string sCorreo);
    Task<IMDResponse<bool>> BUpdate(string sCorreo, string sToken);
}
