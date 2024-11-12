using Newtonsoft.Json;

namespace AppMonederoCommand.Business.TicketsTour
{
    public class BusTicketsTour : IBusTicketsTour
    {
        private readonly ILogger<BusTicketsTour> _logger;
        private readonly IServGenerico _servGenerico;
        private readonly IAuthService _authService;
        private readonly string _urlBaseTicketsTour = Environment.GetEnvironmentVariable("") ?? "";
        private readonly string _urlGetProductos = Environment.GetEnvironmentVariable("") ?? "";
        private readonly string _urlBaseCatalogos = Environment.GetEnvironmentVariable("URLBASE_PAQUETES") ?? "";
        private readonly string _urlLadas = Environment.GetEnvironmentVariable("URL_LADAS") ?? "Ladas/list";
        private readonly string _urlTipoTicketTour = Environment.GetEnvironmentVariable("URL_TIPO_TICKET_TOUR") ?? "TipoTicketsTour/list";
        private readonly string _urlPostAsignarProducto = Environment.GetEnvironmentVariable("") ?? "";
        private readonly string _urlPostObtenerQr = Environment.GetEnvironmentVariable("") ?? "";
        private readonly string _urlPostAddQr = Environment.GetEnvironmentVariable("") ?? "";

        public BusTicketsTour(ILogger<BusTicketsTour> logger, IServGenerico servGenerico, IAuthService authService)
        {
            _logger = logger;
            _servGenerico = servGenerico;
            _authService = authService;
        }

        [IMDMetodo(67823464968280, 67823464967503)]
        public async Task<IMDResponse<EntLstProductosTicketsTour>> BGetProductos()
        {
            IMDResponse<EntLstProductosTicketsTour> response = new IMDResponse<EntLstProductosTicketsTour>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));
            try
            {
                var token = await _authService.BIniciarSesion();
                var result = await _servGenerico.SGetPath(_urlBaseTicketsTour, _urlGetProductos, token.Result.sToken);

                if (result.HasError)
                {
                    response.SetError(result.Message);
                }

                else
                {
                    /*
                    if (result.Result != null && result.Result.productos != null)
                    {
                        EntLstProductosTicketsTour list = JsonConvert.DeserializeObject<EntLstProductosTicketsTour>(result.Result);

                        list.productos = list.productos.Where(i => i.app).ToList();

                        response.SetSuccess(list);
                    }
                    else
                    {
                        response.SetSuccess(new(), Menssages.BusNoRegisters);
                    }
                    */
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(67823464969834, 67823464969057)]
        public async Task<IMDResponse<EntPagination<EntProductosTicketsTour>>> BGetOrderProductos(EntRequestFilterTourTikets entFilters)
        {
            IMDResponse<EntPagination<EntProductosTicketsTour>> response = new();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntRequestFilterTourTikets entFilters)"));
            try
            {
                IMDResponse<EntLstProductosTicketsTour> list = await BGetProductos();

                if (list.HasError || list.Result.productos == null)
                {
                    response.SetSuccess(new EntPagination<EntProductosTicketsTour>(entFilters.NumPag, entFilters.NumReg, 0), Menssages.BusNoRegisters);
                    return response;
                }

                var tempList = list.Result.productos.AsQueryable();

                if (entFilters.bPrecio)
                {
                    tempList = tempList.OrderByDescending(i => i.precioUnitario);
                }
                else
                {
                    tempList = tempList.OrderBy(i => i.precioUnitario);
                }

                var paginator = new EntPagination<EntProductosTicketsTour>(entFilters.NumPag, entFilters.NumReg, list.Result.productos.Count());
                paginator.Datos = IMDPagination.ApplyPagination(tempList, entFilters.NumPag, entFilters.NumReg);
                response.SetSuccess(paginator);

            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntRequestFilterTourTikets entFilters): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(67823464971388, 67823464970611)]
        public async Task<IMDResponse<bool>> BPostAsiganrProducto(EntRequestAsignarProducto pEntity)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));
            try
            {
                var token = await _authService.BIniciarSesion();
                var result = await _servGenerico.SPostBody(_urlBaseTicketsTour, _urlPostAsignarProducto, pEntity, token.Result.sToken);

                if (result.HasError)
                {
                    response.SetError(result.Message);
                }

                else
                {
                    if (result.HasError && result.Result != null)
                    {
                        response.SetSuccess(true);
                    }

                    else
                    {
                        response.SetError(result.Message ?? Menssages.DatErrorGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(67823464972942, 67823464972165)]
        public async Task<IMDResponse<bool>> BPostValidarCupon(EntRequestValidarCupon pEntity)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));
            try
            {
                var token = await _authService.BIniciarSesion();
                var result = await _servGenerico.SPostBody(_urlBaseTicketsTour, _urlPostAsignarProducto, pEntity, token.Result.sToken);

                if (result.HasError)
                {
                    response.SetError(result.Message);
                }

                else
                {
                    if (result.HasError && result.Result != null)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(result.Message ?? Menssages.DatErrorGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(67823464974496, 67823464973719)]
        public async Task<IMDResponse<EntResponseGetQr>> BObtenerQR(object pEntity)
        {
            IMDResponse<EntResponseGetQr> response = new IMDResponse<EntResponseGetQr>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));
            try
            {
                var token = await _authService.BIniciarSesion();
                var result = await _servGenerico.SPostBody(_urlBaseTicketsTour, _urlPostObtenerQr, pEntity, token.Result.sToken);

                if (result.HasError)
                {
                    response.SetError(result.Message);
                }
                else
                {
                    if (result.HasError && result.Result != null)
                    {
                        response.SetSuccess(new());
                    }

                    else
                    {
                        response.SetError(result.Message ?? Menssages.DatErrorGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(67823464976050, 67823464975273)]
        public async Task<IMDResponse<bool>> BAddQR(object pEntity)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));
            try
            {
                var token = await _authService.BIniciarSesion();
                var result = await _servGenerico.SGetPath(_urlBaseCatalogos, _urlLadas, null, token.Result.sToken);

                if (result.HasError)
                {
                    response.SetError(result.Message);
                }
                else
                {
                    if (result.HasError && result.Result != null)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(result.Message ?? Menssages.DatErrorGeneric);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465361442, 67823465360665)]
        public async Task<IMDResponse<List<EntResponLadas>>> BGetLadas()
        {
            IMDResponse<List<EntResponLadas>> response = new IMDResponse<List<EntResponLadas>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));
            try
            {
                var token = await _authService.BIniciarSesion();
                IMDResponse<object> result = await _servGenerico.SGetPath(_urlBaseCatalogos, _urlLadas, token.Result.sToken);

                if (result.HasError)
                {
                    response.SetError(result.Message);
                }
                else
                {
                    List<EntResponLadas> list = JsonConvert.DeserializeObject<List<EntResponLadas>>(result.Result.ToString()) ?? new();

                    response.SetSuccess(list);

                    if (list.Count <= 0)
                    {
                        response.SetSuccess(new(), Menssages.BusNoRegisters);
                    }
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465362996, 67823465362219)]
        public async Task<IMDResponse<List<EntResponTiposTicketsTour>>> BGetTiposTicketsTour()
        {
            IMDResponse<List<EntResponTiposTicketsTour>> response = new IMDResponse<List<EntResponTiposTicketsTour>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}()"));
            try
            {
                var token = await _authService.BIniciarSesion();
                IMDResponse<object> result = await _servGenerico.SGetPath(_urlBaseCatalogos, _urlTipoTicketTour, token.Result.sToken);

                if (result.HasError)
                {
                    response.SetError(result.Message);
                }
                else
                {
                    List<EntResponTiposTicketsTour> list = JsonConvert.DeserializeObject<List<EntResponTiposTicketsTour>>(result.Result.ToString()) ?? new();

                    response.SetSuccess(list);

                    if (list.Count <= 0)
                    {
                        response.SetSuccess(new(), Menssages.BusNoRegisters);
                    }
                }

            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }
    }
}
