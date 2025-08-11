using SimRMS.Shared.Models;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       ISecurity Service Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines the contract for security-related operations in the application, including handshake, login, and user validation.
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
    public interface ISecurityService
    {
        Task<UserSession?> AuthenticateAsync(string username, string password);
        Task<bool> ValidateUserAsync(string userId);
        Task<bool> AuthorizeAsync(string userId, string resource, string action);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<List<string>> GetUserPermissionsAsync(string userId);
        Task<bool> HasPermissionAsync(string userId, string permission);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task LogoutAsync(string userId);
        Task<bool> IsUserActiveAsync(string userId);
        Task<UserSession?> GetUserSessionAsync(string userId);
        Task UpdateUserActivityAsync(string userId);
    }
}
