namespace AppMonederoCommand.Api.Controllers;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 21/08/2023 | Ing. César Cárdenas    | Creación
* ---------------------------------------------------------------------------------------
*      2        | 11/12/2023 | Oscar Luna             | Actualización 
* ---------------------------------------------------------------------------------------
*/
[ApiController]
[Authorize]
[Route("api-movil/monederos")]
[SwaggerTag("Manejo de monedero")]
public class MonederoController : ControllerBase
{
    private readonly ILogger<MonederoController> _logger;
    private readonly IBusMonedero _busMonedero;
    private readonly IAuthService _authService;
    private readonly IBusLenguaje _busLenguaje;
    private readonly IBusUsuario _busUsuario;
    private readonly IBusTarjetas _busTarjeta;

    public MonederoController(ILogger<MonederoController> logger, IBusMonedero busMonedero, 
        IAuthService authService, IBusLenguaje busLenguaje, 
        IBusUsuario busUsuario,
        IBusTarjetas busTarjetas)
    {
        _logger = logger;
        _busMonedero = busMonedero;
        _authService = authService;
        _busLenguaje = busLenguaje;
        _busUsuario = busUsuario;
        _busTarjeta= busTarjetas;
    }

  

    [SwaggerOperation(Summary = "Abonar",
    Description = "Registra el abono a un monedero")]
    [HttpPost("abonar")]
    [IMDMetodo(67823462805112, 67823462804335)]
    public async Task<ActionResult<IMDResponse<bool>>> CPostAbonar([FromBody] EntAbonar entAbonar)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntAbonar entAbonar)", entAbonar));

        try
        {
            _busLenguaje.SetLenguaje(Request.Headers[eLenguajes.HeaderLenguaje.GetDescription()]);
            var authReponse = await _authService.BIniciarSesion();
            var result = await _busMonedero.BAbonar(entAbonar, authReponse.Result.sToken);
            response = result.Item1;
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntAbonar entAbonar): {ex.Message}", entAbonar, ex, response));
        }
        return response;
    }

    [SwaggerOperation(Summary = "Transferir saldo",
    Description = "Transferir saldo entre monederos")]
    [HttpPost("transferir-saldo")]
    [IMDMetodo(67823462806666, 67823462805889)]
    public async Task<ActionResult<IMDResponse<bool>>> CPostTransferir([FromBody] EntTransferirSaldo entTransferirSaldo)
    {
        IMDResponse<bool> response = new IMDResponse<bool>();

        IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
        _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntTransferirSaldo entTransferirSaldo)"));

        try
        {
            _busLenguaje.SetLenguaje(Request.Headers[eLenguajes.HeaderLenguaje.GetDescription()]);
            var authReponse = await _authService.BIniciarSesion();
            var result = await _busMonedero.BTransferirSaldo(entTransferirSaldo, authReponse.Result.sToken);

            response = result.Item1;
            IMDResponse<TraspasoSaldoRequestModel> response02 = result.Item2;

            if (!response.HasError && response.Result)
            {
                //Obtener el usuario...
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    Guid uIdUsuario = Guid.Parse(identity.Claims.First(u => u.Type == "uIdUsuario").Value.ToString());

                    string sDestino = null, sOrigen = null;


                    IMDResponse<EntReadTarjetas> resTarjetaOrigen =await _busTarjeta.BGetByuIdMonedero(entTransferirSaldo.uIdMonederoOrigen);
                    if (!resTarjetaOrigen.HasError && resTarjetaOrigen.Result!=null)
                    {
                        sOrigen = resTarjetaOrigen.Result.iNumeroTarjeta + "-T";
                    }
                    else
                    {
                        var monederoOrigen = await _busMonedero.BDatosMonedero(entTransferirSaldo.uIdMonederoOrigen);
                        if (!monederoOrigen.HasError)
                        {
                            sOrigen = monederoOrigen.Result.numMonedero.ToString() + "-M";
                        }
                    }

                    IMDResponse<EntReadTarjetas> resTarjetaDestino = await _busTarjeta.BGetByuIdMonedero(entTransferirSaldo.uIdMonederoDestino);
                    if (!resTarjetaDestino.HasError && resTarjetaDestino.Result!=null)
                    {
                        sDestino = resTarjetaDestino.Result.iNumeroTarjeta + "-T";
                    }
                    else
                    {
                        var monederoDestino = await _busMonedero.BDatosMonedero(entTransferirSaldo.uIdMonederoDestino);
                        if (!monederoDestino.HasError)
                        {
                            sDestino = monederoDestino.Result.numMonedero.ToString() + "-M";
                        }
                    }

                    if (sOrigen != null && sDestino != null)
                    {
                        var responseNotif = await _busUsuario.BNotificarTraspaso(sOrigen, sDestino, uIdUsuario, response02.Result);
                        response.Message = responseNotif.Message;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            response.ErrorCode = metodo.iCodigoError;
            response.SetError(ex);

            _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntTransferirSaldo entTransferirSaldo): {ex.Message}", entTransferirSaldo, ex, response));
        }
        return response;
    }


}