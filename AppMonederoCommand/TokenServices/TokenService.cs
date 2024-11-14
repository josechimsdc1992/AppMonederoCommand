using AppMonederoCommand.Business.Clases;
using AppMonederoCommand.Entities.Auth;
using AppMonederoCommand.Entities.Config;
using AppMonederoCommand.Entities.Sender;
using Azure;
using Azure.Core;
using IMD.Utils.ImasD;
using IMD.Utils.RabbitMQ;
using System.Dynamic;
using System.Net.Http;

namespace AppMonederoCommand.Api.TokenServices
{
    public class TokenService : BackgroundService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ExchangeConfig _exchangeConfig;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        DataStorage _storage;
        private readonly string ServiceName = "TokenService";
        private bool hasSubscribed = false;
        private Timer? _timer;
        private readonly IMDServiceConfig _iMDServiceConfig;
        private readonly IMDEnvironmentConfig _iMDEnvironmentConfig;

        public TokenService(ILogger<TokenService> logger, ExchangeConfig exchangeConfig, IServiceScopeFactory serviceScopeFactory, DataStorage dataStorage, IMDServiceConfig iMDServiceConfig, IMDEnvironmentConfig iMDEnvironmentConfig)
        {
            _logger = logger;
            _exchangeConfig = exchangeConfig;
            _serviceScopeFactory = serviceScopeFactory;
            _storage = dataStorage;
            _iMDServiceConfig = iMDServiceConfig;
            _iMDEnvironmentConfig = iMDEnvironmentConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _ = Task.Run(() =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        if (!hasSubscribed)
                        {
                            UpdateToken();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ServiceName} : {ex.Message}");
            }
        }

        private void OnReconnect(object? state)
        {
            _logger.LogInformation($"{ServiceName}: Reconnecting to update token.");
            hasSubscribed = false;
        }

        private async void UpdateToken()
        {
            try
            {
                int iMinutosEjecucion = 5;//Es el tiempo que se le restara a la diferencia de tiempo para calcular en cuanto tiempo se tendra que volver a ejecutar
                int iMinutosValidacionToken = 10;//Es el tiempo que se le aumentara a la fecha de expiracion de token para que pueda ser menor al tiempo actual, debe ser mayor a iMinutosEjecucion
                hasSubscribed = true;
                _logger.LogInformation($"{ServiceName} is listening queues...");
                EntDataStorage entDataStorage = _storage.Get<EntDataStorage>(nameof(EntDataStorage));
                EntDataStorage entNewDataStorage = new EntDataStorage();
                bool bGeneraConsulta = false;
                //Si existe los datos y el token
                if (entDataStorage != null && !string.IsNullOrEmpty(entDataStorage.sToken))
                {
                    //Se valida la fecha de expiracion que aun sea valida
                    _logger.LogInformation("ACTUAL-DATE" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    _logger.LogInformation("TOKEN-ACTUAL-DATE" + entDataStorage.dtExpiresToken.ToString("yyyy-MM-dd HH:mm:ss"));
                    _logger.LogInformation("REFRESHTOKEN-ACTUAL-DATE" + entDataStorage.dtRefreshTokenExpiryTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (entDataStorage.dtExpiresToken != DateTime.MinValue && entDataStorage.dtExpiresToken > DateTime.Now.AddMinutes(iMinutosValidacionToken))
                    {
                        bGeneraConsulta = false;
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
                    dynamic usuario = new ExpandoObject();
                    
                    usuario.sUserName = IMDSecurity.BDecrypt(_iMDServiceConfig.Seguridad_UserName, _iMDEnvironmentConfig.PCKEY, _iMDEnvironmentConfig.PCIV);
                    usuario.sPassword = IMDSecurity.BDecrypt(_iMDServiceConfig.Seguridad_Password, _iMDEnvironmentConfig.PCKEY, _iMDEnvironmentConfig.PCIV);
                    var request = new
                    {
                        sUserName = usuario.sUserName,
                        sPassword = usuario.sPassword
                    };

                    HttpClient httpClient = IMDRestClient.CreateClient(_iMDServiceConfig.Seguridad_Host);
                    var usuarioResponse = await IMDRestClient.PostAsync<IMDResponse<EntKongLoginResponse>, object>(httpClient, _iMDServiceConfig.Seguridad_Login, request);
                    if (!usuarioResponse.HasError)
                    {

                        var entToken = usuarioResponse?.Result?.Result;
                        if(entToken != null)
                        {
                            //Se asinga las variables al data
                            entNewDataStorage.sToken = entToken.sToken;
                            entNewDataStorage.sRefreshToken = entToken.sToken;
                            entNewDataStorage.dtExpiresToken = entToken.dtExpiresToken;
                            entNewDataStorage.dtRefreshTokenExpiryTime = entToken.dtRefreshTokenExpiryTime;

                            _logger.LogInformation("ACTUAL-DATE" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            _logger.LogInformation("TOKEN-ACTUAL-DATE" + entNewDataStorage.dtExpiresToken.ToString("yyyy-MM-dd HH:mm:ss"));
                            _logger.LogInformation("REFRESHTOKEN-ACTUAL-DATE" + entNewDataStorage.dtRefreshTokenExpiryTime.ToString("yyyy-MM-dd HH:mm:ss"));

                            _storage.AddData(nameof(EntDataStorage), entNewDataStorage);
                            //hasSubscribed = false;

                            var diff = entNewDataStorage.dtExpiresToken - DateTime.Now;
                            double minutesUntilExpiry = diff.TotalMinutes - iMinutosEjecucion;
                            // Configurar el timer para que ejecute OnReconnect unos minutos antes de la expiración del token
                            _timer = new Timer(OnReconnect, null, TimeSpan.FromMinutes(minutesUntilExpiry), Timeout.InfiniteTimeSpan);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ServiceName} : {ex.Message}");
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }

    }
}
