using Newtonsoft.Json;

namespace AppMonederoCommand.Business.Tarjetas;
/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 07/09/2023 | César Cárdenas         | Creación
* ---------------------------------------------------------------------------------------
*      2        | 19/10/2023 | César Cárdenas         | Actualización
* ---------------------------------------------------------------------------------------
*      3        | 06/12/2023 | Oscar Luna             | Actualización
* ---------------------------------------------------------------------------------------
*      4        | 11/12/2023 | Oscar Luna             | Actualización
* ---------------------------------------------------------------------------------
*/
public class BusTarjetaUsuario : IBusTarjetaUsuario
{
    private readonly ILogger<BusTarjetaUsuario> _logger;
    private readonly IDatTarjetaUsuario _datTarjetas;
    private readonly IBusMonedero _busMonedero;
    private readonly IAuthService _authService;
    private readonly IMDRabbitNotifications _rabbitNotifications;
    private readonly ExchangeConfig _exchangeConfig;
    private readonly string _url = Environment.GetEnvironmentVariable("TARJETA_URL") ?? "";
    private readonly string _urlCatalogo = Environment.GetEnvironmentVariable("URLBASE_PAQUETES") ?? "";
    private readonly string _urlMonederoQ = Environment.GetEnvironmentVariable("MONEDEROQ_URL") ?? "";
    private readonly IServGenerico _servGenerico;
    private readonly IBusTipoTarifa _busTipoTarifa;
    private readonly IDatUsuario _datUsuario;
    private readonly string _errorCodeSesion = Environment.GetEnvironmentVariable("ERROR_CODE_SESION") ?? "";

    public BusTarjetaUsuario(ILogger<BusTarjetaUsuario> logger, IDatTarjetaUsuario datTarjetas, IServiceProvider serviceProvider, ExchangeConfig exchangeConfig, IServGenerico servGenerico,
        IAuthService authService, IBusMonedero busMonedero, IBusTipoTarifa busTipoTarifa, IDatUsuario datUsuario)
    {
        _logger = logger;
        _rabbitNotifications = serviceProvider.GetRequiredService<IMDRabbitNotifications>();
        _exchangeConfig = exchangeConfig;
        _datTarjetas = datTarjetas;
        _servGenerico = servGenerico;
        _authService = authService;
        _busMonedero = busMonedero;
        _busTipoTarifa = busTipoTarifa;
        _datUsuario = datUsuario;
    }

    public async Task<IMDResponse<List<EntTarjetaUsuario>>> BTarjetas(Guid uIdUsuario, Guid uIdMonedero, string token, string? sIdAplicacion = null)
    {
        IMDResponse<List<EntTarjetaUsuario>> response = new IMDResponse<List<EntTarjetaUsuario>>();

        string metodo = nameof(this.BTarjetas);
        _logger.LogInformation(IMDSerializer.Serialize(67823464122127, $"Inicia {metodo}(Guid uIdUsuario, Guid uIdMonedero)", uIdUsuario, uIdMonedero));

        try
        {
            var entUsuarios = await _datUsuario.DGet(uIdUsuario);

            if (sIdAplicacion != null)
            {
                if ((entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.BLOQUEADO || entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.DESBLOQUEADO) && entUsuarios.Result.sIdAplicacion != sIdAplicacion)
                {
                    response.SetError(Menssages.BusLoginOtherDevice);
                    response.Result = new List<EntTarjetaUsuario>();
                    response.HttpCode = HttpStatusCode.PreconditionFailed;
                    response.ErrorCode = int.Parse(_errorCodeSesion);
                    return response;
                }
                if (entUsuarios.Result.iEstatusCuenta == (int)eEstatusCuenta.REPORTADO)
                {
                    response.SetError(Menssages.BusBlockedAccountApp);
                    response.Result = new List<EntTarjetaUsuario>();
                    response.HttpCode = HttpStatusCode.PreconditionFailed;
                    response.ErrorCode = int.Parse(_errorCodeSesion);
                    return response;
                }
            }

            var respTipoTarifas = await _busTipoTarifa.BGetAll();

            if (respTipoTarifas == null)
            {
                response.SetError(Menssages.BusNoRegisters);
                return response;
            }
            if (respTipoTarifas.HasError)
            {
                response.ErrorCode = respTipoTarifas.ErrorCode;
                response.SetError(respTipoTarifas.Message);
                return response;
            }

            response = await _datTarjetas.DTarjetas(uIdUsuario);

            response.Result.ForEach(x =>
            {
                if (x.sNumeroTarjeta?.Length < 16)
                {
                    x.sNumeroTarjeta = new String('0', 16 - x.sNumeroTarjeta.Length) + x.sNumeroTarjeta;
                }

                //Asignar el tipo de tarifa.
                try
                {
                    x.iTipoTarjeta = respTipoTarifas.Result.Where(tt => tt.uIdTipoTarifa == x.uIdTipoTarifa).FirstOrDefault()!.iTipoTarjeta;
                }
                catch { }

                //Obtener las operaciones permitidas para la tarjeta...
                try
                {
                    var resultOperaciones = BValidaTarjetaV2(x.sNumeroTarjeta, token, x.iTipoTarjeta).Result.Result;
                    x.entOperaciones = resultOperaciones;
                }
                catch { x.entOperaciones = null; }
            });

            var authReponse = await _authService.BIniciarSesion();
            if (authReponse.HasError != true)
            {
                var listatarjetas = response.Result;

                //Valida que estatus tiene el monedero
                var estatusMonedero = await _busMonedero.BEstatusMonedero(uIdMonedero);
                if (estatusMonedero.HasError == true)
                {
                    response.ErrorCode = estatusMonedero.ErrorCode;
                    string message = Menssages.BusMonedero;
                    if (estatusMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                    {
                        message = message + estatusMonedero.Message;
                    }
                    else
                    {
                        message = estatusMonedero.Message;
                    }
                    response.SetError(message);
                    return response;
                }
                //Termina validación del estatus del monedero

                //Validacion del estatus de la tarjeta
                foreach (var tarjeta in listatarjetas.ToList())
                {
                    var validaTrajeta = await BValidaEstatusTarjeta(tarjeta.sNumeroTarjeta, authReponse.Result.sToken);
                    if (validaTrajeta.HasError == true)
                    {
                        //Si ocurre un error con la tarjeta no la muestres
                        var tajetaEliminarLista = response.Result.Find(e => e.sNumeroTarjeta == tarjeta.sNumeroTarjeta);
                        if (tajetaEliminarLista != null)
                        {
                            response.Result.Remove(tajetaEliminarLista);
                        }
                    }

                    if (estatusMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion && tarjeta.uIdMonedero == uIdMonedero)
                    {
                        var temp = response.Result.FindIndex(e => e.sNumeroTarjeta == tarjeta.sNumeroTarjeta);

                        response.Result[temp].bBajaMonedero = true;
                    }
                }

                foreach (var ListTarjetas in response.Result)
                {
                    var saldoMonedero = await _busMonedero.BGetSaldo(ListTarjetas.uIdMonedero, authReponse.Result.sToken, null, uIdUsuario);
                    if (saldoMonedero.HasError != true)
                    {
                        ListTarjetas.dSaldo = saldoMonedero.Result.dSaldo;
                    }
                    else
                    {
                        response.ErrorCode = saldoMonedero.ErrorCode;
                        response.SetError(saldoMonedero.Message);
                        return response;
                    }
                }
            }
            else
            {
                response.ErrorCode = authReponse.ErrorCode;
                response.SetError(authReponse.Message);
                return response;
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464122904;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464122904, $"Error en {metodo}(Guid uIdUsuario, Guid uIdMonedero): {ex.Message}", uIdUsuario, uIdMonedero, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462057638, 67823462056861)]
    public async Task<IMDResponse<bool>> BVincularTarjeta(EntVincularTarjeta entVincularTarjeta, string token)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntVincularTarjeta entVincularTarjeta)", entVincularTarjeta));

        try
        {
            // Busca si existe la tarjeta para poderla vincular
            var httpResponse = await BHttpGetTarjeta(entVincularTarjeta, token);
            if (httpResponse.HasError)
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.HasError = true;
                response.Message = httpResponse.Message;
                response.HttpCode = httpResponse.HttpCode;
                return response;
            }

            string endpointTipoTarifa = Environment.GetEnvironmentVariable("ENDPOINT_GET_TIPOTARIFAS") ?? "";
            endpointTipoTarifa = endpointTipoTarifa.Replace("{idTarifas}", httpResponse.Result.uIdTipoTarifa.ToString());
            endpointTipoTarifa = $"{endpointTipoTarifa}{"&Activo=true"}";
            var httpResponseCatalogo = await _servGenerico.SGetPath(_urlCatalogo, endpointTipoTarifa, token);

            if (httpResponseCatalogo.HasError != true)
            {
                var obj = JsonConvert.DeserializeObject<EntReadTipoTarifas>(httpResponseCatalogo.Result.ToString());

                if (obj == null)
                {
                    response.SetError(Menssages.BusTipoTarifaFailed);
                    return response;
                }
                httpResponse.Result.sTipoTarifa = obj.nombreTarifa;
                httpResponse.Result.iTipoTarjeta = obj.tipoTarjeta;
            }
            else
            {
                response.ErrorCode = httpResponseCatalogo.ErrorCode;
                response.SetError(httpResponseCatalogo.Message);
            }

            if (httpResponse.Result == null)
            {
                return response.GetResponse(httpResponse);
            }

            // verifica si existe la tarjeta a vinculada
            response = await _datTarjetas.DTarjeta(httpResponse.Result.uIdTarjeta);
            if (response.HasError)
            {
                return response;
            }

            response = await _datTarjetas.DVincularTarjeta(httpResponse.Result);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(entVincularTarjeta entVincularTarjeta): {ex.Message}", entVincularTarjeta, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823462059192, 67823462058415)]
    public async Task<IMDResponse<bool>> BDesvincularTarjeta(EntDesvincularTarjeta entDesvincularTarjeta)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(entDesvincularTarjeta entDesvincularTarjeta)", entDesvincularTarjeta));

        try
        {
            response = await _datTarjetas.DDesvincularTarjeta(entDesvincularTarjeta);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntDesvincularTarjeta entDesvincularTarjeta): {ex.Message}", entDesvincularTarjeta, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823463827644, 67823463826867)]
    public async Task<IMDResponse<EntTarjetaUsuario>> BGetTarjetaByID(Guid uIdUsuario, Guid uIdTarjeta)
    {
        IMDResponse<EntTarjetaUsuario> response = new IMDResponse<EntTarjetaUsuario>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdTarjeta, Guid uIdTarjeta)", uIdTarjeta, uIdTarjeta));

        try
        {
            var tarjetaResponse = await _datTarjetas.DGetTarjetaByID(uIdUsuario, uIdTarjeta);

            if (tarjetaResponse.HasError != true)
            {
                EntTarjetaUsuario tarjetaData = tarjetaResponse.Result;

                var authReponse = await _authService.BIniciarSesion();
                if (authReponse.HasError != true)
                {
                    var validaTrajeta = await BValidaEstatusTarjeta(tarjetaData.sNumeroTarjeta, authReponse.Result.sToken);

                    if (validaTrajeta.HasError == true)
                    {
                        response.ErrorCode = validaTrajeta.ErrorCode;
                        string message = Menssages.BusCard;
                        if (validaTrajeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                        {
                            message = message + validaTrajeta.Message;
                        }
                        else
                        {
                            message = validaTrajeta.Message;
                        }
                        response.SetError(message);
                        return response;
                    }
                    //Termina la validacion de la tarjeta

                    var saldoMonedero = await _busMonedero.BGetSaldo(tarjetaData.uIdMonedero, authReponse.Result.sToken, null, uIdUsuario);
                    if (saldoMonedero.HasError != true)
                    {
                        tarjetaData.dSaldo = saldoMonedero.Result.dSaldo;
                        tarjetaData.iTipoTarjeta = saldoMonedero.Result.iTipoTarjeta;
                        response.SetSuccess(tarjetaData);
                    }
                    else
                    {
                        response.ErrorCode = saldoMonedero.ErrorCode;
                        response.SetError(saldoMonedero.Message);
                    }

                }
                else
                {
                    response.ErrorCode = authReponse.ErrorCode;
                    response.SetError(authReponse.Message);
                }
            }
            else
            {
                response.ErrorCode = tarjetaResponse.ErrorCode;
                response.SetError(tarjetaResponse.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);
            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdTarjeta, Guid uIdTarjeta): {ex.Message}", uIdTarjeta, uIdTarjeta, ex, response));
        }
        return response;
    }

    public async Task<IMDResponse<EntOperacionesTarjetas>> BValidaTarjeta(string sNumeroTarjeta, string token)
    {
        IMDResponse<EntOperacionesTarjetas> response = new IMDResponse<EntOperacionesTarjetas>();

        string metodo = nameof(this.BValidaTarjeta);
        _logger.LogInformation(IMDSerializer.Serialize(67823464154761, $"Inicia {metodo}(string sNumeroTarjeta, string token)", sNumeroTarjeta, token));

        try
        {
            //Ing. Benigno Manzano
            //Se valida los monederos para que sepamos que operaciones se permiten
            int? iTipoTarjeta = null;
            var httpResponseTarjeta = BGetDatosByNumTarjeta(sNumeroTarjeta, token).Result;
            string mensajeMonedero = Menssages.BusCard;
            if (httpResponseTarjeta.HasError != true)
            {
                iTipoTarjeta = httpResponseTarjeta.Result.iTipoTarjeta;
            }

            var resPermisos = await BValidaTarjetaV2(sNumeroTarjeta, token, iTipoTarjeta);
            if (resPermisos.HasError != true)
                response.SetSuccess(resPermisos.Result, resPermisos.Message);
            else
            {
                response.ErrorCode = resPermisos.ErrorCode;
                response.SetError(resPermisos.Message);
            }

            /*
            var permisosTarjeta = await BValidaEstatusTarjeta(sNumeroTarjeta, token);
            string message = Menssages.BusCard;
            if (permisosTarjeta.HasError != true)
            {
                var permisos = permisosTarjeta.Result;
                EntOperacionesTarjetas resPermisos = new EntOperacionesTarjetas();
                if (!(string.IsNullOrEmpty(permisos.sTodasOperaciones)))
                {
                    resPermisos.bTodasOperaciones = true;
                    resPermisos.bDetalles = true;
                    resPermisos.bMovimientos = true;
                    resPermisos.bRecarga = true;
                    resPermisos.bTraspasos = true;
                    resPermisos.bVincular = true;
                    resPermisos.bVisualizar = true;
                    resPermisos.bGenerarQR = true;
                }
                else
                {
                    resPermisos.bTodasOperaciones = (string.IsNullOrEmpty(permisos.sTodasOperaciones)) ? false : true;
                    resPermisos.bDetalles = (string.IsNullOrEmpty(permisos.sDetalles)) ? false : true;
                    resPermisos.bMovimientos = (string.IsNullOrEmpty(permisos.sMovimientos)) ? false : true;
                    resPermisos.bRecarga = (string.IsNullOrEmpty(permisos.sRecarga)) ? false : true;
                    resPermisos.bTraspasos = (string.IsNullOrEmpty(permisos.sTraspasos)) ? false : true;
                    resPermisos.bVincular = (string.IsNullOrEmpty(permisos.sVincular)) ? false : true;
                    resPermisos.bVisualizar = (string.IsNullOrEmpty(permisos.sVisualizar)) ? false : true;
                    resPermisos.bGenerarQR = (string.IsNullOrEmpty(permisos.sGenerarQR)) ? false : true;
                }

                response.ErrorCode = permisosTarjeta.ErrorCode;
                response.SetSuccess(resPermisos, message + permisosTarjeta.Message);
            }
            else
            {
                response.ErrorCode = permisosTarjeta.ErrorCode;

                if (permisosTarjeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                {
                    message = message + permisosTarjeta.Message;
                }
                else
                {
                    message = permisosTarjeta.Message;
                }
                response.SetError(message);
                return response;
            }
            */
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464155538;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464155538, $"Error en {metodo}(string sNumeroTarjeta, string token): {ex.Message}", sNumeroTarjeta, token, ex, response));
        }
        return response;
    }

    //Ing. Benigno Manzano
    //Se agrega un nuevo servicio en el cual puda recibir el tipo de tarjeta para poder devolver si estan activados o no por este nuevo filtro
    public async Task<IMDResponse<EntOperacionesTarjetas>> BValidaTarjetaV2(string sNumeroTarjeta, string token, int? iTipoTarjeta = null)
    {
        IMDResponse<EntOperacionesTarjetas> response = new IMDResponse<EntOperacionesTarjetas>();

        string metodo = nameof(this.BValidaTarjeta);
        _logger.LogInformation(IMDSerializer.Serialize(67823464154761, $"Inicia {metodo}(string sNumeroTarjeta, string token, int iTipoTarjeta)", sNumeroTarjeta, token, iTipoTarjeta));

        try
        {
            var permisosTarjeta = await BValidaEstatusTarjeta(sNumeroTarjeta, token, iTipoTarjeta);
            string message = Menssages.BusCard;
            if (permisosTarjeta.HasError != true)
            {
                var permisos = permisosTarjeta.Result;
                EntOperacionesTarjetas resPermisos = new EntOperacionesTarjetas();
                if (!(string.IsNullOrEmpty(permisos.sTodasOperaciones)))
                {
                    resPermisos.bTodasOperaciones = true;
                    resPermisos.bDetalles = true;
                    resPermisos.bMovimientos = true;
                    resPermisos.bRecarga = true;
                    resPermisos.bTraspasos = true;
                    resPermisos.bVincular = true;
                    resPermisos.bVisualizar = true;
                    resPermisos.bGenerarQR = true;
                }
                else
                {
                    resPermisos.bTodasOperaciones = (string.IsNullOrEmpty(permisos.sTodasOperaciones)) ? false : true;
                    resPermisos.bDetalles = (string.IsNullOrEmpty(permisos.sDetalles)) ? false : true;
                    resPermisos.bMovimientos = (string.IsNullOrEmpty(permisos.sMovimientos)) ? false : true;
                    resPermisos.bRecarga = (string.IsNullOrEmpty(permisos.sRecarga)) ? false : true;
                    resPermisos.bTraspasos = (string.IsNullOrEmpty(permisos.sTraspasos)) ? false : true;
                    resPermisos.bVincular = (string.IsNullOrEmpty(permisos.sVincular)) ? false : true;
                    resPermisos.bVisualizar = (string.IsNullOrEmpty(permisos.sVisualizar)) ? false : true;
                    resPermisos.bGenerarQR = (string.IsNullOrEmpty(permisos.sGenerarQR)) ? false : true;
                }

                response.ErrorCode = permisosTarjeta.ErrorCode;
                response.SetSuccess(resPermisos, message + permisosTarjeta.Message);
            }
            else
            {
                response.ErrorCode = permisosTarjeta.ErrorCode;

                if (permisosTarjeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                {
                    message = message + permisosTarjeta.Message;
                }
                else
                {
                    message = permisosTarjeta.Message;
                }
                response.SetError(message);
                return response;
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464155538;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464155538, $"Error en {metodo}(string sNumeroTarjeta, string token, int iTipoTarjeta): {ex.Message}", sNumeroTarjeta, token, iTipoTarjeta, ex, response));
        }
        return response;
    }

    public async Task<IMDResponse<EntCreateTarjeta>> BHttpGetTarjeta(EntVincularTarjeta item, string token)
    {
        IMDResponse<EntCreateTarjeta> response = new IMDResponse<EntCreateTarjeta>();
        EntCreateTarjeta entTarjetaUsuario = new EntCreateTarjeta();
        try
        {
            var body = new
            {
                numeroTarjeta = Convert.ToDecimal(item.sNumeroTarjeta)
            };

            string mensajeBloqueado = string.Empty;
            long erroCodeBloqueado = 0;
            string ruta = "Tarjetas";

            var httpResponse = await _servGenerico.SPostBody(_url, ruta, body, token);

            if (httpResponse.HasError != true)
            {
                var responseObject = JsonConvert.DeserializeObject<List<EntTarjetaRes>>(JsonConvert.SerializeObject(httpResponse.Result));
                if (responseObject == null)
                {
                    response.SetError(Menssages.BusNoRespon);
                    return response;
                }

                if (responseObject.Count() == 0)
                {
                    response.SetError(Menssages.BusVerificatedCard);
                    return response;
                }


                var res = responseObject?.First();
                if (res == null)
                {
                    response.SetError(Menssages.BusNoRespon);
                    return response;
                }

                var validaTrajeta = await BValidaEstatusTarjeta(res);

                if (validaTrajeta.HasError == true) //Siocurrio un error retorna respuesta
                {
                    response.ErrorCode = validaTrajeta.ErrorCode;
                    string message = Menssages.BusCard;
                    if (validaTrajeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                    {
                        message = message + validaTrajeta.Message;
                    }
                    else
                    {
                        message = validaTrajeta.Message;
                    }
                    response.SetError(message);
                    return response;
                }

                //Se cargan las operaciones
                var operacionesPermitidasTarjeta = validaTrajeta.Result;

                //Determina si puede vincular la tarjeta
                if (operacionesPermitidasTarjeta.sTodasOperaciones != OperacionesTarjeta.TodasOperaciones.GetDescription() || operacionesPermitidasTarjeta.sTodasOperaciones == null)
                {
                    if (operacionesPermitidasTarjeta.sVincular != null)
                    {
                        if (operacionesPermitidasTarjeta.sVincular != OperacionesTarjeta.Vincular.GetDescription())
                        {
                            response.ErrorCode = validaTrajeta.ErrorCode;
                            string message = Menssages.BusCard;
                            if (validaTrajeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                            {
                                message = message + validaTrajeta.Message;
                            }
                            else
                            {
                                message = validaTrajeta.Message;
                            }
                            response.SetError(message);
                            return response;
                        }
                        erroCodeBloqueado = validaTrajeta.ErrorCode;
                        mensajeBloqueado = validaTrajeta.Message;
                    }
                    else
                    {
                        response.ErrorCode = validaTrajeta.ErrorCode;
                        string message = Menssages.BusCard;
                        if (validaTrajeta.ErrorCode == EntConfiguracionEstatusTarjeta.iErrorCodeInformacion)
                        {
                            message = message + validaTrajeta.Message;
                        }
                        else
                        {
                            message = validaTrajeta.Message;
                        }
                        response.SetError(message);
                        return response;
                    }
                }
                //Termina la validacion de la tarjeta

                var bodyMonedero = new
                {
                    idMonedero = res.IdMonedero,
                    numPag = 1,
                    numReg = 1
                };

                ruta = "vista-monedero";

                var httpResponseMonedero = await _servGenerico.SPostBody(_urlMonederoQ, ruta, bodyMonedero, token);

                if (httpResponseMonedero.HasError != true)
                {
                    var responseMonedero = JsonConvert.DeserializeObject<EntPagination<EntMonederoRes>>(JsonConvert.SerializeObject(httpResponseMonedero.Result));
                    var resMonedero = responseMonedero?.Datos?.FirstOrDefault();

                    if (resMonedero == null || responseMonedero == null)
                    {
                        response.SetError(Menssages.BusNoExistMonedero);
                        return response;
                    }
                    //Valida que estatus tiene el monedero

                    var estatusMonedero = await _busMonedero.BEstatusMonedero(resMonedero);

                    if (estatusMonedero.HasError == true)
                    {
                        response.ErrorCode = estatusMonedero.ErrorCode;
                        string message = Menssages.BusMonedero;
                        if (estatusMonedero.ErrorCode == EntConfiguracionEstatusMonedero.iErrorCodeInformacion)
                        {
                            message = message + estatusMonedero.Message;
                        }
                        else
                        {
                            message = estatusMonedero.Message;
                        }
                        response.SetError(message);
                        return response;
                    }
                    //Termina la validacion del monedero

                    if (res.CCV != item.iClaveVerificacion.ToString())
                    {
                        response.SetError(Menssages.BusVerificatedCard);
                        return response;
                    }

                    entTarjetaUsuario.uIdTarjeta = res.IdTarjeta;
                    entTarjetaUsuario.uIdMonedero = res.IdMonedero;
                    entTarjetaUsuario.uIdUsuario = item.uIdUsuario;
                    entTarjetaUsuario.dSaldo = resMonedero.saldo;
                    entTarjetaUsuario.sNumeroTarjeta = res.NumeroTarjeta;
                    entTarjetaUsuario.iNoMonedero = res.NumeroMonedero;
                    entTarjetaUsuario.sTipoTarifa = res.TipoTarifa;
                    entTarjetaUsuario.uIdTipoTarifa = res.idTipoTarifa;
                    entTarjetaUsuario.sFechaVigencia = res.FechaVigencia;
                    entTarjetaUsuario.iActivo = 1;
                    entTarjetaUsuario.sMotivoBaja = "";
                    entTarjetaUsuario.sMotivoBloqueo = "";

                    if (erroCodeBloqueado != 0)
                    {
                        response.ErrorCode = erroCodeBloqueado;
                    }

                    if (string.IsNullOrEmpty(mensajeBloqueado))
                    {
                        response.SetSuccess(entTarjetaUsuario);
                    }
                    else
                    {
                        response.SetSuccess(entTarjetaUsuario, mensajeBloqueado);
                    }

                }
                else
                {
                    response.ErrorCode = httpResponseMonedero.ErrorCode;
                    response.SetError(httpResponseMonedero.Message);
                }
            }
            else
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.SetError(httpResponse.Message);
            }
        }
        catch (Exception ex)
        {
            response.SetError(ex);
        }
        return response;
    }

    //Valida la tarjeta sin hacer el llamado al servicio de tarjeta
    public async Task<IMDResponse<EntOperacionesPermitidasTarjeta>> BValidaEstatusTarjeta(EntTarjetaRes tarjeta)
    {
        IMDResponse<EntOperacionesPermitidasTarjeta> response = new IMDResponse<EntOperacionesPermitidasTarjeta>();

        string metodo = nameof(this.BValidaEstatusTarjeta);
        _logger.LogInformation(IMDSerializer.Serialize(67823464084831, $"Inicia {metodo}(EntTarjetaRes tarjeta)", tarjeta));

        try
        {
            EntOperacionesPermitidasTarjeta operacionesPermitidas = new EntOperacionesPermitidasTarjeta();
            string uIdEstatusTarjeta = tarjeta.idEstatusTarjeta.ToString() ?? "";

            if (tarjeta.Activo == true && tarjeta.Baja == false)
            {
                switch (uIdEstatusTarjeta)
                {
                    case EntConfiguracionEstatusTarjeta.sActiva:
                        operacionesPermitidas.sTodasOperaciones = OperacionesTarjeta.TodasOperaciones.GetDescription();
                        response.SetSuccess(operacionesPermitidas);
                        break;
                    case EntConfiguracionEstatusTarjeta.sInactiva:
                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                        response.SetError(Menssages.BusCardBloq);
                        return response;
                        break;
                    case EntConfiguracionEstatusTarjeta.sBloqueada:
                        if (tarjeta.entMotivos != null)
                        {
                            if (tarjeta.entMotivos.PermitirReactivar == null)
                            {
                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetError(Menssages.BusCardBloq);// Motivo
                                return response;
                            }
                            if (tarjeta.entMotivos.PermitirReactivar == false)
                            {
                                operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetSuccess(operacionesPermitidas, Menssages.BusCardBloqSimple + " " + tarjeta.entMotivos.Nombre);
                            }
                            else
                            {
                                if (tarjeta.entMotivos.PermitirOperaciones == null)
                                {
                                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                    response.SetError(Menssages.BusCardBloq);
                                    return response;
                                }
                                if (tarjeta.entMotivos.PermitirOperaciones == true)
                                {
                                    operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                    operacionesPermitidas.sRecarga = OperacionesTarjeta.Recarga.GetDescription();
                                    operacionesPermitidas.sTraspasos = OperacionesTarjeta.Traspasos.GetDescription();
                                    operacionesPermitidas.sVincular = OperacionesTarjeta.Vincular.GetDescription();
                                    operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, Menssages.BusCardBloqSimple + " " + tarjeta.entMotivos.Nombre);
                                }
                                else
                                {
                                    operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                    operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, Menssages.BusCardBloqSimple + " " + tarjeta.entMotivos.Nombre);
                                }
                            }
                        }
                        else
                        {
                            response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                            response.SetError(Menssages.BusCardBloq);// Motivo
                            return response;
                        }
                        break;
                    default:
                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                        response.SetError(Menssages.BusNoCorrectStatusConfig);
                        return response;
                        break;
                }
            }
            else
            {
                if (tarjeta.Activo == false && tarjeta.Baja == true)
                {
                    switch (uIdEstatusTarjeta)
                    {

                        case EntConfiguracionEstatusTarjeta.sBaja:
                            response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                            response.SetError(Menssages.BusNoAviableSytem);
                            return response;
                            break;
                        default:
                            response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                            response.SetError(Menssages.BusNoCorrectStatusConfig);
                            return response;
                            break;
                    }
                }
                else
                {
                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                    response.SetError(Menssages.BusNoCorrectConfig);
                    return response;
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464085608;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464085608, $"Error en {metodo}(EntTarjetaRes tarjeta): {ex.Message}", tarjeta, ex, response));
        }
        return response;
    }


    //Valida la tarjeta con llamado al servicio
    public async Task<IMDResponse<EntOperacionesPermitidasTarjeta>> BValidaEstatusTarjeta(string sNumeroTarjeta, string token, int? iTipoTarjeta = null)
    {
        IMDResponse<EntOperacionesPermitidasTarjeta> response = new IMDResponse<EntOperacionesPermitidasTarjeta>();

        string metodo = nameof(this.BValidaEstatusTarjeta);
        _logger.LogInformation(IMDSerializer.Serialize(67823464086385, $"Inicia {metodo}(string sNumeroTarjeta, string token, int? iTipoTarjeta = null)", sNumeroTarjeta, token, iTipoTarjeta));

        try
        {
            var body = new
            {
                numeroTarjeta = Convert.ToDecimal(sNumeroTarjeta)
            };
            EntOperacionesPermitidasTarjeta operacionesPermitidas = new EntOperacionesPermitidasTarjeta();
            string mensajeBloqueado = string.Empty;
            string ruta = "Tarjetas";

            var httpResponse = await _servGenerico.SPostBody(_url, ruta, body, token);

            if (httpResponse.HasError != true)
            {
                var responseObject = JsonConvert.DeserializeObject<List<EntTarjetaRes>>(JsonConvert.SerializeObject(httpResponse.Result));


                if (responseObject == null)
                {
                    response.SetError(Menssages.BusNoRespon);
                    return response;
                }

                if (responseObject.Count() == 0)
                {
                    response.SetError(Menssages.BusVerificatedCard);
                    return response;
                }


                var tarjeta = responseObject?.First();
                if (tarjeta == null)
                {
                    response.SetError(Menssages.BusNoRespon);
                    return response;
                }

                string uIdEstatusTarjeta = tarjeta.idEstatusTarjeta.ToString() ?? "";

                if (tarjeta.Activo == true && tarjeta.Baja == false)
                {
                    switch (uIdEstatusTarjeta)
                    {
                        case EntConfiguracionEstatusTarjeta.sActiva:
                            operacionesPermitidas.sTodasOperaciones = OperacionesTarjeta.TodasOperaciones.GetDescription();
                            response.SetSuccess(operacionesPermitidas);
                            break;
                        case EntConfiguracionEstatusTarjeta.sBloqueada:
                            if (tarjeta.entMotivos != null)
                            {
                                if (tarjeta.entMotivos.PermitirReactivar == null)
                                {
                                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                    response.SetError(Menssages.BusCardBloq);// Motivo
                                    return response;
                                }
                                if (tarjeta.entMotivos.PermitirReactivar == false)
                                {
                                    operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                    operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                    operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                    response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                    response.SetSuccess(operacionesPermitidas, Menssages.BusCardBloqSimple + " " + tarjeta.entMotivos.Nombre);
                                }
                                else
                                {
                                    if (tarjeta.entMotivos.PermitirOperaciones == null)
                                    {
                                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                        response.SetError(Menssages.BusCardBloq);
                                        return response;
                                    }
                                    if (tarjeta.entMotivos.PermitirOperaciones == true)
                                    {
                                        operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                        operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                        operacionesPermitidas.sRecarga = OperacionesTarjeta.Recarga.GetDescription();
                                        operacionesPermitidas.sTraspasos = OperacionesTarjeta.Traspasos.GetDescription();
                                        operacionesPermitidas.sVincular = OperacionesTarjeta.Vincular.GetDescription();
                                        operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                        response.SetSuccess(operacionesPermitidas, Menssages.BusCardBloqSimple + " " + tarjeta.entMotivos.Nombre);
                                    }
                                    else
                                    {
                                        operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                                        operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                                        operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();

                                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                        response.SetSuccess(operacionesPermitidas, Menssages.BusCardBloqSimple + " " + tarjeta.entMotivos.Nombre);

                                    }
                                }

                            }
                            else
                            {
                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetError(Menssages.BusCardBloq);// Motivo
                                return response;
                            }
                            break;
                        default:
                            response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                            response.SetError(Menssages.BusNoCorrectStatusConfig);
                            return response;
                    }
                }
                else
                {
                    if (tarjeta.Activo == false && tarjeta.Baja == true)
                    {
                        switch (uIdEstatusTarjeta)
                        {

                            case EntConfiguracionEstatusTarjeta.sBaja:
                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetError(Menssages.BusNoAviableSytem);
                                break;
                            default:
                                response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                                response.SetError(Menssages.BusNoCorrectStatusConfig);
                                break;
                        }
                    }
                    else
                    {
                        response.ErrorCode = EntConfiguracionEstatusTarjeta.iErrorCodeInformacion;
                        response.SetError(Menssages.BusNoCorrectConfig);
                        return response;
                    }
                }

                //Ing. Benigno Manzano
                //Se agrega una validacion adicional en la cual se identifica que las de sin costo no se pueden realizar recargas ni traspasos
                if (iTipoTarjeta != null)
                {
                    if (eTipoTarjeta.SINCOSTO == (eTipoTarjeta)iTipoTarjeta)
                    {
                        //Si hay todas las operaciones se tiene que agregar las operaciones restantes y solo no incluir las que se bloquearian
                        if(operacionesPermitidas.sTodasOperaciones != null)
                        {
                            operacionesPermitidas.sTodasOperaciones = null;
                            operacionesPermitidas.sDetalles = OperacionesTarjeta.Detalles.GetDescription();
                            operacionesPermitidas.sMovimientos = OperacionesTarjeta.Movimientos.GetDescription();
                            operacionesPermitidas.sGenerarQR = OperacionesTarjeta.GenerarQR.GetDescription();
                            operacionesPermitidas.sVincular = OperacionesTarjeta.Vincular.GetDescription();
                            operacionesPermitidas.sVisualizar = OperacionesTarjeta.Visualizar.GetDescription();
                        }
                        //Se quitan las operaciones no permitidas
                        else
                        {
                            operacionesPermitidas.sTodasOperaciones = null;
                            operacionesPermitidas.sRecarga = null;
                            operacionesPermitidas.sTraspasos = null;
                        }
                        response.SetSuccess(operacionesPermitidas);
                    }
                }

            }
            else
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.SetError(httpResponse.Message);
                return response;
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = 67823464087162;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823464087162, $"Error en {metodo}(string sNumeroTarjeta, string token, int? iTipoTarjeta = null): {ex.Message}", sNumeroTarjeta, token, iTipoTarjeta, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465283742, 67823465282965)]
    public async Task<IMDResponse<EntTarjetaUsuario>> BGetTarjetaByIdMonedero(Guid uIdMonedero)
    {
        IMDResponse<EntTarjetaUsuario> response = new IMDResponse<EntTarjetaUsuario>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMonedero)", uIdMonedero));

        try
        {
            response = await _datTarjetas.DGetTarjetaByIdMonedero(uIdMonedero);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMonedero): {ex.Message}", uIdMonedero, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465370766, 67823465369989)]
    public async Task<IMDResponse<EntDatosTarjeta>> BGetDatosByNumTarjeta(string sNumeroTarjeta, string token)
    {
        IMDResponse<EntDatosTarjeta> response = new IMDResponse<EntDatosTarjeta>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sNumeroTarjeta)", sNumeroTarjeta));

        try
        {
            var ruta = "Tarjetas/All";

            dynamic body = new ExpandoObject();
            body.numPag = 1;
            body.numReg = 10;
            body.iNumeroTarjeta = sNumeroTarjeta;

            var httpResponse = await _servGenerico.SPostBody(_url, ruta, body, token);

            if (httpResponse.HasError != true)
            {
                string s = httpResponse.Result.ToString();
                dynamic json = JObject.Parse(s);

                if (json != null)
                {
                    if (json["datos"].Count > 0)
                    {
                        //Obtener el saldo de la tarjeta...
                        decimal dSaldo = 0;
                        var datosMonedero = await _busMonedero.BDatosMonedero(Guid.Parse(json["datos"][0].IdMonedero.ToString()));
                        if (!datosMonedero.HasError)
                        {
                            dSaldo = datosMonedero.Result.saldo;
                        }

                        //Obtener la tarifa...
                        int iTipoTarjeta = 0;
                        var tarifa = await _busMonedero.BTipoTarifa(Guid.Parse(json["datos"][0].idTipoTarifa.ToString()));
                        if (!tarifa.HasError)
                        {
                            iTipoTarjeta = tarifa.Result.tipoTarjeta;
                        }

                        var monedero = new EntDatosTarjeta
                        {
                            uIdMonedero = json["datos"][0].IdMonedero,
                            NumeroTarjeta = json["datos"][0].NumeroTarjeta,
                            dSaldo = dSaldo,
                            iTipoTarjeta = iTipoTarjeta,
                            sEstatus = datosMonedero.Result.estatus
                        };

                        response.SetSuccess(monedero);
                    }
                    else
                    {
                        response.SetError("Sin respuesta exitosa del catalogo de tarjetas.");
                        return response;
                    }
                }
                else
                {
                    response.SetError("Sin respuesta exitosa del catalogo de tarjetas.");
                    return response;
                }
            }
            else
            {
                response.ErrorCode = httpResponse.ErrorCode;
                response.SetError(httpResponse.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sNumeroTarjeta): {ex.Message}", sNumeroTarjeta, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465569678, 67823465568901)]
    public async Task<IMDResponse<List<EntTarjetaUsuario>>> BGetTarjetasByUsuarioApp(Guid uIdUsuario)
    {
        IMDResponse<List<EntTarjetaUsuario>> response = new IMDResponse<List<EntTarjetaUsuario>>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

        try
        {
            response = await _datTarjetas.DTarjetas(uIdUsuario);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
        }
        return response;
    }

    [IMDMetodo(67823465603866, 67823465603089)]
    public async Task<IMDResponse<EntTarjetaUsuario>> BGetTarjetaByNumTarjeta(string sNumTarjeta)
    {
        IMDResponse<EntTarjetaUsuario> response = new IMDResponse<EntTarjetaUsuario>();
        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sNumTarjeta)", sNumTarjeta));

        try
        {
            response = await _datTarjetas.DGetTarjetaByNumTarjeta(sNumTarjeta);
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(67823465603866, $"Error en {metodo}(string sNumTarjeta): {ex.Message}", sNumTarjeta, ex, response));
        }
        return response;
    }
}