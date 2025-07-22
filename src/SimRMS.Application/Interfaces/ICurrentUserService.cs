using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
