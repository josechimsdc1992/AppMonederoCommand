using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Config
{
    public class IMDEnvironmentConfig
    {
        public string SERVICE_NAME { get; internal set; }
        public string PCKEY { get; internal set; }
        public string PCIV { get; internal set; }
        public string CONNECTION_STRING { get; internal set; }
        public string CONNECTION_STRING_DECRIPTED { get; internal set; }
        public string JWT_ISSUER { get; internal set; }
        public string JWT_SECRET_KEY { get; internal set; }
        public string AUDIENCES { get; internal set; }
        public List<string> JWT_AUDIENCES { get; internal set; }
        public string API_VERSION { get; internal set; }
        public string ROLE_ACCESS_CONFIG { get; internal set; }
        public RabbitMQConfig RABBITMQ_CONFIG { get; internal set; }
        public ExchangeConfig EXCHANGE_CONFIG { get; internal set; }
        public IMDServiceConfig SERVICES { get; internal set; }
        public IMDParametroConfig PARAMETROS { get; internal set; }

        public IMDEnvironmentConfig()
        {
            Configure();
        }

        public void Configure()
        {
            GetEnviroment();
            SetEnviroment();
        }

        private void GetEnviroment()
        {
            SERVICE_NAME = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "SERVICE";

            PCKEY = Environment.GetEnvironmentVariable("PCKEY") ?? "";
            PCIV = Environment.GetEnvironmentVariable("PCIV") ?? "";

            CONNECTION_STRING = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
            CONNECTION_STRING_DECRIPTED = IMDSecurity.BDecrypt(CONNECTION_STRING, PCKEY, PCIV);

            JWT_ISSUER = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "";
            JWT_SECRET_KEY = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
            AUDIENCES = Environment.GetEnvironmentVariable("JWT_AUDIENCES") ?? "";
            JWT_AUDIENCES = AUDIENCES.Split(',').ToList();

            RABBITMQ_CONFIG = new RabbitMQConfig
            {
                host = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                port = Environment.GetEnvironmentVariable("RABBITMQ_PORT"),
                username = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
                password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"),
                queueConfig = new QueueConfig()
            };

            RABBITMQ_CONFIG.queueConfig.durable = true;

            EXCHANGE_CONFIG = new ExchangeConfig
            {
                name = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE") ?? "",
                type = ExchangeType.Topic,
                durable = true
            };

            SERVICES = new IMDServiceConfig
            {
                Seguridad_Host = IMDURL.NormalizeURL(Environment.GetEnvironmentVariable("SEGURIDAD-SERVICE-BASEURL") ?? ""),
                Seguridad_Login = IMDURL.NormalizeEndPoint(Environment.GetEnvironmentVariable("ENDPOINT-LOGIN") ?? ""),
                Seguridad_Refresh = IMDURL.NormalizeEndPoint(Environment.GetEnvironmentVariable("ENDPOINT-LOGIN") ?? ""),
                Seguridad_UserName = Environment.GetEnvironmentVariable("SYSTEM-USER-NAME") ?? "",
                Seguridad_Password = Environment.GetEnvironmentVariable("SYSTEM-USER-PASSWORD") ?? "",
                MonederoC_Host = IMDURL.NormalizeURL(Environment.GetEnvironmentVariable("MONEDEROC_URL") ?? ""),
                MonederoC_Abonar = "abonar",
                MonederoC_Traspaso= "traspaso-saldo"
            };

            SERVICES.Seguridad_UserName = IMDSecurity.BDecrypt(SERVICES.Seguridad_UserName, PCKEY, PCIV);
            SERVICES.Seguridad_Password = IMDSecurity.BDecrypt(SERVICES.Seguridad_Password, PCKEY, PCIV);

            PARAMETROS = new IMDParametroConfig
            {
                Parametro_Error_Generico = Environment.GetEnvironmentVariable("PARAMETRO-ERROR-GENERICO") ?? ""

            };

            string errorCode = Environment.GetEnvironmentVariable("ERROR_CODE_SESION");
            PARAMETROS._errorCodeSesion = errorCode;
        }

        private void SetEnviroment()
        {
            API_VERSION = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.1";
            Environment.SetEnvironmentVariable("API_VERSION", API_VERSION);

        }

    }
}
