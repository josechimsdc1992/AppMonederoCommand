using System;
namespace AppMonederoCommand.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "X-API-KEY";
        private readonly string apiKey = Environment.GetEnvironmentVariable("APP_API_KEY") ?? "";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                if (apiKey.Equals(extractedApiKey))
                {
                    var claims = new[] { new System.Security.Claims.Claim("ApiKey", extractedApiKey) };
                    var identity = new System.Security.Claims.ClaimsIdentity(claims, "ApiKey");
                    context.User = new System.Security.Claims.ClaimsPrincipal(identity);
                }
            }
            await _next(context);
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}

