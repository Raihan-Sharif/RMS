using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Application.Interfaces
{
    public interface IExternalTokenService
    {
        Task<string> GenerateTokenAsync(string userId, Dictionary<string, object>? additionalClaims = null);
        Task<bool> ValidateTokenAsync(string token);
        Task<Dictionary<string, object>?> GetTokenClaimsAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<bool> HandshakeAsync();
    }
}
