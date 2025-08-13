using Microsoft.AspNetCore.Authentication;

namespace SimRMS.WebAPI.Security
{
    /// <summary>
    /// Options for Token authentication scheme
    /// </summary>
    public class TokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        // Empty options class - authentication logic is handled by TokenAuthenticationMiddleware
    }
}