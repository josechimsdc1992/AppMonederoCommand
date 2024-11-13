using AppMonederoCommand.Business.Interfaces.Parametro;
using AppMonederoCommand.Entities.Config;
using IMD.Utils;
using System.Collections.Generic;

namespace AppMonederoCommand.Business.Parametro
{
    public class BusParametros : IBusParametros
    {
        private readonly ILogger<BusParametros> _logger;
        private readonly IDatParametros _datParametros;
        private readonly IMDServiceConfig _iMDServiceConfig;
        private readonly IMDParametroConfig _IMDParametroConfig;
        private readonly IMDEnvironmentConfig _IMDEnvironmentConfig;
        private readonly IBusTipoOperaciones _busTipoOperaciones;
        private readonly IBusTipoTarifa _busTipoTarifa;
        private readonly IBusMotivos _busMotivos;

        public BusParametros(ILogger<BusParametros> logger, IDatParametros datParametros, 
            IMDServiceConfig iMDServiceConfig, IMDParametroConfig iMDParametroConfig, 
            IBusTipoOperaciones busTipoOperaciones, IMDEnvironmentConfig iMDEnvironmentConfig, 
            IBusTipoTarifa busTipoTarifa, IBusMotivos busMotivos)
        {
            _logger = logger;
            _datParametros = datParametros;
            _iMDServiceConfig = iMDServiceConfig;
            _IMDParametroConfig = iMDParametroConfig;
            _busTipoOperaciones = busTipoOperaciones;
            _IMDEnvironmentConfig = iMDEnvironmentConfig;
            _busTipoTarifa= busTipoTarifa;
            _busMotivos=busMotivos;
        }

        public async Task<IMDResponse<bool>> BAgregar(EntParametros entParametro)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BAgregar);
            _logger.LogInformation(IMDSerializer.Serialize(67823462193613, $"Inicia {metodo}(EntParametros entParametro)", entParametro));

            try
            {
                if (entParametro.bEncriptado == true)
                {
                    try
                    {
                        string sValor = IMDSecurity.BDecrypt(entParametro.sValor, _IMDEnvironmentConfig.PCKEY, _IMDEnvironmentConfig.PCIV);
                    }
                    catch
                    {
                        entParametro.sValor = IMDSecurity.BEncrypt(entParametro.sValor, _IMDEnvironmentConfig.PCKEY, _IMDEnvironmentConfig.PCIV);
                    }
                }

                entParametro.sNombre = entParametro.sNombre.ToUpper();
                entParametro.uIdParametro = Guid.NewGuid();
                entParametro.dtFechaCreacion = DateTime.Now;
                entParametro.bActivo = true;
                entParametro.bBaja = false;

                response = await _datParametros.DAgregar(entParametro);
            }
            catch (Exception ex)
            {

                response.SetError($"67823462194390 {Menssages.DatErrorGeneric}");
                _logger.LogError(IMDSerializer.Serialize(67823462194390, $"Error en {metodo}(EntParametros entParametro): {ex.Message}", entParametro, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<List<EntParametros>>> BObtener()
        {
            IMDResponse<List<EntParametros>> response = new IMDResponse<List<EntParametros>>();

            string metodo = nameof(this.BObtener);
            _logger.LogInformation(IMDSerializer.Serialize(67823462202937, $"Inicia {metodo}()"));

            try
            {
                response = await _datParametros.DObtener();
            }
            catch (Exception ex)
            {
                response.SetError($"67823462203714 {Menssages.DatErrorGeneric}");
                _logger.LogError(IMDSerializer.Serialize(67823462203714, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntParametros>> BObtener(string sNombre)
        {
            IMDResponse<EntParametros> response = new IMDResponse<EntParametros>();

            string metodo = nameof(this.BObtener);
            _logger.LogInformation(IMDSerializer.Serialize(67823462210707, $"Inicia {metodo}(string sNombre)", sNombre));

            try
            {
                response = await _datParametros.DObtener(sNombre);
                if (!response.HasError)
                {
                    if (response.Result.bEncriptado == true)
                    {
                        try
                        {
                            response.Result.sValor = IMDSecurity.BDecrypt(response.Result.sValor, _IMDEnvironmentConfig.PCKEY, _IMDEnvironmentConfig.PCIV);
                        }
                        catch
                        { }
                    }
                }
                else
                {
                    response.Result = new EntParametros
                    {
                        sValor = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                response.Message = $"67823462211484 {Menssages.DatErrorGeneric}";
                _logger.LogError(IMDSerializer.Serialize(67823462211484, $"Error en {metodo}(string sNombre): {ex.Message}", sNombre, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> BActualizar(EntActualizarParametros entParametros)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.BActualizar);
            _logger.LogInformation(IMDSerializer.Serialize(67823462215369, $"Inicia {metodo}(EntActualizarParametros entParametros)", entParametros));

            try
            {
                if (entParametros.bEncriptado == true)
                {
                    try
                    {
                        string sValor = IMDSecurity.BDecrypt(entParametros.sValor, _IMDEnvironmentConfig.PCKEY, _IMDEnvironmentConfig.PCIV);
                    }
                    catch
                    {
                        entParametros.sValor = IMDSecurity.BEncrypt(entParametros.sValor, _IMDEnvironmentConfig.PCKEY, _IMDEnvironmentConfig.PCIV);
                    }
                }

                entParametros.sNombre = entParametros.sNombre.ToUpper();
                entParametros.dtFechaModificacion = DateTime.Now;

                response = await _datParametros.DActualizar(entParametros);
            }
            catch (Exception ex)
            {
                response.Message = $"67823462216146 {Menssages.DatErrorGeneric}";
                _logger.LogError(IMDSerializer.Serialize(67823462216146, $"Error en {metodo}(EntActualizarParametros entParametros): {ex.Message}", entParametros, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<List<EntParametros>>> BObtenerByClaves(List<string> claves)
        {
            IMDResponse<List<EntParametros>> response = new IMDResponse<List<EntParametros>>();

            string metodo = nameof(this.BObtener);
            _logger.LogInformation(IMDSerializer.Serialize(67823462202937, $"Inicia {metodo}()"));

            try
            {
                response = await _datParametros.DObtenerByClaves(claves);
            }
            catch (Exception ex)
            {
                response.SetError($"67823462203714 {Menssages.DatErrorGeneric}");
                _logger.LogError(IMDSerializer.Serialize(67823462203714, $"Error en {metodo}(): {ex.Message}", ex, response));
            }
            return response;
        }
        public async void SetParametros()
        {
            string claveAPP_ABONAR_DESCRIPCION = nameof(_IMDParametroConfig.PARAMETRO_APP_ABONAR_DESCRIPCION).Replace("PARAMETRO_", "");
            string claveAPP_ABONAR_GUID = nameof(_IMDParametroConfig.PARAMETRO_APP_ABONAR_GUID).Replace("PARAMETRO_", "");
            string claveAPP_APP_TRANSFERIR_DESCRIPCION = nameof(_IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_DESCRIPCION).Replace("PARAMETRO_", "");
            string claveAPP_APP_TRANSFERIR_GUID = nameof(_IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_GUID).Replace("PARAMETRO_", "");
            string claveAPP_VALIDACION_TRASPASO_MAIL = nameof(_IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_MAIL).Replace("PARAMETRO_", "");
            string claveAPP_ATI_ATENCION_CLIENTES_TELEFONO = nameof(_IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_TELEFONO).Replace("PARAMETRO_", "");
            string claveAPP_ATI_ATENCION_CLIENTES_EMAIL = nameof(_IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_EMAIL).Replace("PARAMETRO_", "");
            string claveAPP_APP_VALIDACION_TRASPASO_SMS = nameof(_IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_SMS).Replace("PARAMETRO_", "");
            string claveAPP_APP_VALIDACION_TRASPASO_PUSH = nameof(_IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_PUSH).Replace("PARAMETRO_", "");
            string claveAPP_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH = nameof(_IMDParametroConfig.PARAMETRO_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH).Replace("PARAMETRO_", "");

        List<string> listClaves = new List<string>();
            listClaves.Add(claveAPP_ABONAR_DESCRIPCION);
            listClaves.Add(claveAPP_ABONAR_GUID);
            listClaves.Add(claveAPP_APP_TRANSFERIR_GUID);
            listClaves.Add(claveAPP_APP_TRANSFERIR_DESCRIPCION);
            listClaves.Add(claveAPP_VALIDACION_TRASPASO_MAIL);
            listClaves.Add(claveAPP_ATI_ATENCION_CLIENTES_TELEFONO);
            listClaves.Add(claveAPP_ATI_ATENCION_CLIENTES_EMAIL);
            listClaves.Add(claveAPP_APP_VALIDACION_TRASPASO_SMS);
            listClaves.Add(claveAPP_APP_VALIDACION_TRASPASO_PUSH);
            listClaves.Add(claveAPP_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH);

            IMDResponse<List<EntParametros>> IMDListResponse = await BObtenerByClaves(listClaves);
            foreach (EntParametros entRead in IMDListResponse.Result)
            {
                SetConfiguracionModulo(entRead.sNombre, entRead.sValor);
            }

        }
        public async void SetConfiguracionModulo(string sClave, string sValor)
        {
            string claveAPP_ABONAR_DESCRIPCION = nameof(_IMDParametroConfig.PARAMETRO_APP_ABONAR_DESCRIPCION).Replace("PARAMETRO_", "");
            string claveAPP_ABONAR_GUID = nameof(_IMDParametroConfig.PARAMETRO_APP_ABONAR_GUID).Replace("PARAMETRO_", "");
            string claveAPP_APP_TRANSFERIR_DESCRIPCION = nameof(_IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_DESCRIPCION).Replace("PARAMETRO_", "");
            string claveAPP_APP_TRANSFERIR_GUID = nameof(_IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_GUID).Replace("PARAMETRO_", "");
            string claveAPP_VALIDACION_TRASPASO_MAIL = nameof(_IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_MAIL).Replace("PARAMETRO_", "");
            string claveAPP_ATI_ATENCION_CLIENTES_TELEFONO = nameof(_IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_TELEFONO).Replace("PARAMETRO_", "");
            string claveAPP_ATI_ATENCION_CLIENTES_EMAIL = nameof(_IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_EMAIL).Replace("PARAMETRO_", "");
            string claveAPP_APP_VALIDACION_TRASPASO_SMS = nameof(_IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_SMS).Replace("PARAMETRO_", "");
            string claveAPP_APP_VALIDACION_TRASPASO_PUSH = nameof(_IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_PUSH).Replace("PARAMETRO_", "");
            string claveAPP_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH = nameof(_IMDParametroConfig.PARAMETRO_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH).Replace("PARAMETRO_", "");


            _logger.LogInformation($"SetConfiguracionModulo:{sClave}-{sValor}");

            if (sClave == claveAPP_ABONAR_DESCRIPCION)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_ABONAR_DESCRIPCION = sValor;
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_APP_TRANSFERIR_DESCRIPCION)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_DESCRIPCION = sValor;
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_ABONAR_GUID)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_ABONAR_GUID = sValor;
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_APP_TRANSFERIR_GUID)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_TRANSFERIR_GUID = sValor;
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_VALIDACION_TRASPASO_MAIL)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_MAIL = bool.Parse(sValor);
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_ATI_ATENCION_CLIENTES_TELEFONO)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_TELEFONO = sValor;
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_ATI_ATENCION_CLIENTES_EMAIL)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_ATI_ATENCION_CLIENTES_EMAIL = sValor;
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_APP_VALIDACION_TRASPASO_SMS)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_SMS = bool.Parse(sValor);
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_APP_VALIDACION_TRASPASO_PUSH)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_VALIDACION_TRASPASO_PUSH = bool.Parse(sValor);
                }
                catch (Exception ex)
                {

                }

            }
            if (sClave == claveAPP_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH)
            {
                try
                {
                    _IMDParametroConfig.PARAMETRO_APP_DISPOSITIVOS_NOTIFICA_TRASPASO_PUSH = int.Parse(sValor ?? "2");
                }
                catch (Exception ex)
                {

                }

            }

        }
        public async void SetListadosAdicionales()
        {
            IMDResponse<List<EntTipoOperaciones>> res=await _busTipoOperaciones.BObtenerTipoOperaciones();
            if (!res.HasError)
            {
                _IMDParametroConfig.TipoOperaciones = res.Result;
            }

            IMDResponse<List<EntReplicaTipoTarifas>> resTipoTarifa = await _busTipoTarifa.BGetAll();
            if (!res.HasError)
            {
                _IMDParametroConfig.TipoTarifas = resTipoTarifa.Result;
            }

            IMDResponse<List<EntMotivo>> resMotivos = await _busMotivos.BObtenerTodos();
            if (!res.HasError)
            {
                _IMDParametroConfig.TipoTarifas = resTipoTarifa.Result;
            }

        }

    }
}
