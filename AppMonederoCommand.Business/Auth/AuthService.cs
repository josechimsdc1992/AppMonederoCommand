using AppMonederoCommand.Business.Clases;
using AppMonederoCommand.Entities.Auth;
using AppMonederoCommand.Entities.Config;
using AppMonederoCommand.Entities.Sender;
using IMD.Utils.ImasD;
using Newtonsoft.Json;

namespace AppMonederoCommand.Business;

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
public class AuthService : IAuthService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
    private readonly ILogger<AuthService> _logger;
    private readonly IServGenerico _servGenerico;
    DataStorage _storage;
    private readonly IMDServiceConfig _iMDServiceConfig;
    private readonly IMDEnvironmentConfig _iMDEnvironmentConfig;

    public AuthService(ILogger<AuthService> logger, IServGenerico servGenerico, DataStorage storage,IMDServiceConfig iMDServiceConfig, IMDEnvironmentConfig iMDEnvironmentConfig)
    {
        _logger = logger;
        _servGenerico = servGenerico;
        _storage = storage;
        _iMDServiceConfig = iMDServiceConfig;
        _iMDEnvironmentConfig = iMDEnvironmentConfig;
    }

    public async Task<IMDResponse<EntKongLoginResponse>> BIniciarSesion()
    {
        IMDResponse<EntKongLoginResponse> response = new IMDResponse<EntKongLoginResponse>();
        IMDResponse<EntKongLoginResponse> usuarioResponse = new IMDResponse<EntKongLoginResponse>();
        string metodo = nameof(this.BIniciarSesion);
        _logger.LogInformation(IMDSerializer.Serialize(67823462019565, $"Inicia {metodo}()"));
        try
        {
            /*
            dynamic usuario = new ExpandoObject();
            usuario.sUserName = IMDSecurity.BDecrypt(_userApp, PCKEY, PCIV);
            usuario.sPassword = IMDSecurity.BDecrypt(_passwordApp, PCKEY, PCIV);
            usuarioInfo = usuario;
            usuarioResponse = await _servGenerico.SPostBody(_URLBase, _endPointAuth, usuarioInfo, null);
            if (usuarioResponse.HasError)
            {
                return response.GetResponse(usuarioResponse);
            }
            var obj = System.Text.Json.JsonSerializer.Deserialize<EntKongLoginResponse>(usuarioResponse.Result.ToString())!;
            */
            usuarioResponse = await BIniciarSesionV2();
            response.SetSuccess(usuarioResponse.Result, Menssages.BusCompleteCorrect);
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823462020342;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823462020342, $"Error en {metodo}(): {ex.Message}", usuarioResponse, ex, response));
        }
        return response;
    }

    public async Task<IMDResponse<EntKongLoginResponse>> BIniciarSesionV2()
    {
        IMDResponse<EntKongLoginResponse> response = new IMDResponse<EntKongLoginResponse>();

        IMDResponse<object> usuarioResponse = new IMDResponse<object>();
        var usuarioInfo = new object();

        string metodo = nameof(this.BIniciarSesionV2);
        _logger.LogInformation(IMDSerializer.Serialize(67823462019565, $"Inicia {metodo}(EntLogin pUsuario)"));
        try
        {
            EntDataStorage entDataStorage = _storage.Get<EntDataStorage>(nameof(EntDataStorage));
            EntDataStorage entNewDataStorage = new EntDataStorage();

            bool bGeneraConsulta = false;
            //Si existe los datos y el token
            if (entDataStorage != null && !string.IsNullOrEmpty(entDataStorage.sToken))
            {
                //Se valida la fecha de expiracion que aun sea valida
                if (entDataStorage.dtExpiresToken != DateTime.MinValue && entDataStorage.dtExpiresToken > DateTime.Now)
                {
                    response.Result = new EntKongLoginResponse();
                    response.Result.sToken = entDataStorage.sToken;
                    response.Result.sRefreshToken = entDataStorage.sRefreshToken;
                    response.Result.dtExpiresToken = entDataStorage.dtExpiresToken;
                    response.Result.dtRefreshTokenExpiryTime = entDataStorage.dtRefreshTokenExpiryTime;
                }
                //Si ya no es valida
                else
                    bGeneraConsulta = true;
            }
            //Si ya no existe el token
            else
                bGeneraConsulta = true;

            if (bGeneraConsulta)
            {
                _logger.LogInformation($"[NOTVALID]Solicitud nueva para actualizacion token y actualización");
                dynamic usuario = new ExpandoObject();
                usuario.sUserName = _iMDServiceConfig.Seguridad_UserName;
                usuario.sPassword = _iMDServiceConfig.Seguridad_Password;

                usuarioInfo = usuario;
                usuarioResponse = await _servGenerico.SPostBody(_iMDServiceConfig.Seguridad_Host, _iMDServiceConfig.Seguridad_Login, usuarioInfo, null);

                if (usuarioResponse.HasError)
                {
                    return response.GetResponse(usuarioResponse);
                }

                EntKongLoginResponse result = System.Text.Json.JsonSerializer.Deserialize<EntKongLoginResponse>(usuarioResponse.Result.ToString())!;
                response.SetSuccess(result, Menssages.BusCompleteCorrect);
                //Se asinga las variables al data
                entNewDataStorage.sToken = result.sToken;
                entNewDataStorage.sRefreshToken = result.sRefreshToken;
                entNewDataStorage.dtExpiresToken = result.dtExpiresToken;
                entNewDataStorage.dtRefreshTokenExpiryTime = result.dtRefreshTokenExpiryTime;

                _storage.AddData(nameof(EntDataStorage), entNewDataStorage);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823462020342;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823462020342, $"Error en {metodo}(EntLogin pUsuario): {ex.Message}", usuarioInfo, usuarioResponse, ex, response));
        }
        return response;
    }
}
