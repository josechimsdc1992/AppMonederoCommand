namespace AppMonederoCommand.Business.JWTService
{
    public class BusJwToken : IBusJwToken
    {
        private readonly ILogger<BusJwToken> _logger;
        //JWT
        private readonly IConfiguration _configuration;
        private readonly IDatHistorialRefreshToken _datHistorialRefreshToken;
        private readonly IBusParametros _busParametros;
        public BusJwToken(ILogger<BusJwToken> logger, IConfiguration configuration, IDatHistorialRefreshToken datHistorialRefreshToken, IBusParametros busParametros)
        {
            this._logger = logger;

            this._configuration = configuration;
            this._datHistorialRefreshToken = datHistorialRefreshToken;
            this._busParametros = busParametros;
        }

        #region  Métodos Service JWT

        public async Task<IMDResponse<dynamic>> BDevolverRefreshToken(EntRefreshTokenRequest pRefreshTokenRequest, Guid pUIdUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BDevolverRefreshToken);
            _logger.LogInformation(IMDSerializer.Serialize(67823462128345, $"Inicia {metodo}(EntRefreshTokenRequest pRefreshTokenRequest, Guid pUIdUsuario)", pRefreshTokenRequest, pUIdUsuario));

            try
            {
                var refreshTokenEncontrado = _datHistorialRefreshToken.DGet(pRefreshTokenRequest, pUIdUsuario).Result;
                if (refreshTokenEncontrado.HasError == true)
                {
                    response = new IMDResponse<dynamic>
                    {
                        Result = refreshTokenEncontrado.Result,
                        HttpCode = refreshTokenEncontrado.HttpCode,
                        HasError = refreshTokenEncontrado.HasError,
                        ErrorCode = refreshTokenEncontrado.ErrorCode,
                        Message = refreshTokenEncontrado.Message
                    };
                }
                else
                {
                    var refreshTokenCreado = await BGenerarRefreshToken();
                    var tokenCreado = await BGenerarTokenJWT(pUIdUsuario);
                    IMDResponse<EntHistorialRefreshToken> guardaHitorial = await BGuardaHistorialRefreshToken(pUIdUsuario, tokenCreado.Result.token, refreshTokenCreado.Result);
                    response.SetCreated(guardaHitorial.Result);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462129122;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462129122, $"Error en {metodo}(EntRefreshTokenRequest pRefreshTokenRequest, Guid pUIdUsuario): {ex.Message}", pRefreshTokenRequest, pUIdUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<dynamic>> BGenerarRefreshToken()
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BGenerarRefreshToken);
            _logger.LogInformation(IMDSerializer.Serialize(67823462129899, $"Inicia {metodo}()"));

            try
            {
                var byteArray = new byte[64];
                var refreshToken = "";
                using (var mg = RandomNumberGenerator.Create())
                {
                    mg.GetBytes(byteArray);
                    refreshToken = Convert.ToBase64String(byteArray);
                }
                response.SetSuccess(refreshToken);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462130676;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462130676, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntHistorialRefreshToken>> BGuardaHistorialRefreshToken(Guid pUIdUusario, string pSToken, string pSRefreshToken)
        {
            IMDResponse<EntHistorialRefreshToken> response = new IMDResponse<EntHistorialRefreshToken>();

            string metodo = nameof(this.BGuardaHistorialRefreshToken);
            _logger.LogInformation(IMDSerializer.Serialize(67823462131453, $"Inicia {metodo}(Guid pUIdUusario, string pSToken, string pSRefreshToken)", pUIdUusario, pSToken, pSRefreshToken));

            try
            {
                //Obtener vigencia del refresh token...
                string vigenciaRefreshToken = _busParametros.BObtener("APP_VIGENCIA_JWT_REFRESH").Result.Result.sValor;
                if (string.IsNullOrEmpty(vigenciaRefreshToken))
                {
                    vigenciaRefreshToken = "10080";
                }

                var historialRrefreshToken = new EntHistorialRefreshToken
                {
                    uIdUsuario = pUIdUusario,
                    sToken = pSToken,
                    sRefreshToken = pSRefreshToken,
                    dtFechaCreacion = DateTime.UtcNow,
                    dtFechaExpiracion = DateTime.UtcNow.AddMinutes(Convert.ToInt16(vigenciaRefreshToken)),
                    bActivo = true
                };

                var tempResponse = await _datHistorialRefreshToken.DSave(historialRrefreshToken);
                if (tempResponse.HasError != true)
                {
                    response.SetSuccess(tempResponse.Result);
                }
                else
                {
                    response = tempResponse;
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462132230;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462132230, $"Error en {metodo}(Guid pUIdUusario, string pSToken, string pSRefreshToken): {ex.Message}", pUIdUusario, pSToken, pSRefreshToken, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<dynamic>> BGenerarTokenJWT(Guid pUIdUsuario)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.BGenerarTokenJWT);
            _logger.LogInformation(IMDSerializer.Serialize(67823462064631, $"Inicia {metodo}(Guid pUIdUsuario)", pUIdUsuario));

            try
            {
                var key = _configuration.GetSection("JwtSettings:Key");
                var keyByte = Encoding.UTF8.GetBytes(key.Value);
                var claims = new ClaimsIdentity();
                var keySymmetric = new SymmetricSecurityKey(keyByte);

                claims.AddClaim(new Claim("uIdUsuario", pUIdUsuario.ToString()));

                var credencialesToken = new SigningCredentials(keySymmetric, SecurityAlgorithms.HmacSha256);

                int iMinutos = 25;
                var minutos = _busParametros.BObtener("APP_VIGENCIA_JWT").Result;
                if (!minutos.HasError)
                {
                    iMinutos = Convert.ToInt16(minutos.Result.sValor);
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(iMinutos),
                    SigningCredentials = credencialesToken
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string tokenCreado = tokenHandler.WriteToken(tokenConfig);
                string expiraToken = tokenConfig.ValidTo.ToString();

                var tokenJWT = new
                {
                    token = tokenCreado,
                    expira = expiraToken
                };

                response.SetSuccess(tokenJWT);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462065408;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462065408, $"Error en {metodo}(Guid pUIdUsuario): {ex.Message}", pUIdUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> BDesactivaRefreshToken(Guid uIdUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BDesactivaRefreshToken);
            _logger.LogInformation(IMDSerializer.Serialize(67823462781025, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var desactivarToken = await _datHistorialRefreshToken.DUpdateDesactivarToken(uIdUsuario);
                if (desactivarToken.HasError != true)
                {
                    response.SetSuccess(desactivarToken.Result, desactivarToken.Message);
                }
                else
                {
                    if (desactivarToken.HttpCode == HttpStatusCode.NotFound)
                    {
                        response = desactivarToken;
                    }
                    else
                    {
                        response.ErrorCode = desactivarToken.ErrorCode;
                        response.SetError(desactivarToken.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462781802;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462781802, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }
        #endregion
    }
}
