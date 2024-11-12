using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppMonederoCommand.Api.Authorization
{
    public class ApiKeyOrJwtAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            // Verificar JWT
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            // Verificar API Key
            var apiKey = context.HttpContext.Request.Headers["X-API-KEY"].FirstOrDefault();
            var validApiKey = Environment.GetEnvironmentVariable("APP_API_KEY") ?? "";

            if (!string.IsNullOrEmpty(apiKey) && apiKey == validApiKey)
            {
                return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}

