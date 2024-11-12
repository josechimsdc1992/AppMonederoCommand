using DbMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Usuarios.JWTEntities.EntHistorialRefreshToken, AppMonederoCommand.Data.Entities.Jwt.HistorialRefreshToken>;

namespace AppMonederoCommand.Data.Queries.Jwt
{
    /* IMASD S.A.DE C.V
  =========================================================================================
  * Descripción: 
  * Historial de cambios:
  * ---------------------------------------------------------------------------------------
  *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
  * ---------------------------------------------------------------------------------------
  *      1        | 04/08/2023 | L.I. Oscar Luna   | Creación
  * ---------------------------------------------------------------------------------------
  */
    public class DatHistorialRefreshToken : IDatHistorialRefreshToken
    {
        protected TransporteContext _dbContext { get; }
        private readonly ILogger<DatHistorialRefreshToken> _logger;

        public DatHistorialRefreshToken(TransporteContext dbContext, ILogger<DatHistorialRefreshToken> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #region Métodos Service DatHistorialRefreshToken
        public async Task<IMDResponse<bool>> DGet(EntRefreshTokenRequest pRefreshTokenRequest, Guid idUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DGet);
            _logger.LogInformation(IMDSerializer.Serialize(67823462133007, $"Inicia {metodo}(EntRefreshTokenRequest pRefreshTokenRequest, Guid idUsuario)", pRefreshTokenRequest, idUsuario));

            try
            {
                DateTime refresTokenValido = DateTime.UtcNow;
                var queryToken = await _dbContext.HistorialRefreshToken.FirstOrDefaultAsync(u =>
                u.uIdUsuario == idUsuario &&
                u.sToken == pRefreshTokenRequest.sTokenExpirado &&
                u.sRefreshToken == pRefreshTokenRequest.sRefreshToken &&
                refresTokenValido < u.dtFechaExpiracion &&
                u.bActivo == true
                );

                if (queryToken != null)
                {
                    queryToken.bActivo = false;

                    int i = await _dbContext.SaveChangesAsync();

                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatTokenInactive);
                        response.HttpCode = HttpStatusCode.Unauthorized;
                        //Termina update
                    }
                }
                else
                {
                    response.SetError(Menssages.DatTokenNoValids);
                    response.HttpCode = HttpStatusCode.Unauthorized;
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462133784;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462133784, $"Error en {metodo}(EntRefreshTokenRequest pRefreshTokenRequest, Guid idUsuario): {ex.Message}", pRefreshTokenRequest, idUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntHistorialRefreshToken>> DSave(EntHistorialRefreshToken newItem)
        {
            IMDResponse<EntHistorialRefreshToken> response = new IMDResponse<EntHistorialRefreshToken>();

            string metodo = nameof(this.DSave);
            _logger.LogInformation(IMDSerializer.Serialize(67823462134561, $"Inicia {metodo}(EntHistorialRefreshToken newItem)", newItem));

            try
            {
                var entToken = DbMapper.MapEntity(newItem);
                entToken.uIdHistorialToken = Guid.NewGuid();


                _dbContext.HistorialRefreshToken.Add(entToken);

                int i = await _dbContext.SaveChangesAsync();

                if (i == 0)
                {

                    response.SetError(Menssages.DatNoHistoryRefresToken);
                }
                else
                {
                    newItem.uIdHistorialToken = entToken.uIdHistorialToken;
                    response.SetSuccess(newItem);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462135338;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462135338, $"Error en {metodo}(EntHistorialRefreshToken newItem): {ex.Message}", newItem, ex, response));
            }
            return response;
        }


        public async Task<IMDResponse<bool>> DUpdateDesactivarToken(Guid uIdUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateDesactivarToken);
            _logger.LogInformation(IMDSerializer.Serialize(67823462782579, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {

                var queryExisteActivo = await _dbContext.HistorialRefreshToken.Where(u => u.uIdUsuario == uIdUsuario && u.bActivo == true)
                                        .ExecuteUpdateAsync(x => x.SetProperty(y => y.bActivo, false).SetProperty(y => y.dtFechaModificacion, DateTime.UtcNow));
                if (queryExisteActivo > 0)
                {
                    response.SetSuccess(true, Menssages.DatDesactivateTokens);

                }
                else
                {
                    response.SetNotFound(false, Menssages.DatNoRefreshTokens);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462783356;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462783356, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }


        #endregion
    }
}
