using Microsoft.IdentityModel.Tokens;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;
using System.Net;
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
        private readonly ICacheService _cacheService;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor, 
            ISecurityService securityService,
            ICacheService cacheService,
            ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _securityService = securityService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public int UserId => Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid));
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
            if (!IsAuthenticated || string.IsNullOrEmpty(UserName))
                return false;

            return await _securityService.IsUserActiveAsync(UserName);
        }

        public int? GetCurrentUserId()
        {
            try
            {
                if (IsAuthenticated)
                {
                    return UserId;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user ID");
                return null;
            }
        }

        public UserSession? GetCurrentUserSession()
        {
            try
            {
                if (!IsAuthenticated || string.IsNullOrEmpty(UserName))
                {
                    return null;
                }

                // Get user session from cache using the same pattern as SecurityService
                var cacheKey = $"{AppConstants.UserSessionCacheKey}{UserName}";
                var userSession = _cacheService.GetAsync<UserSession>(cacheKey).Result;
                
                return userSession;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user session");
                return null;
            }
        }

        public string GetClientIPAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return "127.0.0.1"; // Fallback for non-HTTP contexts
                }

                // Priority order: Custom header first, then standard headers
                var ipHeaders = new[]
                {
                    "LB-Client-IP",        // custom header - highest priority
                    "CF-Connecting-IP",    // Cloudflare - very reliable
                    "X-Real-IP",           // Nginx - single clean IP
                    "X-Forwarded-For",     // Standard but may have multiple IPs
                    "X-Client-IP"          // Some proxies use this
                };

                // Check headers in priority order
                foreach (var header in ipHeaders)
                {
                    var headerValue = httpContext.Request.Headers[header].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        var clientIP = ExtractValidIP(headerValue.Trim());
                        if (!string.IsNullOrWhiteSpace(clientIP))
                        {
                            _logger.LogDebug("Client IP from {Header}: {IP}", header, clientIP);
                            return clientIP;
                        }
                    }
                }

                // Fallback to direct connection
                var remoteIP = httpContext.Connection.RemoteIpAddress;
                if (remoteIP != null)
                {
                    var ipString = remoteIP.IsIPv4MappedToIPv6 ? remoteIP.MapToIPv4().ToString() : remoteIP.ToString();
                    
                    // If it's a public IP, use it
                    if (IsPublicIP(ipString))
                    {
                        _logger.LogDebug("Client IP from connection: {IP}", ipString);
                        return ipString;
                    }
                }

                // Development/localhost scenario
                _logger.LogInformation("Client IP resolved to localhost (normal for development)");
                return "127.0.0.1";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client IP address");
                return "127.0.0.1";
            }
        }

        private static string? ExtractValidIP(string headerValue)
        {
            // Handle comma-separated IPs (common in X-Forwarded-For)
            var ips = headerValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var ip in ips)
            {
                var cleanIP = ip.Trim();
                if (IsPublicIP(cleanIP))
                {
                    return cleanIP;
                }
            }
            
            return null;
        }

        private static bool IsPublicIP(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress) || !IPAddress.TryParse(ipAddress, out var parsedIP))
                return false;

            // Exclude invalid and private ranges
            if (ipAddress == "0.0.0.0" || ipAddress == "::1")
                return false;

            // IPv4 private ranges
            if (parsedIP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var bytes = parsedIP.GetAddressBytes();
                
                // 127.x.x.x (loopback)
                if (bytes[0] == 127) return false;
                
                // 10.x.x.x (private)
                if (bytes[0] == 10) return false;
                
                // 172.16.x.x - 172.31.x.x (private)
                if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return false;
                
                // 192.168.x.x (private)
                if (bytes[0] == 192 && bytes[1] == 168) return false;
                
                // 169.254.x.x (link-local)
                if (bytes[0] == 169 && bytes[1] == 254) return false;
            }

            return true; // It's a public IP
        }
    }
}
