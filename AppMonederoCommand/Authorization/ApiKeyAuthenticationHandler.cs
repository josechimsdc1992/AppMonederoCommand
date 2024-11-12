using System;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace AppMonederoCommand.Api.Authorization
{
	public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>

    {
        private readonly IConfiguration _configuration;
        private readonly string apiKey = Environment.GetEnvironmentVariable("APP_API_KEY") ?? "";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (providedApiKey == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("No API Key provided."));
            }

            if (apiKey.Equals(providedApiKey))
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
                var identity = new ClaimsIdentity(claims, "ApiKey");
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided."));
        }
    }
}

