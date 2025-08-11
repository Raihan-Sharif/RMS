/// <summary>
/// <para>
/// ===================================================================
/// Title:       AppConstants
/// Author:      Md. Raihan Sharif
/// Purpose:     Fixed Constants for Application Configuration
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

namespace SimRMS.Shared.Constants
{
    public static class AppConstants
    {
        public const string DefaultConnectionString = "DefaultConnection";
        public const string TokenCacheKey = "USER_TOKEN_";
        public const string UserSessionCacheKey = "USER_SESSION_";
        public const string PermissionsCacheKey = "USER_PERMISSIONS_";

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string RiskAnalyst = "RiskAnalyst";
            public const string RiskManager = "RiskManager";
            public const string SuperAdmin = "SuperAdmin";
            public const string User = "User";
        }

        public static class Permissions
        {
            public const string ViewRisks = "ViewRisks";
            public const string CreateRisks = "CreateRisks";
            public const string UpdateRisks = "UpdateRisks";
            public const string DeleteRisks = "DeleteRisks";
            public const string ViewUsers = "ViewUsers";
            public const string ManageUsers = "ManageUsers";
            public const string ViewReports = "ViewReports";
            public const string ManageSystem = "ManageSystem";
        }

        public static class CacheKeys
        {
            public const string ApiVersion = "API_VERSION";
            public const string DefaultApiVersion = "1.0";
        }

        public static class Headers
        {
            public const string Authorization = "Authorization";
            public const string ApiKey = "X-API-Key";
            public const string TraceId = "X-Trace-Id";
            public const string ApiVersion = "X-API-Version";
        }
    }
}
