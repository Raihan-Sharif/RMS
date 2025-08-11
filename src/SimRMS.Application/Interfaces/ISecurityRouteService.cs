/// <summary>
/// <para>
/// ===================================================================
/// Title:       ISecurityRouteService Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines methods for managing security routes in the application.
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