using Microsoft.AspNetCore.Authorization;
using SimRMS.Application.Interfaces;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Custom Authorization Handler
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Custom Authorization Logic
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

namespace SimRMS.WebAPI.Security
{
    public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizationRequirement>
    {
        private readonly ISecurityService _securityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CustomAuthorizationHandler> _logger;

        public CustomAuthorizationHandler(
            ISecurityService securityService,
            ICurrentUserService currentUserService,
            ILogger<CustomAuthorizationHandler> logger)
        {
            _securityService = securityService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CustomAuthorizationRequirement requirement)
        {
            try
            {
                var userId = _currentUserService.UserId;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Authorization failed: User ID is null or empty");
                    context.Fail();
                    return;
                }

                // Check if user is active
                var isActive = await _securityService.IsUserActiveAsync(userId);
                if (!isActive)
                {
                    _logger.LogWarning("Authorization failed: User {UserId} is not active", userId);
                    context.Fail();
                    return;
                }

                // Update user activity
                await _securityService.UpdateUserActivityAsync(userId);

                // Check specific permission if required
                if (!string.IsNullOrEmpty(requirement.Permission))
                {
                    var hasPermission = await _securityService.HasPermissionAsync(userId, requirement.Permission);
                    if (!hasPermission)
                    {
                        _logger.LogWarning("Authorization failed: User {UserId} does not have permission {Permission}",
                            userId, requirement.Permission);
                        context.Fail();
                        return;
                    }
                }

                // Check specific role if required
                if (!string.IsNullOrEmpty(requirement.Role))
                {
                    var isInRole = await _securityService.IsInRoleAsync(userId, requirement.Role);
                    if (!isInRole)
                    {
                        _logger.LogWarning("Authorization failed: User {UserId} is not in role {Role}",
                            userId, requirement.Role);
                        context.Fail();
                        return;
                    }
                }

                context.Succeed(requirement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authorization error for user {UserId}", _currentUserService.UserId);
                context.Fail();
            }
        }
    }

    public class CustomAuthorizationRequirement : IAuthorizationRequirement
    {
        public string? Permission { get; set; }
        public string? Role { get; set; }
    }
}
