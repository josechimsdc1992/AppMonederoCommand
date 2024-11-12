using System.IdentityModel.Tokens.Jwt;

namespace AppMonederoCommand.Api.Middleware
{
    public class CombinedAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-KEY";

        public CombinedAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null && endpoint.Metadata.GetMetadata<AuthorizeAttribute>() != null)
            {
                if (context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
                {
                    var appSetting = context.RequestServices.GetRequiredService<IConfiguration>();
                    var apiKey = appSetting.GetValue<string>("ApiKey");

                    if (apiKey.Equals(extractedApiKey))
                    {
                        await _next(context);
                        return;
                    }
                }

                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(context.RequestServices.GetRequiredService<IConfiguration>()["JwtSettings:Key"]!);
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };

                    try
                    {
                        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                        await _next(context);
                        return;
                    }
                    catch
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                }

                context.Response.StatusCode = 401;
                return;
            }

            await _next(context);
        }
    }
}

