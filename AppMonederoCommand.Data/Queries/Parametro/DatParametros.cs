namespace AppMonederoCommand.Data.Queries.Parametro
{
    using BusMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Data.Entities.Parametro.Parametros, AppMonederoCommand.Entities.Parametro.EntParametros>;
    using DatMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Parametro.EntParametros, AppMonederoCommand.Data.Entities.Parametro.Parametros>;
    public class DatParametros : IDatParametros
    {
        private readonly TransporteContext _dbContext;
        private readonly ILogger<DatParametros> _logger;

        public DatParametros(TransporteContext dbContext, ILogger<DatParametros> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IMDResponse<bool>> DAgregar(EntParametros entParametro)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DAgregar);
            _logger.LogInformation(IMDSerializer.Serialize(67823462192059, $"Inicia {metodo}(EntParametros entParametro)", entParametro));

            try
            {
                var newParametro = DatMapper.MapEntity(entParametro);
                _dbContext.Parametros.Add(newParametro);
                int i = await _dbContext.SaveChangesAsync();
                if (i == 0)
                {
                    response.SetError(Menssages.DatNoAddParameter);
                }
                else
                {
                    response.SetSuccess(true, Menssages.DatAddSuccesParameter);
                }
            }
            catch (Exception ex)
            {
                response.SetError("67823462192836 " + Menssages.DatErrorGeneric);
                _logger.LogError(IMDSerializer.Serialize(67823462192836, $"Error en {metodo}(EntParametros entParametro): {ex.Message}", entParametro, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<List<EntParametros>>> DObtener()
        {
            IMDResponse<List<EntParametros>> response = new IMDResponse<List<EntParametros>>();

            string metodo = nameof(this.DObtener);
            _logger.LogInformation(IMDSerializer.Serialize(67823462201383, $"Inicia {metodo}()"));

            try
            {
                var listaParametros = await _dbContext.Parametros.OrderBy(o => o.sNombre).ToListAsync();
                if (listaParametros != null)
                {
                    var listaEntParametros = BusMapper.MapList(listaParametros);
                    response.SetSuccess(listaEntParametros, Menssages.DatGetSuccesParameter);
                }
                else
                {
                    response.SetError(Menssages.DatNoExistParameters);
                }
            }
            catch (Exception ex)
            {
                response.SetError("67823462202160 " + Menssages.DatErrorGeneric);
                _logger.LogError(IMDSerializer.Serialize(67823462202160, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntParametros>> DObtener(string sNombre)
        {
            IMDResponse<EntParametros> response = new IMDResponse<EntParametros>();

            string metodo = nameof(this.DObtener);
            _logger.LogInformation(IMDSerializer.Serialize(67823462207599, $"Inicia {metodo}(string sNombre)", sNombre));

            try
            {
                var parametro = await _dbContext.Parametros.Where(w => w.sNombre.ToUpper() == sNombre.ToUpper()).FirstOrDefaultAsync();
                if (parametro != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(parametro));
                }
                else
                {
                    response.SetError(string.Format("{0} {1} {2}",
                        Menssages.DatParameterCompuest1, sNombre, Menssages.DatParameterCompuest2));
                }
            }
            catch (Exception ex)
            {
                response.SetError("67823462208376 " + Menssages.DatErrorGeneric);
                _logger.LogError(IMDSerializer.Serialize(67823462208376, $"Error en {metodo}(string sNombre): {ex.Message}", sNombre, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DActualizar(EntActualizarParametros entParametros)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DActualizar);
            _logger.LogInformation(IMDSerializer.Serialize(67823462213815, $"Inicia {metodo}(EntActualizarParametros entParametros)", entParametros));

            try
            {
                var uptParametro = await _dbContext.Parametros.FindAsync(entParametros.uIdParametro);
                if (uptParametro != null)
                {
                    _dbContext.Entry(uptParametro).CurrentValues.SetValues(entParametros);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i == 0)
                    {
                        response.SetError(Menssages.DatNoUpdateParameter);
                    }
                    else
                    {
                        response.SetSuccess(true, Menssages.DatSuccesUpdateParameter);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatNoExistUpdateParameter);
                }
            }
            catch (Exception ex)
            {
                response.SetError("67823462214592 " + Menssages.DatErrorGeneric);
                _logger.LogError(IMDSerializer.Serialize(67823462214592, $"Error en {metodo}(EntActualizarParametros entParametros): {ex.Message}", entParametros, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<List<EntParametros>>> DObtenerByClaves(List<string> claves)
        {
            IMDResponse<List<EntParametros>> response = new IMDResponse<List<EntParametros>>();

            string metodo = nameof(this.DObtener);
            _logger.LogInformation(IMDSerializer.Serialize(67823462201383, $"Inicia {metodo}()"));

            try
            {
                var listaParametros = await _dbContext.Parametros.Where(x => claves.Contains(x.sNombre) && x.bActivo).ToListAsync();
                if (listaParametros != null)
                {
                    var listaEntParametros = BusMapper.MapList(listaParametros);
                    response.SetSuccess(listaEntParametros, Menssages.DatGetSuccesParameter);
                }
                else
                {
                    response.SetError(Menssages.DatNoExistParameters);
                }
            }
            catch (Exception ex)
            {
                response.SetError("67823462202160 " + Menssages.DatErrorGeneric);
                _logger.LogError(IMDSerializer.Serialize(67823462202160, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

    }
}
