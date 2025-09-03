/// <summary>
/// <para>
/// ===================================================================
/// Title:       Cache Service
/// Author:      Md. Raihan Sharif
/// Purpose:     this config class manage the LoggingSettings.
/// Creation:    02/Sep/2025
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

namespace SimRMS.Shared.Configuration;

/// <summary>
/// Enterprise-grade logging configuration for controlling what gets logged
/// Designed to minimize log size while maintaining essential debugging information
/// Matches appsettings.json LoggingSettings section exactly
/// </summary>
public class LoggingConfiguration
{
    public const string SectionName = "LoggingSettings";

    /// <summary>
    /// Request/Response logging settings
    /// </summary>
    public RequestResponseLogging RequestResponse { get; set; } = new();

    /// <summary>
    /// Performance logging settings
    /// </summary>
    public PerformanceLogging Performance { get; set; } = new();

    /// <summary>
    /// Security logging settings
    /// </summary>
    public SecurityLogging Security { get; set; } = new();

    /// <summary>
    /// Database logging settings
    /// </summary>
    public DatabaseLogging Database { get; set; } = new();

    /// <summary>
    /// Framework logging filters
    /// </summary>
    public FrameworkLogging Framework { get; set; } = new();
}

/// <summary>
/// Request/Response logging configuration - matches appsettings.json exactly
/// </summary>
public class RequestResponseLogging
{
    /// <summary>
    /// Log successful API requests (default: false in production)
    /// </summary>
    public bool LogSuccessfulRequests { get; set; } = false;

    /// <summary>
    /// Log request bodies (default: false for security/size)
    /// </summary>
    public bool LogRequestBodies { get; set; } = false;

    /// <summary>
    /// Log response bodies (default: false for security/size)
    /// </summary>
    public bool LogResponseBodies { get; set; } = false;

    /// <summary>
    /// Log request/response headers (default: false for security)
    /// </summary>
    public bool LogHeaders { get; set; } = false;

    /// <summary>
    /// Maximum request body size to log (bytes)
    /// </summary>
    public int MaxBodySizeToLog { get; set; } = 512; // Match appsettings: 512

    /// <summary>
    /// Log only slow requests above this threshold (milliseconds)
    /// </summary>
    public int SlowRequestThresholdMs { get; set; } = 2000; // Match appsettings: 2000

    /// <summary>
    /// Paths to exclude from any logging
    /// </summary>
    public List<string> ExcludedPaths { get; set; } = new()
    {
        "/health", "/healthcheck", "/favicon.ico", "/robots.txt",
        "/metrics", "/swagger", "/_framework", "/_vs/browserLink"
    };
}

/// <summary>
/// Performance logging configuration - matches appsettings.json exactly
/// </summary>
public class PerformanceLogging
{
    /// <summary>
    /// Enable performance logging (default: true)
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Log database operation performance (default: false)
    /// </summary>
    public bool LogDatabaseOperations { get; set; } = false; // Match appsettings: false

    /// <summary>
    /// Threshold for slow database operations (milliseconds)
    /// </summary>
    public int SlowDatabaseThresholdMs { get; set; } = 1000; // Match appsettings: 1000

    /// <summary>
    /// Log external service call performance (default: true)
    /// </summary>
    public bool LogExternalServiceCalls { get; set; } = true;
}

/// <summary>
/// Security logging configuration - matches appsettings.json exactly
/// </summary>
public class SecurityLogging
{
    /// <summary>
    /// Always log authentication events (cannot be disabled)
    /// </summary>
    public bool LogAuthenticationEvents { get; set; } = true;

    /// <summary>
    /// Log authorization failures (default: true)
    /// </summary>
    public bool LogAuthorizationFailures { get; set; } = true;

    /// <summary>
    /// Log validation failures (default: false, can be noisy)
    /// </summary>
    public bool LogValidationErrors { get; set; } = false;

    /// <summary>
    /// Log suspicious activity (multiple failed attempts, etc.)
    /// </summary>
    public bool LogSuspiciousActivity { get; set; } = true;
}

/// <summary>
/// Database logging configuration - matches appsettings.json exactly
/// </summary>
public class DatabaseLogging
{
    /// <summary>
    /// Log successful database operations (default: false, only errors)
    /// </summary>
    public bool LogSuccessfulOperations { get; set; } = false;

    /// <summary>
    /// Always log database errors (cannot be disabled)
    /// </summary>
    public bool LogDatabaseErrors { get; set; } = true;

    /// <summary>
    /// Log stored procedure parameters (default: false for security)
    /// </summary>
    public bool LogParameters { get; set; } = false;
}

/// <summary>
/// Framework logging configuration - matches appsettings.json exactly
/// </summary>
public class FrameworkLogging
{
    /// <summary>
    /// Suppress noisy Microsoft framework logs (default: true)
    /// </summary>
    public bool SuppressFrameworkNoise { get; set; } = true;

    /// <summary>
    /// Log Entity Framework queries (if using EF - default: false)
    /// </summary>
    public bool LogEntityFrameworkQueries { get; set; } = false;

    /// <summary>
    /// Log ASP.NET Core request pipeline details (default: false)
    /// </summary>
    public bool LogAspNetCorePipeline { get; set; } = false;

    /// <summary>
    /// Log HTTP client requests (default: false, can be very noisy)
    /// </summary>
    public bool LogHttpClientRequests { get; set; } = false;

    /// <summary>
    /// Log hosting lifetime events (startup/shutdown - default: true)
    /// </summary>
    public bool LogHostingEvents { get; set; } = true;
}

/// <summary>
/// Environment-specific presets for logging configuration
/// </summary>
public static class LoggingPresets
{
    /// <summary>
    /// Production preset - minimal logging for performance
    /// </summary>
    public static LoggingConfiguration Production => new()
    {
        RequestResponse = new RequestResponseLogging
        {
            LogSuccessfulRequests = false,
            LogRequestBodies = false,
            LogResponseBodies = false,
            LogHeaders = false,
            SlowRequestThresholdMs = 2000,
            MaxBodySizeToLog = 0 // No body logging in production
        },
        Performance = new PerformanceLogging
        {
            Enabled = true,
            LogDatabaseOperations = false,
            SlowDatabaseThresholdMs = 1000,
            LogExternalServiceCalls = true
        },
        Security = new SecurityLogging
        {
            LogAuthenticationEvents = true,
            LogAuthorizationFailures = true,
            LogValidationErrors = false,
            LogSuspiciousActivity = true
        },
        Database = new DatabaseLogging
        {
            LogSuccessfulOperations = false,
            LogDatabaseErrors = true,
            LogParameters = false
        },
        Framework = new FrameworkLogging
        {
            SuppressFrameworkNoise = true,
            LogEntityFrameworkQueries = false,
            LogAspNetCorePipeline = false,
            LogHttpClientRequests = false,
            LogHostingEvents = true
        }
    };

    /// <summary>
    /// Development preset - detailed logging for debugging
    /// </summary>
    public static LoggingConfiguration Development => new()
    {
        RequestResponse = new RequestResponseLogging
        {
            LogSuccessfulRequests = true,
            LogRequestBodies = true,
            LogResponseBodies = true,
            LogHeaders = true,
            SlowRequestThresholdMs = 1000,
            MaxBodySizeToLog = 2048
        },
        Performance = new PerformanceLogging
        {
            Enabled = true,
            LogDatabaseOperations = true,
            SlowDatabaseThresholdMs = 500,
            LogExternalServiceCalls = true
        },
        Security = new SecurityLogging
        {
            LogAuthenticationEvents = true,
            LogAuthorizationFailures = true,
            LogValidationErrors = true,
            LogSuspiciousActivity = true
        },
        Database = new DatabaseLogging
        {
            LogSuccessfulOperations = true,
            LogDatabaseErrors = true,
            LogParameters = true
        },
        Framework = new FrameworkLogging
        {
            SuppressFrameworkNoise = false,
            LogEntityFrameworkQueries = true,
            LogAspNetCorePipeline = true,
            LogHttpClientRequests = true,
            LogHostingEvents = true
        }
    };

    /// <summary>
    /// Debug preset - maximum logging for troubleshooting
    /// </summary>
    public static LoggingConfiguration Debug => new()
    {
        RequestResponse = new RequestResponseLogging
        {
            LogSuccessfulRequests = true,
            LogRequestBodies = true,
            LogResponseBodies = true,
            LogHeaders = true,
            SlowRequestThresholdMs = 100,
            MaxBodySizeToLog = 10240 // 10KB for debugging
        },
        Performance = new PerformanceLogging
        {
            Enabled = true,
            LogDatabaseOperations = true,
            SlowDatabaseThresholdMs = 100,
            LogExternalServiceCalls = true
        },
        Security = new SecurityLogging
        {
            LogAuthenticationEvents = true,
            LogAuthorizationFailures = true,
            LogValidationErrors = true,
            LogSuspiciousActivity = true
        },
        Database = new DatabaseLogging
        {
            LogSuccessfulOperations = true,
            LogDatabaseErrors = true,
            LogParameters = true
        },
        Framework = new FrameworkLogging
        {
            SuppressFrameworkNoise = false,
            LogEntityFrameworkQueries = true,
            LogAspNetCorePipeline = true,
            LogHttpClientRequests = true,
            LogHostingEvents = true
        }
    };
}