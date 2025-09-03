using System.Diagnostics;
using System.Text;
using System.Security.Claims;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Logging;
using SimRMS.Shared.Configuration;
using Microsoft.Extensions.Options;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Enhanced Request Logging Middleware
/// Author:      Md. Raihan Sharif
/// Purpose:     Professional Request Logging for API Calls with Debugging Support
/// Creation:    03/Aug/2025
/// Modified:    02/Sep/2025 - Enhanced with professional logging
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// Raihan       02/Sep/2025  Enhanced with structured logging and debugging support
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.WebAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly LoggingConfiguration _loggingConfig;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, 
            IOptions<LoggingConfiguration> loggingOptions)
        {
            _next = next;
            _logger = logger;
            _loggingConfig = loggingOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSkipLogging(context))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            // Get current user information directly from claims (set by TokenAuthenticationMiddleware)
            var userId = context.User?.FindFirstValue(ClaimTypes.Sid) ?? "Anonymous";
            var userName = context.User?.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
            var requestId = context.TraceIdentifier;
            var requestStartTime = DateTime.UtcNow;
            
            // Store start time for exception handler performance context
            context.Items["RequestStartTime"] = requestStartTime;
            
            // Extract minimal request details  
            var clientIP = GetClientIPAddress(context);

            // Add minimal enriched context for subsequent logging
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = requestId,
                ["ClientIP"] = clientIP,
                ["UserName"] = userName
            }))
            {
                // Only process request/response bodies if configured
                if (_loggingConfig.RequestResponse.LogRequestBodies || _loggingConfig.RequestResponse.LogResponseBodies)
                {
                    await LogRequestResponseWithBodiesAsync(context, requestId, clientIP, userName, stopwatch);
                }
                else
                {
                    // Efficient path - no body capturing
                    await _next(context);
                    stopwatch.Stop();
                    
                    // Log only what's configured
                    LogRequestResponseSummary(context, requestId, stopwatch.Elapsed);
                }
            }
        }

        private async Task LogRequestResponseWithBodiesAsync(HttpContext context, string requestId, string clientIP, string userName, Stopwatch stopwatch)
        {
            var originalBodyStream = context.Response.Body;
            
            try
            {
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Log request if configured
                if (_loggingConfig.RequestResponse.LogRequestBodies && ShouldLogRequestBody(context))
                {
                    var requestBody = await ReadRequestBodyAsync(context);
                    if (!string.IsNullOrEmpty(requestBody))
                    {
                        _logger.LogDebug("Request Body: {RequestBody}", requestBody);
                    }
                }

                await _next(context);
                stopwatch.Stop();

                // Log response if configured
                if (_loggingConfig.RequestResponse.LogResponseBodies && ShouldLogResponseBody(context))
                {
                    var responseBody = await ReadResponseBodyAsync(responseBodyStream);
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        _logger.LogDebug("Response Body: {ResponseBody}", responseBody);
                    }
                }

                LogRequestResponseSummary(context, requestId, stopwatch.Elapsed);
                
                // Copy response back to original stream
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
            catch (Exception)
            {
                stopwatch.Stop();
                context.Items["RequestDuration"] = stopwatch.ElapsedMilliseconds;
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private void LogRequestResponseSummary(HttpContext context, string requestId, TimeSpan duration)
        {
            var endpoint = $"{context.Request.Method} {context.Request.Path}";
            var statusCode = context.Response.StatusCode;
            var durationMs = duration.TotalMilliseconds;

            // Only log successful requests if configured
            if (statusCode >= 200 && statusCode < 300)
            {
                if (_loggingConfig.RequestResponse.LogSuccessfulRequests)
                {
                    _logger.LogInformation("✅ {Method} {Path} → {StatusCode} ({Duration:F1}ms)", 
                        context.Request.Method, context.Request.Path, statusCode, durationMs);
                }
            }
            else if (statusCode >= 400)
            {
                // Always log errors/warnings
                var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;
                _logger.Log(logLevel, "❌ {Method} {Path} → {StatusCode} ({Duration:F1}ms)", 
                    context.Request.Method, context.Request.Path, statusCode, durationMs);
            }

            // Log slow requests if configured
            if (_loggingConfig.Performance.Enabled && durationMs > _loggingConfig.RequestResponse.SlowRequestThresholdMs)
            {
                _logger.LogWarning("🐌 SLOW REQUEST: {Method} {Path} took {Duration:F1}ms (threshold: {Threshold}ms)", 
                    context.Request.Method, context.Request.Path, durationMs, _loggingConfig.RequestResponse.SlowRequestThresholdMs);
            }
        }


        private bool ShouldSkipLogging(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";
            
            // Check against configured excluded paths
            return _loggingConfig.RequestResponse.ExcludedPaths.Any(excludedPath => 
                path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase));
        }

        private bool ShouldLogRequestBody(HttpContext context)
        {
            var request = context.Request;
            
            // Don't log file uploads or very large requests
            if (request.ContentLength > _loggingConfig.RequestResponse.MaxBodySizeToLog)
                return false;
                
            // Only log for specific content types
            var contentType = request.ContentType?.ToLowerInvariant();
            return contentType != null && (
                contentType.Contains("application/json") ||
                contentType.Contains("application/xml") ||
                contentType.Contains("text/plain")
            );
        }

        private bool ShouldLogResponseBody(HttpContext context)
        {
            var response = context.Response;
            
            // Don't log large responses or file downloads
            if (response.ContentLength > _loggingConfig.RequestResponse.MaxBodySizeToLog)
                return false;
                
            // Only log for specific content types
            var contentType = response.ContentType?.ToLowerInvariant();
            return contentType != null && (
                contentType.Contains("application/json") ||
                contentType.Contains("application/xml") ||
                contentType.Contains("text/plain")
            );
        }

        private async Task<string?> ReadRequestBodyAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                var body = context.Request.Body;
                body.Position = 0;
                
                using var reader = new StreamReader(body, Encoding.UTF8, leaveOpen: true);
                var requestBody = await reader.ReadToEndAsync();
                body.Position = 0;
                
                return string.IsNullOrWhiteSpace(requestBody) ? null : requestBody;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to read request body");
                return null;
            }
        }

        private async Task<string?> ReadResponseBodyAsync(MemoryStream responseBodyStream)
        {
            try
            {
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
                var responseBody = await reader.ReadToEndAsync();
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                
                return string.IsNullOrWhiteSpace(responseBody) ? null : responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to read response body");
                return null;
            }
        }

        private static string GetClientIPAddress(HttpContext context)
        {
            // Check for forwarded IP addresses (common in load balancer/proxy scenarios)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (ips.Length > 0)
                    return ips[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
                return realIp;

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

    }
}
