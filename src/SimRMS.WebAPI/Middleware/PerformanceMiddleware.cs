using System.Diagnostics;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Performance Middleware
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Performance Monitoring for API Requests
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

namespace SimRMS.WebAPI.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;
        private const int SlowRequestThresholdMs = 2000;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path.Value;
            var requestMethod = context.Request.Method;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                if (elapsedMs > SlowRequestThresholdMs)
                {
                    _logger.LogWarning("SLOW REQUEST: {Method} {Path} took {ElapsedMs}ms",
                        requestMethod, requestPath, elapsedMs);
                }

                // FIXED: Only add headers if response hasn't started
                if (!context.Response.HasStarted)
                {
                    try
                    {
                        context.Response.Headers.TryAdd("X-Response-Time", $"{elapsedMs}ms");
                    }
                    catch (InvalidOperationException)
                    {
                        // Headers already sent - just log it
                        _logger.LogDebug("Could not add performance header - response already started");
                    }
                }
            }
        }
    }
}