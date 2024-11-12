namespace AppMonederoCommand.Data.Queries.Monedero
{
    using DbMapper = IMDAutoMapper<EntTransferirSaldo, Entities.Monedero.Movimiento>;
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 18/08/2009 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public class DatMonedero : IDatMonedero
    {
        protected TransporteContext _dbContext { get; }
        private readonly ILogger<DatMonedero> _logger;
        private readonly IMapper _mapper;

        public DatMonedero(TransporteContext dbContext, ILogger<DatMonedero> logger,IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IMDResponse<decimal>> DSaldo(Guid uIdMonedero)
        {
            IMDResponse<decimal> response = new IMDResponse<decimal>();

            string metodo = nameof(this.DSaldo);
            _logger.LogInformation(IMDSerializer.Serialize(67823462178073, $"Inicia {metodo}(Guid uIdMonedero)", uIdMonedero));

            try
            {

            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462178850;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462178850, $"Error en {metodo}(Guid uIdMonedero): {ex.Message}", uIdMonedero, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<List<EntMovimientos>>> DConsultarMovimientos(EntBusquedaMovimientos filtros, Guid uIdMonedero)
        {
            IMDResponse<List<EntMovimientos>> response = new IMDResponse<List<EntMovimientos>>();

            string metodo = nameof(this.DConsultarMovimientos);
            _logger.LogInformation(IMDSerializer.Serialize(67823462179627, $"Inicia {metodo}(EntBusquedaMovimientos filtros, Guid uIdMonedero)", filtros, uIdMonedero));

            try
            {

            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462180404;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462180404, $"Error en {metodo}(EntBusquedaMovimientos filtros, Guid uIdMonedero): {ex.Message}", filtros, uIdMonedero, ex, response));
            }
            return response;
        }


        public async Task<IMDResponse<decimal>> DSaveMovimiento(EntTransferirSaldo item)
        {
            var response = new IMDResponse<decimal>();
            try
            {
                var movimiento = DbMapper.MapEntity(item);
                movimiento.uId = Guid.NewGuid();

                if (item.uIdTipoMovimiento == Guid.Empty)
                {
                    movimiento.dSaldoActual = 0;
                    movimiento.dSaldoAnterior = 0;
                }

                int i = await _dbContext.SaveChangesAsync();

                if (i == 0)
                {
                    response.SetError("Datos no guardados");
                }
                else
                {
                    response.SetSuccess(movimiento.dSaldoActual);
                }
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }

            return response;
        }

        public async Task<IMDResponse<EntMonedero>> DMonedero(Guid uIdMonedero)
        {
            IMDResponse<EntMonedero> response = new IMDResponse<EntMonedero>();

            string metodo = nameof(this.DMonedero);
            _logger.LogInformation(IMDSerializer.Serialize(67823462075509, $"Inicia {metodo}(Guid uIdMonedero)", uIdMonedero));

            try
            {

            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462076286;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462076286, $"Error en {metodo}(Guid uIdMonedero): {ex.Message}", uIdMonedero, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465937199, 67823465937976)]
        public async Task<IMDResponse<bool>> DMonederoCreacion(EntCreateReplicaMonederos entMonederoNotificacion)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));

            try
            {
                EstadoDeCuenta mapEstadoDeCuenta = _mapper.Map<EstadoDeCuenta>(entMonederoNotificacion);
                _dbContext.EstadoDeCuenta.Add(mapEstadoDeCuenta);
                int i = await _dbContext.SaveChangesAsync();
                if (i == 0)
                {
                    response.SetError(Menssages.DatNoAddInfo);
                }
                else
                {
                    response.SetSuccess(true, Menssages.DatAddSuccessInfo);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entMonederoNotificacion, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465938753, 67823465939530)]
        public async Task<IMDResponse<EntInfoMonedero>> DConsultarMonedero(Guid uIdMonedero)
        {
            IMDResponse<EntInfoMonedero> response = new IMDResponse<EntInfoMonedero>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                var infoMonedero = await _dbContext.EstadoDeCuenta.Where(w => w.uIdMonedero == uIdMonedero).FirstOrDefaultAsync();

                if (infoMonedero != null)
                {
                    EntInfoMonedero entInfoMonedero = new EntInfoMonedero
                    {

                        IdEstadoDeCuenta = infoMonedero.uIdEstadoDeCuenta,
                        IdMonedero =infoMonedero.uIdMonedero,
                        NumeroMonedero = infoMonedero.iNumeroMonedero.ToString(),
                        IdTipoTarifa = infoMonedero.uIdTipoTarifa,
                        TipoTarifa = infoMonedero.sTipoTarifa,
                        Saldo = infoMonedero.dSaldo,
                        IdEstatusMonedero = infoMonedero.uIdEstatus,
                        Estatus = infoMonedero.sEstatus,
                        Telefono = infoMonedero.sTelefono,
                        Activo = infoMonedero.bActivo,
                        Baja = infoMonedero.bBaja,
                        FechaVigencia = infoMonedero.sFechaVigencia,
                        FechaCreacion = infoMonedero.dtFechaCreacion,
                        FechaUltimoAbono = infoMonedero.dtFechaUltimoAbono,
                        FechaUltimaOperacion = infoMonedero.dtFechaUltimaOperacion,
                        Nombre = infoMonedero.sNombre,
                        ApellidoPaterno = infoMonedero.sApellidoPaterno,
                        ApellidoMaterno = infoMonedero.sApellidoMaterno,
                        Correo = infoMonedero.sCorreo,
                        uIdMotivo = infoMonedero.uIdMotivo,
                    };

                    response.SetSuccess(entInfoMonedero, Menssages.DatGetMonederoSuccess);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatMonederoNotFound);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdMonedero, ex, response));
            }
            return response;
        }

        [IMDMetodo(0, 0)]
        public async Task<IMDResponse<EntEstadoDeCuenta>> DGetByIdMonedero(Guid iKey)
        {
            IMDResponse<EntEstadoDeCuenta> response = new();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})"));

            try
            {
                var result = await _dbContext.EstadoDeCuenta.AsNoTracking().FirstOrDefaultAsync(i => i.uIdMonedero == iKey);
                var mapEstadoDeCuenta = _mapper.Map<EntEstadoDeCuenta>(result);

                response.SetSuccess(mapEstadoDeCuenta, "consultado correctamente");
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(0, 0)]
        public async Task<IMDResponse<EntEstadoDeCuenta>> DGetByNumMonedero(long entity)
        {
            IMDResponse<EntEstadoDeCuenta> response = new();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})"));

            try
            {
                var result = await _dbContext.EstadoDeCuenta.AsNoTracking().FirstOrDefaultAsync(i => i.iNumeroMonedero == entity);
                var mapEstadoDeCuenta = _mapper.Map<EntEstadoDeCuenta>(result);
                response.SetSuccess(mapEstadoDeCuenta, "consultado correctamente");
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(0, 0)]
        public async Task<IMDResponse<bool>> DUpdate(List<EntEstadoDeCuenta> entity)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})"));

            try
            {
                await BulkOperations.DesconectarEntidadesAsync(_dbContext);

                var success = await _dbContext.ActualizarEnLotes(entity);

                _logger.LogInformation($"Updatelist: {success.ToString()},   {entity}");
                if (success)
                {
                    _logger.LogInformation($"success");
                    response.SetSuccess(true, "Actualizado Correctamente");
                }

                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError("No se pudo actualizar el registro");
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): No se pudo actualizar el registro", entity, response));
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(0, 0)]
        public async Task<IMDResponse<bool>> DUpdate(EntEstadoDeCuenta entity)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})"));

            try
            {
                await BulkOperations.DesconectarEntidadesAsync(_dbContext);
                var mapEstadoDeCuenta = _mapper.Map<EstadoDeCuenta>(entity);

                _dbContext.Attach(mapEstadoDeCuenta);

                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.uIdTipoTarifa).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.sTipoTarifa).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.dSaldo).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.uIdEstatus).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.sEstatus).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.dtFechaUltimaOperacion).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.dtFechaUltimoAbono).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.uIdMotivo).IsModified = true;
                _dbContext.Entry(mapEstadoDeCuenta).Property(i => i.uIdUltimaOperacion).IsModified = true;
                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(true, "Actualizado Correctamente");
                }

                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError("No se pudo actualizar el registro");
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): No se pudo actualizar el registro", entity, response));

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", ex, response));
            }
            return response;
        }
    }
}