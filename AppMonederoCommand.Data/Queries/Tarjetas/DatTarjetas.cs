using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Tarjetas.EntReadTarjetas, AppMonederoCommand.Data.Entities.Tarjeta.EntTarjetas>;
using BusMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Data.Entities.Tarjeta.EntTarjetas, AppMonederoCommand.Entities.Tarjetas.EntReadTarjetas>;
using Microsoft.EntityFrameworkCore;

namespace AppMonederoCommand.Data.Queries.Tarjetas
{
    public class DatTarjetas : IDatTarjetas
    {
        protected TransporteContext _DbContext { get; }
        private readonly ILogger<DatTarjetas> _logger;
        private readonly IMapper _mapper;
        public DatTarjetas(TransporteContext dbContext, ILogger<DatTarjetas> logger, IMapper mapper)
        {
            _DbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }
        public Task<IMDResponse<bool>> DDelete(Guid iKey)
        {
            throw new NotImplementedException();
        }

        public Task<IMDResponse<List<EntReadTarjetas>>> DGet()
        {
            throw new NotImplementedException();
        }

        public async Task<IMDResponse<EntReadTarjetas>> DGet(Guid iKey)
        {
            IMDResponse<EntReadTarjetas> response = new IMDResponse<EntReadTarjetas>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})", iKey));

            try
            {
                var result = await _DbContext.Tarjetas.AsNoTracking().FirstOrDefaultAsync(i => i.uIdTarjeta == iKey && i.bActivo == true);

                if (result == null)
                {
                    response.SetSuccess(new EntReadTarjetas(), "No se encontraron resultados");
                }
                else
                {
                    var mapEntity = BusMapper.MapEntity(result);
                    response.SetSuccess(mapEntity, "Consultado Correctamente");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", iKey, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntReadTarjetas>> DSave(EntReadTarjetas newItem)
        {
            IMDResponse<EntReadTarjetas> response = new IMDResponse<EntReadTarjetas>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})"));

            try
            {
                var entity = DbMapper.MapEntity(newItem);
                entity.bActivo = true;
                entity.dtFechaCreacion = DateTime.Now;

                _DbContext.Tarjetas.Add(entity);
                var exec = await _DbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(newItem, "Agregado satisfactoriamente");
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError("No se pudo agregar el registro");
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}({metodo.sParametros}): No se pudo agregar el registro", newItem, response));
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

        public Task<IMDResponse<List<EntReadTarjetas>>> DSave(List<EntReadTarjetas> rangeItems)
        {
            throw new NotImplementedException();
        }

        public Task<IMDResponse<bool>> DUpdate(EntReadTarjetas entity)
        {
            throw new NotImplementedException();
        }

        [IMDMetodo(67823464281412, 67823464280635)]
        public async Task<IMDResponse<EntReadTarjetas>> DGetByNumTarjeta(long plTarjeta)
        {
            IMDResponse<EntReadTarjetas> response = new();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})", plTarjeta));

            try
            {
                //await BulkOperations.DesconectarEntidadesAsync(_DbContext);

                var query = _DbContext.Tarjetas.Include(x=>x.entTipoTarifa).Include(x=>x.entMotivos).AsNoTracking().FirstOrDefault(u => u.iNumeroTarjeta == plTarjeta);
                var result = _mapper.Map<EntReadTarjetas>(query);
                response.SetSuccess(result);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError("Ocurrió un error inesperado");
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", plTarjeta, ex, response));
            }

            return response;
        }

        public async Task<IMDResponse<EntReadTarjetas>> DGetByuIdMonedero(Guid uidMonedero)
        {
            IMDResponse<EntReadTarjetas> response = new();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}({metodo.sParametros})", uidMonedero));

            try
            {
                await BulkOperations.DesconectarEntidadesAsync(_DbContext);

                var query = _DbContext.Tarjetas.AsNoTracking().FirstOrDefault(u => u.uIdMonedero == uidMonedero);
                var result = _mapper.Map<EntReadTarjetas>(query);
                response.SetSuccess(result);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError("Ocurrió un error inesperado");
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Error en {metodo.sNombre}({metodo.sParametros}): {ex.Message}", uidMonedero, ex, response));
            }

            return response;
        }
    }
}
