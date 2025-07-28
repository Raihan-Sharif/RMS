using Microsoft.Extensions.Logging;

namespace SimRMS.Shared.Models
{
    public class SecurityConfiguration
    {
        public const string SectionName = "Security";

        public List<string> AllowedRoutes { get; set; } = new();
        public List<string> AllowedRoutePrefixes { get; set; } = new();
        public bool EnableStrictPathMatching { get; set; } = true;
        public bool CaseSensitiveMatching { get; set; } = false;
    }

    public class RouteMatchingService
    {
        private readonly SecurityConfiguration _config;
        private readonly ILogger<RouteMatchingService> _logger;

        public RouteMatchingService(SecurityConfiguration config, ILogger<RouteMatchingService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public bool IsAllowedRoute(string requestPath)
        {
            if (string.IsNullOrEmpty(requestPath))
                return false;

            var comparison = _config.CaseSensitiveMatching
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            // Check exact matches
            if (_config.AllowedRoutes.Any(route =>
                string.Equals(requestPath, route, comparison)))
            {
                return true;
            }

            // Check prefix matches
            if (_config.AllowedRoutePrefixes.Any(prefix =>
                requestPath.StartsWith(prefix, comparison)))
            {
                return true;
            }

            return false;
        }

        public bool IsAllowedRouteSegment(string requestPath)
        {
            if (string.IsNullOrEmpty(requestPath))
                return false;

            var comparison = _config.CaseSensitiveMatching
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            // Check if request path starts with any allowed route prefix
            return _config.AllowedRoutePrefixes.Any(prefix =>
                requestPath.StartsWith(prefix, comparison));
        }
    }
}