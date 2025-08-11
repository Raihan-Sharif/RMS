using SimRMS.Application.Interfaces;
using System.Security.Claims;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Current User Service
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Current User Context and Session Validation
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

namespace SimRMS.WebAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityService _securityService;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ISecurityService securityService)
        {
            _httpContextAccessor = httpContextAccessor;
            _securityService = securityService;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        public string? FullName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("FullName");
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(x => x.Value) ?? Enumerable.Empty<string>();
        public IEnumerable<string> Permissions => _httpContextAccessor.HttpContext?.User?.FindAll("permission")?.Select(x => x.Value) ?? Enumerable.Empty<string>();

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public string? GetClaim(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
        }

        public async Task<bool> ValidateSessionAsync()
        {
            if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
                return false;

            return await _securityService.IsUserActiveAsync(UserId);
        }
    }
}
