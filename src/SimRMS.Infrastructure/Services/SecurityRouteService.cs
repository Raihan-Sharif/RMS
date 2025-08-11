using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Models;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Security Route Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This service manages security routes and checks if a request path is allowed based on configured routes and prefixes.
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

namespace SimRMS.Infrastructure.Services
{
    public class SecurityRouteService : ISecurityRouteService
    {
        private readonly IOptionsMonitor<SecurityConfiguration> _securityOptions;
        private readonly ICacheService _cacheService;
        private readonly ILogger<SecurityRouteService> _logger;
        private const string CACHE_KEY = "SECURITY_ALLOWED_ROUTES";
        private const string CACHE_PREFIX_KEY = "SECURITY_ALLOWED_ROUTE_PREFIXES";

        public SecurityRouteService(
            IOptionsMonitor<SecurityConfiguration> securityOptions,
            ICacheService cacheService,
            ILogger<SecurityRouteService> logger)
        {
            _securityOptions = securityOptions;
            _cacheService = cacheService;
            _logger = logger;
        }

        public bool IsAllowedRoute(string requestPath)
        {
            if (string.IsNullOrEmpty(requestPath))
                return false;

            try
            {
                var config = _securityOptions.CurrentValue;
                var comparison = config.CaseSensitiveMatching
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;

                // Check exact matches first (faster)
                if (config.AllowedRoutes.Any(route =>
                    string.Equals(requestPath, route, comparison)))
                {
                    _logger.LogDebug("Exact route match found for: {RequestPath}", requestPath);
                    return true;
                }

                // Check prefix matches
                if (config.AllowedRoutePrefixes.Any(prefix =>
                    requestPath.StartsWith(prefix, comparison)))
                {
                    _logger.LogDebug("Prefix route match found for: {RequestPath}", requestPath);
                    return true;
                }

                _logger.LogDebug("No route match found for: {RequestPath}", requestPath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking allowed route for: {RequestPath}", requestPath);
                return false;
            }
        }

        public bool IsAllowedRouteSegment(string requestPath)
        {
            if (string.IsNullOrEmpty(requestPath))
                return false;

            var config = _securityOptions.CurrentValue;
            var comparison = config.CaseSensitiveMatching
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return config.AllowedRoutePrefixes.Any(prefix =>
                requestPath.StartsWith(prefix, comparison));
        }

        public void RefreshRoutes()
        {
            try
            {
                _cacheService.RemoveAsync(CACHE_KEY);
                _cacheService.RemoveAsync(CACHE_PREFIX_KEY);
                _logger.LogInformation("Security routes cache refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing security routes cache");
            }
        }

        public List<string> GetAllowedRoutes()
        {
            return _securityOptions.CurrentValue.AllowedRoutes.ToList();
        }

        public List<string> GetAllowedRoutePrefixes()
        {
            return _securityOptions.CurrentValue.AllowedRoutePrefixes.ToList();
        }

        public void AddAllowedRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return;

            var config = _securityOptions.CurrentValue;
            if (!config.AllowedRoutes.Contains(route))
            {
                config.AllowedRoutes.Add(route);
                RefreshRoutes();
                _logger.LogInformation("Added allowed route: {Route}", route);
            }
        }

        public void RemoveAllowedRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return;

            var config = _securityOptions.CurrentValue;
            if (config.AllowedRoutes.Remove(route))
            {
                RefreshRoutes();
                _logger.LogInformation("Removed allowed route: {Route}", route);
            }
        }
    }
}