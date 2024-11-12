using Microsoft.EntityFrameworkCore.Query;

namespace AppMonederoCommand.Business.Lenguaje
{
    public class BusLenguaje : IBusLenguaje
    {
        private readonly ILogger<BusLenguaje> _logger;
        private readonly IBusParametros _busParametros;
        private IMDResponse<EntParametros> urlParametros;

        public BusLenguaje(ILogger<BusLenguaje> logger, IBusParametros busParametros)
        {
            _logger = logger;
            _busParametros = busParametros;

            urlParametros = _busParametros.BObtener("APP_URL_RECOVERYPASSWORD").Result;
        }

        public bool SetLenguaje(string? pEntity)
        {
            try
            {
                if (!pEntity.IsNullOrEmpty())
                {

                    if (pEntity == eLenguajes.InglesLesguaje.GetDescription()
                        || pEntity == eLenguajes.EspañolLenguaje.GetDescription())
                    {
                        SetCulture(pEntity);
                        return true;
                    }

                    else
                    {
                        var lenguaje = pEntity.Split(",")[1].Split(";")[0]; //es-419,es;q=0.9

                        _logger.LogInformation("pEntity: " + pEntity + "/t lenguaje: " + lenguaje);

                        if (lenguaje == eLenguajes.InglesLesguaje.GetDescription()
                            || lenguaje == eLenguajes.EspañolLenguaje.GetDescription())
                        {
                            SetCulture(lenguaje);
                        }

                        else
                        {
                            SetCulture(eLenguajes.EspañolLenguaje.GetDescription());
                        }
                    }

                }
                else
                {

                    SetCulture(eLenguajes.EspañolLenguaje.GetDescription());
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("excepcion: ", ex);
                SetCulture(eLenguajes.EspañolLenguaje.GetDescription());
            }

            return true;
        }

        private void SetCulture(string pEntity)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(pEntity);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(pEntity);
            Environment.SetEnvironmentVariable("LENGUAJE_CONSUMO", pEntity);
        }
        public string BusSetLanguajeVentaSaldo()
        {
            try
            {

                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.VentaSaldo.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlBalancePurchase);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                html = html.Replace("{4}", Menssages.HtmlGreetings);
                html = html.Replace("{5}", Menssages.HtmlOperationDetails);

                html = html.Replace("{6}", Menssages.HtmlAmount);
                html = html.Replace("{7}", Menssages.HtmlOperationFolio);
                html = html.Replace("{8}", Menssages.HtmlDateAndTime);
                html = html.Replace("{9}", Menssages.HtmlImportant);
                html = html.Replace("{10}", Menssages.HtmlConcept);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeBienvenido()
        {
            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.Bienvenido.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlWelcome);
                html = html.Replace("{3}", Menssages.HtmlRegisterSuccess);
                html = html.Replace("{4}", Menssages.HtmlDownloadApp);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                html = html.Replace("{7}", Menssages.HtmlGreetings);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeDelCuenta()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.EliminaCuenta.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlDeletedAccount);
                html = html.Replace("{2}", Menssages.HtmlUsReceived);
                html = html.Replace("{3}", Menssages.HtmlDelAccount);
                html = html.Replace("{4}", Menssages.HtmlDelInfo);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                html = html.Replace("{6}", Menssages.HtmlDateAndTime);
                html = html.Replace("{7}", Menssages.HtmlGreetings);
                html = html.Replace("{8}", Menssages.HtmlOperationDetails);
                html = html.Replace("{9}", Menssages.HtmlOperationFolio);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeRecuperarPass()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.RecuperaPass.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlRecoverYourPassword);
                html = html.Replace("{2}", Menssages.HtmlRePassInfo);
                html = html.Replace("{3}", Menssages.HtmlNewPass);
                html = html.Replace("{4}", Menssages.HtmlIs);
                html = html.Replace("{5}", Menssages.HtmlImportant);
                html = html.Replace("{6}", Menssages.HtmlPassWarning);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                html = html.Replace("{9}", Menssages.HtmlGreetings);
                html = html.Replace("{urlRecovery}", urlParametros.Result.sValor);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeSugerencias()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.Sugerencias.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlReportIncide);
                html = html.Replace("{2}", Menssages.HtmlType);
                html = html.Replace("{3}", Menssages.HtmlDate);
                html = html.Replace("{4}", Menssages.HtmlInfo);
                html = html.Replace("{5}", Menssages.HtmlComent);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);

                html = html.Replace("{8}", Menssages.HtmlReportedBy);
                html = html.Replace("{9}", Menssages.HtmlContact);
                html = html.Replace("{10}", Menssages.HtmlUnid);
                html = html.Replace("{11}", Menssages.HtmlInfraType);
                html = html.Replace("{12}", Menssages.HtmlRoute);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeTemporalPass()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.TemporalPass.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlTemporalPassword);
                html = html.Replace("{2}", Menssages.HtmlSendTemporalPass);
                html = html.Replace("{3}", Menssages.HtmlRememberPass);
                html = html.Replace("{4}", Menssages.Html48Hours);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                html = html.Replace("{7}", Menssages.HtmlGreetings);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeVerificationCode()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.VerificationCode.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", "Código de verificación");
                html = html.Replace("{2}", Menssages.HtmlSendCode);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                html = html.Replace("{5}", Menssages.HtmlGreetings);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeTraspasoSaldo()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.TraspasoSaldo.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlTransfer);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                html = html.Replace("{4}", Menssages.HtmlGreetings);
                html = html.Replace("{5}", Menssages.HtmlOperationDetails);
                html = html.Replace("{6}", Menssages.HtmlAmount);
                html = html.Replace("{7}", Menssages.HtmlOperationFolio);
                html = html.Replace("{8}", Menssages.HtmlDateAndTime);
                html = html.Replace("{9}", Menssages.HtmlImportant);
                html = html.Replace("{10}", Menssages.HtmlOrigin);
                html = html.Replace("{11}", Menssages.HtmlDestiny);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeEliminaCuentaCode()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.CodeEliminaCuenta.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlAccountCancelation);
                html = html.Replace("{2}", Menssages.HtmlCodeVerification);
                html = html.Replace("{3}", Menssages.HtmlEnterCode);
                html = html.Replace("{4}", Menssages.HtmlCodeValidity);
                html = html.Replace("{5}", Menssages.HtmlDontShare);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string BusSetLanguajeVerificationCodeVigencia()
        {

            try
            {
                var x = Thread.CurrentThread.CurrentCulture;
                var html = Resources.Main.ToString().Replace("{body}", Resources.VerificationCodeVigencia.ToString());
                html = html.Replace("{0}", Menssages.HtmlConfirmationEmail);
                html = html.Replace("{1}", Menssages.HtmlRequestVerificationCode);
                html = html.Replace("{2}", Menssages.HtmlCodeVerification);
                html = html.Replace("{3}", Menssages.HtmlEnterCode);
                html = html.Replace("{4}", Menssages.HtmlCodeValidity);
                html = html.Replace("{5}", Menssages.HtmlDontShare);
                html = html.Replace("{HtmlInfoEmail}", Menssages.HtmlInfoEmail);
                return html;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
