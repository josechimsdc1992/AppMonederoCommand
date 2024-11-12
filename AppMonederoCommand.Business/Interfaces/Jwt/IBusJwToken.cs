namespace AppMonederoCommand.Business.Interfaces.Jwt
{
    public interface IBusJwToken
    {
        Task<IMDResponse<dynamic>> BGenerarTokenJWT(Guid pUIdUsuario);
        Task<IMDResponse<dynamic>> BDevolverRefreshToken(EntRefreshTokenRequest pRefreshTokenRequest, Guid pUIdUsuario);
        Task<IMDResponse<dynamic>> BGenerarRefreshToken();
        Task<IMDResponse<EntHistorialRefreshToken>> BGuardaHistorialRefreshToken(Guid pUIdUusario, string pSToken, string pSRefreshToken);
        Task<IMDResponse<bool>> BDesactivaRefreshToken(Guid uIdUsuario);
    }
}
