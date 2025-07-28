namespace SimRMS.Application.Interfaces
{
    public interface ISecurityRouteService
    {
        bool IsAllowedRoute(string requestPath);
        bool IsAllowedRouteSegment(string requestPath);
        void RefreshRoutes();
        List<string> GetAllowedRoutes();
        List<string> GetAllowedRoutePrefixes();
        void AddAllowedRoute(string route);
        void RemoveAllowedRoute(string route);
    }
}