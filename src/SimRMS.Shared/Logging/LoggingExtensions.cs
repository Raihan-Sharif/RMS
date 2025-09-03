using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SimRMS.Shared.Logging;

/// <summary>
/// Professional logging extensions for comprehensive debugging and monitoring
/// Provides structured logging methods for performance tracking, security auditing, and debugging
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Logs method entry with parameters for debugging
    /// </summary>
    public static IDisposable LogMethodEntry(this ILogger logger, 
        object? parameters = null,
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        var stopwatch = Stopwatch.StartNew();
        
        logger.LogDebug("→ Entering {ClassName}.{MethodName}() at line {LineNumber} with parameters: {@Parameters}", 
            className, methodName, lineNumber, parameters);
            
        return new MethodScope(logger, className, methodName, stopwatch);
    }

    /// <summary>
    /// Logs performance metrics for operations
    /// </summary>
    public static void LogPerformance(this ILogger logger, 
        string operation, 
        TimeSpan duration, 
        object? additionalData = null,
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "")
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        
        logger.LogInformation("⏱️ PERFORMANCE: {Operation} in {ClassName}.{MethodName}() took {Duration}ms {@AdditionalData}",
            operation, className, methodName, duration.TotalMilliseconds, additionalData);
    }

    /// <summary>
    /// Logs database operation details with configuration control
    /// </summary>
    public static void LogDatabaseOperation(this ILogger logger,
        string storedProcedure,
        TimeSpan duration,
        bool logSuccessful = false,
        bool logParameters = false,
        int slowThresholdMs = 1000,
        object? parameters = null,
        int? recordsAffected = null,
        [CallerMemberName] string methodName = "")
    {
        var durationMs = duration.TotalMilliseconds;
        
        // Only log slow operations or if configured to log all
        if (logSuccessful || durationMs > slowThresholdMs)
        {
            var logLevel = durationMs > slowThresholdMs ? LogLevel.Warning : LogLevel.Information;
            var icon = durationMs > slowThresholdMs ? "🐌" : "🗄️";
            
            if (logParameters && parameters != null)
            {
                logger.Log(logLevel, "{Icon} DATABASE: {StoredProcedure} ({Duration:F1}ms) | Records: {Records} | Parameters: {@Parameters}", 
                    icon, storedProcedure, durationMs, recordsAffected ?? 0, parameters);
            }
            else
            {
                logger.Log(logLevel, "{Icon} DATABASE: {StoredProcedure} ({Duration:F1}ms) | Records: {Records}", 
                    icon, storedProcedure, durationMs, recordsAffected ?? 0);
            }
        }
    }

    /// <summary>
    /// Logs authentication and authorization events for security auditing
    /// </summary>
    public static void LogSecurityEvent(this ILogger logger,
        string eventType,
        string userId,
        string? ipAddress = null,
        string? userAgent = null,
        bool success = true,
        string? additionalInfo = null)
    {
        var logLevel = success ? LogLevel.Information : LogLevel.Warning;
        var icon = success ? "✅" : "⚠️";
        
        logger.Log(logLevel, "{Icon} SECURITY: {EventType} | User: {UserId} | IP: {IPAddress} | Success: {Success} | UserAgent: {UserAgent} | Info: {AdditionalInfo}",
            icon, eventType, userId, ipAddress ?? "Unknown", success, userAgent ?? "Unknown", additionalInfo ?? "None");
    }

    /// <summary>
    /// Logs business validation errors with context
    /// </summary>
    public static void LogValidationError(this ILogger logger,
        string validationRule,
        object? inputData = null,
        string? userId = null,
        [CallerMemberName] string methodName = "")
    {
        logger.LogWarning("❌ VALIDATION: Rule '{ValidationRule}' failed in {MethodName}() | User: {UserId} | Input: {@InputData}",
            validationRule, methodName, userId ?? "Anonymous", inputData);
    }

    /// <summary>
    /// Logs API request/response for debugging
    /// </summary>
    public static void LogApiRequest(this ILogger logger,
        string method,
        string endpoint,
        string? userId = null,
        string? requestId = null,
        object? requestBody = null,
        string? ipAddress = null)
    {
        logger.LogInformation("🌐 API REQUEST: {Method} {Endpoint} | User: {UserId} | RequestId: {RequestId} | IP: {IPAddress} | Body: {@RequestBody}",
            method, endpoint, userId ?? "Anonymous", requestId ?? "N/A", ipAddress ?? "Unknown", requestBody);
    }

    /// <summary>
    /// Logs API response for debugging
    /// </summary>
    public static void LogApiResponse(this ILogger logger,
        string method,
        string endpoint,
        int statusCode,
        TimeSpan duration,
        string? requestId = null,
        object? responseBody = null)
    {
        var logLevel = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        var icon = statusCode >= 400 ? "❌" : "✅";
        
        logger.Log(logLevel, "{Icon} API RESPONSE: {Method} {Endpoint} | Status: {StatusCode} | Duration: {Duration}ms | RequestId: {RequestId} | Body: {@ResponseBody}",
            icon, method, endpoint, statusCode, duration.TotalMilliseconds, requestId ?? "N/A", responseBody);
    }

    /// <summary>
    /// Logs critical business errors that need immediate attention
    /// </summary>
    public static void LogCriticalError(this ILogger logger,
        Exception exception,
        string context,
        object? additionalData = null,
        string? userId = null,
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "")
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        
        logger.LogCritical(exception, "🚨 CRITICAL ERROR in {ClassName}.{MethodName}(): {Context} | User: {UserId} | Data: {@AdditionalData} | Exception: {ExceptionType} | Message: {ExceptionMessage}",
            className, methodName, context, userId ?? "System", additionalData, exception.GetType().Name, exception.Message);
    }

    /// <summary>
    /// Logs cache operations for debugging
    /// </summary>
    public static void LogCacheOperation(this ILogger logger,
        string operation,
        string cacheKey,
        bool hit = false,
        TimeSpan? duration = null,
        [CallerMemberName] string methodName = "")
    {
        var icon = operation.ToLower() switch
        {
            "hit" => "🎯",
            "miss" => "❌",
            "set" => "💾",
            "remove" => "🗑️",
            _ => "📦"
        };

        logger.LogDebug("{Icon} CACHE {Operation}: Key '{CacheKey}' in {MethodName}() | Hit: {Hit} | Duration: {Duration}ms",
            icon, operation.ToUpper(), cacheKey, methodName, hit, duration?.TotalMilliseconds ?? 0);
    }

    private class MethodScope : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _className;
        private readonly string _methodName;
        private readonly Stopwatch _stopwatch;
        private bool _disposed;

        public MethodScope(ILogger logger, string className, string methodName, Stopwatch stopwatch)
        {
            _logger = logger;
            _className = className;
            _methodName = methodName;
            _stopwatch = stopwatch;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _stopwatch.Stop();
                _logger.LogDebug("← Exiting {ClassName}.{MethodName}() after {Duration}ms", 
                    _className, _methodName, _stopwatch.ElapsedMilliseconds);
                _disposed = true;
            }
        }
    }
}

/// <summary>
/// Common log event IDs for structured logging and monitoring
/// </summary>
public static class LogEvents
{
    // System Events (1000-1999)
    public const int ApplicationStarted = 1000;
    public const int ApplicationStopping = 1001;
    public const int ConfigurationLoaded = 1002;
    
    // Authentication Events (2000-2999)
    public const int LoginSuccessful = 2000;
    public const int LoginFailed = 2001;
    public const int TokenValidated = 2002;
    public const int TokenExpired = 2003;
    public const int UnauthorizedAccess = 2004;
    
    // Database Events (3000-3999)
    public const int DatabaseConnectionOpened = 3000;
    public const int DatabaseConnectionFailed = 3001;
    public const int StoredProcedureExecuted = 3002;
    public const int DatabaseTimeout = 3003;
    public const int DatabaseError = 3004;
    
    // API Events (4000-4999)
    public const int ApiRequestReceived = 4000;
    public const int ApiRequestCompleted = 4001;
    public const int ApiValidationFailed = 4002;
    public const int ApiRateLimitExceeded = 4003;
    
    // Business Logic Events (5000-5999)
    public const int BusinessRuleValidationFailed = 5000;
    public const int WorkflowCompleted = 5001;
    public const int DataProcessed = 5002;
    
    // Error Events (9000-9999)
    public const int UnhandledException = 9000;
    public const int CriticalError = 9001;
    public const int SecurityViolation = 9002;
}