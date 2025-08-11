using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Models.Auth;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Token Authentication Middleware
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Token-Based Authentication and Authorization
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
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenAuthenticationMiddleware> _logger;

        public TokenAuthenticationMiddleware(
            RequestDelegate next,
            IServiceProvider serviceProvider,
            ILogger<TokenAuthenticationMiddleware> logger)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var securityRouteService = scope.ServiceProvider.GetRequiredService<ISecurityRouteService>();

            // Use configuration-based route checking
            if (securityRouteService.IsAllowedRoute(context.Request.Path.Value ?? string.Empty))
            {
                _logger.LogDebug("Skipping authentication for allowed route: {Path}", context.Request.Path);
                await _next(context);
                return;
            }

            try
            {
                var tokenService = scope.ServiceProvider.GetRequiredService<IExternalTokenService>();
                var securityService = scope.ServiceProvider.GetRequiredService<ISecurityService>();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                // Check handshake token first
                var handshakeToken = ExtractHandshakeTokenFromRequest(context);
                if (string.IsNullOrEmpty(handshakeToken))
                {
                    _logger.LogWarning("Missing handshake token for route: {Path}", context.Request.Path);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Handshake token required");
                    return;
                }

                // Validate handshake token
                var handshakeCacheKey = $"HANDSHAKE_TOKEN_{handshakeToken}";
                var handshakeInfo = await cacheService.GetAsync<HandshakeTokenInfo>(handshakeCacheKey);

                if (handshakeInfo == null || !handshakeInfo.IsActive || handshakeInfo.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Invalid or expired handshake token for route: {Path}", context.Request.Path);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid or expired handshake token");
                    return;
                }

                // Check user token for authenticated endpoints
                var userToken = ExtractUserTokenFromRequest(context);

                if (!string.IsNullOrEmpty(userToken))
                {
                    var isValidToken = await tokenService.ValidateTokenAsync(userToken);

                    if (isValidToken)
                    {
                        var userId = ExtractUserIdFromToken(userToken) ?? "UNKNOWN";
                        var userSession = await securityService.GetUserSessionAsync(userId);

                        if (userSession != null && userSession.IsActive)
                        {
                            var claimsIdentity = new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, userId),
                                new Claim(ClaimTypes.Name, userSession.UserName),
                                new Claim(ClaimTypes.Email, userSession.Email),
                                new Claim("FullName", userSession.FullName),
                                new Claim("Token", userToken),
                                new Claim("HandshakeToken", handshakeToken)
                            }, "TokenAuthentication");

                            // Add roles
                            foreach (var role in userSession.Roles)
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                            }

                            // Add permissions
                            foreach (var permission in userSession.Permissions)
                            {
                                claimsIdentity.AddClaim(new Claim("permission", permission));
                            }

                            var principal = new ClaimsPrincipal(claimsIdentity);
                            context.User = principal;

                            // Update user activity
                            await securityService.UpdateUserActivityAsync(userId);

                            _logger.LogDebug("User {UserId} authenticated successfully for route: {Path}", userId, context.Request.Path);
                        }
                        else
                        {
                            _logger.LogWarning("User session not found or inactive for token on route: {Path}", context.Request.Path);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Invalid user token provided for route: {Path}", context.Request.Path);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in token authentication middleware for route: {Path}", context.Request.Path);
            }

            await _next(context);
        }

        private string? ExtractHandshakeTokenFromRequest(HttpContext context)
        {
            return context.Request.Headers["X-Handshake-Token"].FirstOrDefault();
        }

        private string? ExtractUserTokenFromRequest(HttpContext context)
        {
            var authHeader = context.Request.Headers[AppConstants.Headers.Authorization].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }

        private string? ExtractUserIdFromToken(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length >= 2)
                {
                    var payload = parts[1];
                    switch (payload.Length % 4)
                    {
                        case 2: payload += "=="; break;
                        case 3: payload += "="; break;
                    }

                    var payloadBytes = Convert.FromBase64String(payload);
                    var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
                    var payloadData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);

                    if (payloadData != null && payloadData.ContainsKey("PID"))
                    {
                        return payloadData["PID"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract user ID from token");
            }

            return null;
        }
    }
}