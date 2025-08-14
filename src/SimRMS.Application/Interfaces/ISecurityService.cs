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
        Task<bool> ValidateUserAsync(string username);
        Task<bool> AuthorizeAsync(string username, string resource, string action);
        Task<List<string>> GetUserRolesAsync(string username);
        Task<List<string>> GetUserPermissionsAsync(string username);
        Task<bool> HasPermissionAsync(string username, string permission);
        Task<bool> IsInRoleAsync(string username, string role);
        Task LogoutAsync(string username);
        Task<bool> IsUserActiveAsync(string username);
        Task<UserSession?> GetUserSessionAsync(string username);
        Task UpdateUserActivityAsync(string username);
    }
}
