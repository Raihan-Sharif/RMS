using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Models;

namespace SimRMS.WebAPI.Security
{
    /// <summary>
    /// Authentication scheme handler for Token authentication
    /// This handler works with TokenAuthenticationMiddleware which does the actual authentication
    /// </summary>
    public class TokenAuthenticationSchemeHandler : AuthenticationHandler<TokenAuthenticationSchemeOptions>
    {
        public TokenAuthenticationSchemeHandler(IOptionsMonitor<TokenAuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if user is already authenticated by TokenAuthenticationMiddleware
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var ticket = new AuthenticationTicket(Context.User, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            // If not authenticated, return no result (let the middleware handle challenges)
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Return 401 with proper headers for authentication challenges
            Context.Response.StatusCode = 401;
            Context.Response.Headers["WWW-Authenticate"] = "Bearer";
            
            return Context.Response.WriteAsync("Authentication required. Please provide a valid authorization token.");
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            // Return 403 for authorization failures
            Context.Response.StatusCode = 403;
            return Context.Response.WriteAsync("Access denied. You do not have permission to access this resource.");
        }
    }
}