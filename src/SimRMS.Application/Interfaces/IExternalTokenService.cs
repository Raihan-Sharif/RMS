/// <summary>
/// <para>
/// ===================================================================
/// Title:       IExternalTokenService Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines methods for managing external token operations such as generation, validation, and revocation.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
///

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
