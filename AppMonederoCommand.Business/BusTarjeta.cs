using AppMonederoCommand.Business.Repositories.Parametro;
using AppMonederoCommand.Entities.Parametro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Business
{
    public class BusTarjeta : IBusTarjetas
    {
        private readonly ILogger<BusTarjeta> _logger;
        private readonly IDatTarjetas _datTarjetas;

        public BusTarjeta(ILogger<BusTarjeta> logger, IDatTarjetas datTarjetas)
        {
            this._logger = logger;
            this._datTarjetas = datTarjetas;
        }
        public async Task<IMDResponse<EntReadTarjetas>> BCreate(EntReadTarjetas createModel)
        {
            IMDResponse<EntReadTarjetas> response = new IMDResponse<EntReadTarjetas>();
            string metodo = nameof(this.BCreate);
            try
            {
                EntReadTarjetas ent = createModel;
                var resp = await _datTarjetas.DSave(ent);


                if (resp.HasError)
                {
                    response.SetError("No se ha creado");
                }
                else
                {
                    ent = resp.Result;
                    response.SetCreated(ent);

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 40232026130522;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(20232026130522,
                    $"Error en {metodo}(EntCreateParametro createModel): {ex.Message}", createModel, ex, response));
            }

            return response;
        }

        public Task<IMDResponse<bool>> BDelete(Guid iKey)
        {
            throw new NotImplementedException();
        }

        public async Task<IMDResponse<EntReadTarjetas>> BGet(Guid iKey)
        {
            IMDResponse<EntReadTarjetas> response = new IMDResponse<EntReadTarjetas>();
            string metodo = nameof(this.BGet);

            try
            {
                var resData = await _datTarjetas.DGet(iKey);
                response.SetSuccess(resData.Result);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 40232026130522;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(20232026130522,
                    $"Error en {metodo}(string iKey): {ex.Message}", iKey, ex, response));
            }

            return response;
        }

        public async Task<IMDResponse<List<EntReadTarjetas>>> BGetAll()
        {
            IMDResponse<List<EntReadTarjetas>> response = new IMDResponse<List<EntReadTarjetas>>();
            string metodo = nameof(this.BGetAll);

            try
            {
                var resData = await _datTarjetas.DGet();
                response.SetSuccess(resData.Result);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 40232026130522;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(20232026130522,
                    $"Error en {metodo}(): {ex.Message}", ex, response));
            }

            return response;
        }

        public async Task<IMDResponse<EntReadTarjetas>> BGetByNumTarjeta(long plTarjeta)
        {
            IMDResponse<EntReadTarjetas> response = new IMDResponse<EntReadTarjetas>();
            string metodo = nameof(this.BGetAll);

            try
            {
                var resData = await _datTarjetas.DGetByNumTarjeta(plTarjeta);
                response.SetSuccess(resData.Result);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 40232026130522;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(20232026130522,
                    $"Error en {metodo}(): {ex.Message}", ex, response));
            }

            return response;
        }

        public async Task<IMDResponse<EntReadTarjetas>> BGetByuIdMonedero(Guid uIdMonedero)
        {
            IMDResponse<EntReadTarjetas> response = new IMDResponse<EntReadTarjetas>();
            string metodo = nameof(this.BGetAll);

            try
            {
                var resData = await _datTarjetas.DGetByuIdMonedero(uIdMonedero);
                response.SetSuccess(resData.Result);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 40232026130522;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(20232026130522,
                    $"Error en {metodo}(): {ex.Message}", ex, response));
            }

            return response;
        }

        public async Task<IMDResponse<EntReadTarjetas>> BUpdate(EntReadTarjetas updateModel)
        {
            IMDResponse<EntReadTarjetas> response = new IMDResponse<EntReadTarjetas>();
            string metodo = nameof(this.BUpdate);

            try
            {
                EntReadTarjetas entParametro = updateModel;
                var resp = await _datTarjetas.DUpdate(entParametro);


                if (resp.HasError)
                {
                    response.SetError("No se ha actualizado");
                }
                else
                {
                    response.SetSuccess(entParametro);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 40232026130522;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(20232026130522,
                    $"Error en {metodo}(EntParametro updateModel): {ex.Message}", ex, response));
            }

            return response;
        }
    }
}
