using RMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Interfaces
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
