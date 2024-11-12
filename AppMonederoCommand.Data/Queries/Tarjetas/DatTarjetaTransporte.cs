using BusMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Data.Entities.TarjetaUsuario.TarjetaUsuario, AppMonederoCommand.Entities.Tarjetas.EntTarjetaUsuario>;
using DbMapperCreate = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Tarjetas.EntCreateTarjeta, AppMonederoCommand.Data.Entities.TarjetaUsuario.TarjetaUsuario>;

namespace AppMonederoCommand.Data.Queries.TarjetaUsuario
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 18/08/2023 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    *      1        | 19/19/2023 | César Cárdenas         | Actualización
    * ---------------------------------------------------------------------------------------
    */
    public class DatTarjetaUsuario : IDatTarjetaUsuario
    {
        protected TransporteContext _dbContext { get; }
        private readonly ILogger<DatTarjetaUsuario> _logger;
        public DatTarjetaUsuario(TransporteContext dbContext, ILogger<DatTarjetaUsuario> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IMDResponse<bool>> DSave(EntCreateTarjeta entCreateTarjeta)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            try
            {
                var tarjeta = DbMapperCreate.MapEntity(entCreateTarjeta);
                tarjeta.uIdTarjetaUsuario = Guid.NewGuid();
                var res = _dbContext.TarjetaUsuario.Add(tarjeta);
                int i = await _dbContext.SaveChangesAsync();

                if (i == 00)
                {
                    response.SetError(Menssages.DatNoSave);
                }
                else
                {
                    response.SetSuccess(true);
                }
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }
            return response;
        }

        public async Task<IMDResponse<List<EntTarjetaUsuario>>> DTarjetas(Guid uIdUsuario)
        {
            IMDResponse<List<EntTarjetaUsuario>> response = new IMDResponse<List<EntTarjetaUsuario>>();

            string metodo = nameof(this.DTarjetas);
            _logger.LogInformation(IMDSerializer.Serialize(67823462050645, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var query = await _dbContext.TarjetaUsuario
                    .Where(u => u.uIdUsuario == uIdUsuario && u.bActivo == true)
                    .AsNoTracking()
                    .ToListAsync();

                var res = BusMapper.MapList(query);
                response.SetSuccess(res);

            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462051422;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462051422, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DTarjeta(Guid uIdTarjeta)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DTarjeta);
            _logger.LogInformation(IMDSerializer.Serialize(67823462073955, $"Inicia {metodo}(Guid uIdTarjeta)", uIdTarjeta));

            try
            {
                var query = await _dbContext.TarjetaUsuario
                    .Where(u => u.uIdTarjeta == uIdTarjeta && u.bBaja != true)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (query == null)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetError(Menssages.DatRegisteredCard);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462074732;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462074732, $"Error en {metodo}(Guid uIdTarjeta): {ex.Message}", uIdTarjeta, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DDesvincularTarjeta(EntDesvincularTarjeta entDesvincularTarjeta)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DDesvincularTarjeta);
            _logger.LogInformation(IMDSerializer.Serialize(67823462077063, $"Inicia {metodo}(EntDesvincularTarjeta entDesvincularTarjeta)", entDesvincularTarjeta));

            try
            {
                var query = await _dbContext.TarjetaUsuario
                    .Where(u => u.uIdUsuario == entDesvincularTarjeta.uIdUsuario && u.uIdTarjeta == entDesvincularTarjeta.uIdTarjeta && u.bActivo == true)
                    .FirstOrDefaultAsync();

                if (query == null)
                {
                    response.SetError(Menssages.DatNoGetRegister);
                    return response;
                }

                query.bActivo = false;
                query.bBaja = true;
                query.iActivo = 0;
                _dbContext.TarjetaUsuario.Attach(query);
                int i = await _dbContext.SaveChangesAsync();

                response.Result = true;
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462077840;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462077840, $"Error en {metodo}(EntDesvincularTarjeta entDesvincularTarjeta): {ex.Message}", entDesvincularTarjeta, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463826090, 67823463825313)]
        public async Task<IMDResponse<EntTarjetaUsuario>> DGetTarjetaByID(Guid uIdUsuario, Guid uIdTarjeta)
        {
            IMDResponse<EntTarjetaUsuario> response = new IMDResponse<EntTarjetaUsuario>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdTarjeta, Guid uIdTarjeta)", uIdTarjeta, uIdTarjeta));

            try
            {
                var query = await _dbContext.TarjetaUsuario
                    .Where(u => u.uIdUsuario == uIdUsuario && u.uIdTarjeta == uIdTarjeta && u.bActivo == true)
                    .FirstOrDefaultAsync();

                if (query == null)
                {
                    response.SetError(Menssages.DatNoGetRegister);
                    return response;
                }

                response.Result = BusMapper.MapEntity(query);
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462077840;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdTarjeta, Guid uIdTarjeta): {ex.Message}", uIdTarjeta, uIdTarjeta, ex, response));
            }
            return response;
        }


        public async Task<IMDResponse<bool>> DVincularTarjeta(EntCreateTarjeta entVincularTarjeta)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            try
            {
                var tarjeta = DbMapperCreate.MapEntity(entVincularTarjeta);
                tarjeta.uIdTarjetaUsuario = Guid.NewGuid();
                var res = _dbContext.TarjetaUsuario.Add(tarjeta);
                int i = await _dbContext.SaveChangesAsync();

                if (res == null)
                {
                    response.SetError(Menssages.DatNoSave);
                }
                else
                {
                    response.SetSuccess(true);
                }
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateSaldo(Guid uIdMonedero, decimal dSaldo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            try
            {
                var query = await _dbContext.TarjetaUsuario
                    .Where(u => u.uIdMonedero == uIdMonedero && u.bActivo == true)
                    .FirstOrDefaultAsync();

                if (query == null)
                {
                    response.SetError(Menssages.DatNoGetRegister);
                    return response;
                }

                query.dSaldo = dSaldo;
                _dbContext.TarjetaUsuario.Attach(query);
                int i = await _dbContext.SaveChangesAsync();

                response.Result = true;
                return response;
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }
            return response;
        }

        [IMDMetodo(67823465282188, 67823465281411)]
        public async Task<IMDResponse<EntTarjetaUsuario>> DGetTarjetaByIdMonedero(Guid uIdMonedero)
        {
            IMDResponse<EntTarjetaUsuario> response = new IMDResponse<EntTarjetaUsuario>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMonedero)", uIdMonedero));

            try
            {
                var tarjeta = await _dbContext.TarjetaUsuario.Where(w => w.uIdMonedero == uIdMonedero).FirstOrDefaultAsync();
                if (tarjeta != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(tarjeta));
                }
                else
                {
                    response.SetError(Menssages.DatNoExistRegister);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMonedero): {ex.Message}", uIdMonedero, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465605420, 67823465604643)]
        public async Task<IMDResponse<EntTarjetaUsuario>> DGetTarjetaByNumTarjeta(string sNumTarjeta)
        {
            IMDResponse<EntTarjetaUsuario> response = new IMDResponse<EntTarjetaUsuario>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sNumTarjeta)", sNumTarjeta));

            try
            {
                var tarjeta = await _dbContext.TarjetaUsuario.Where(w => w.sNumeroTarjeta == sNumTarjeta).FirstOrDefaultAsync();
                if (tarjeta != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(tarjeta));
                }
                else
                {
                    response.SetError(Menssages.DatNoExistRegister);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sNumTarjeta): {ex.Message}", sNumTarjeta, ex, response));
            }
            return response;
        }
    }
}
