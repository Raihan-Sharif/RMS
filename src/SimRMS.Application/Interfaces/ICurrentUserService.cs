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

using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
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
        
        /// <summary>
        /// Get the current user's ID from session (nullable version for backward compatibility)
        /// </summary>
        /// <returns>Current user ID or null if not authenticated</returns>
        int? GetCurrentUserId();

        /// <summary>
        /// Get the current user session information
        /// </summary>
        /// <returns>User session or null if not authenticated</returns>
        UserSession? GetCurrentUserSession();

        /// <summary>
        /// Get the client IP address from the HTTP request
        /// </summary>
        /// <returns>Client IP address or fallback IP if not available</returns>
        string GetClientIPAddress();
    }
}
