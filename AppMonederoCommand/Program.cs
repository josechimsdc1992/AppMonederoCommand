using AppMonederoCommand.Business.Clases;
using AppMonederoCommand.Data;
using AppMonederoCommand.Entities.Config;

var builder = WebApplication.CreateBuilder(args);

// EnvironmentVariable
var EnvConfig = new IMDEnvironmentConfig();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Servicio de la App"
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

string PCKEY = Environment.GetEnvironmentVariable("PCKEY") ?? "";
string PCIV = Environment.GetEnvironmentVariable("PCIV") ?? "";

string CONNECTION_STRING = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
string CONNECTION_STRING_DECRIPTED = IMDSecurity.BDecrypt(CONNECTION_STRING, PCKEY, PCIV);

if (Environment.GetEnvironmentVariable("USE_OCI_CONECTION") == "1")
{
    string wallet_dir = "WalletOCI";
    OracleConfiguration.TnsAdmin = wallet_dir;
    OracleConfiguration.WalletLocation = wallet_dir;
}
builder.Services.AddDbContext<TransporteContext>(options => { options.UseOracle(EnvConfig.CONNECTION_STRING_DECRIPTED); }, ServiceLifetime.Scoped);
builder.Services.AddScoped<DBContextExtensions>();

#region Configurar AutoMapper con los perfiles de mapeo en el proyecto Mapping
var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<BusMappingProfile>();
    cfg.AddProfile<DBMappingProfile>();
});

IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton(provider => { return EnvConfig.EXCHANGE_CONFIG; });
builder.Services.AddSingleton(provider => { return EnvConfig.SERVICES; });
builder.Services.AddSingleton(provider => { return EnvConfig.PARAMETROS; });
#endregion

IoC.AddRegistration(builder.Services);

# region JWT y Api-Key
var key = builder.Configuration.GetSection("JwtSettings:Key");
var keyByte = Encoding.UTF8.GetBytes(key.Value!);

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyByte),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero

    };
});

builder.Services.AddAuthentication("ApiKeyScheme")
        .AddScheme<AuthenticationSchemeOptions, AppMonederoCommand.Api.Authorization.ApiKeyAuthenticationHandler>("ApiKeyScheme", null);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


#endregion

#region Rabbit Service
builder.Services.AddSingleton(provider =>
{
    return new ExchangeConfig
    {
        name = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE") ?? "",
        type = ExchangeType.Topic,
        durable = true
    };
});

builder.Services.AddLogging(builder => builder.AddConsole()).AddSingleton(provider =>
{
    ILogger<IMDRabbitNotifications> logger = provider.GetRequiredService<ILogger<IMDRabbitNotifications>>();

    return new IMDRabbitNotifications(logger, EnvConfig.RABBITMQ_CONFIG);

}).AddSingleton<ILoggerFactory, LoggerFactory>();
#endregion

# region Hosted Services
builder.Services.AddSingleton<AppMonederoCommand.Api.HostedServices.NotificationsService>();
builder.Services.AddHostedService<AppMonederoCommand.Api.HostedServices.NotificationsService>();

#endregion

#region Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
#endregion

#region Data
builder.Services.AddSingleton(provider => { return EnvConfig; });
builder.Services.AddSingleton<DataStorage>();
#endregion

#region TokenServices
builder.Services.AddSingleton<AppMonederoCommand.Api.TokenServices.TokenService>();
builder.Services.AddHostedService<AppMonederoCommand.Api.TokenServices.TokenService>();
//builder.Services.AddSingleton<IAuthService, AuthService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    var basePath = "/api/v1/app/api-movil";
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}" } };
        });
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

//JWT habilitar autentificaci�n
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();