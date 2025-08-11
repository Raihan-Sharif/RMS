/// <summary>
/// <para>
/// ===================================================================
/// Title:       ICurrentUserService Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines methods to manage the current user's context and session validation.
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
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        string? FullName { get; }
        bool IsAuthenticated { get; }
        IEnumerable<string> Roles { get; }
        IEnumerable<string> Permissions { get; }
        bool IsInRole(string role);
        bool HasPermission(string permission);
        string? GetClaim(string claimType);
        Task<bool> ValidateSessionAsync();
    }
}
