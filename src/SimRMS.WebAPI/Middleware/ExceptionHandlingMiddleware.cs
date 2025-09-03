using System.Net;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using SimRMS.Domain.Exceptions;
using SimRMS.Shared.Models;
using SimRMS.Shared.Logging;
using SimRMS.Shared.Configuration;
using Microsoft.AspNetCore.Authentication;
using SimRMS.Application.Interfaces;
using Microsoft.Extensions.Options;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Exception Handling Middleware
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Global Exception Handling for API to Provide Consistent Error Responses
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

namespace SimRMS.WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly LoggingConfiguration _loggingConfig;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, 
            IOptions<LoggingConfiguration> loggingOptions)
        {
            _next = next;
            _logger = logger;
            _loggingConfig = loggingOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Professional exception logging with full context
                await LogExceptionWithContextAsync(context, ex);
                
                // Don't handle the exception if the response has already started
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response has already started, cannot modify response for {RequestId}. Exception: {ExceptionType}", 
                        context.TraceIdentifier, ex.GetType().Name);
                    throw;
                }
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task LogExceptionWithContextAsync(HttpContext context, Exception exception)
        {
            var requestId = context.TraceIdentifier;
            
            // Get current user information directly from claims (set by TokenAuthenticationMiddleware)
            var userId = context.User?.FindFirstValue(ClaimTypes.Sid) ?? "Anonymous";
            var userName = context.User?.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
            
            var clientIP = GetClientIPAddress(context);
            var endpoint = $"{context.Request.Method} {context.Request.Path}";

            // Log based on exception severity and type with configuration control
            switch (exception)
            {
                // File operation exceptions
                case FileSizeExceededException fileSizeEx:
                case InvalidFileTypeException fileTypeEx:
                case FileOperationException fileOpEx:
                    _logger.LogWarning("📁 FILE ERROR: {ExceptionType} in {Endpoint} | User: {UserId}", 
                        exception.GetType().Name, endpoint, userId);
                    break;

                case NotFoundException notFoundEx:
                    _logger.LogWarning("🔍 NOT FOUND: {Resource} in {Endpoint} | User: {UserId}", 
                        notFoundEx.Message, endpoint, userId);
                    break;

                // Authentication/Authorization - Always log security events
                case UnauthorizedAccessException:
                case InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("authenticationScheme"):
                    if (_loggingConfig.Security.LogAuthenticationEvents)
                    {
                        _logger.LogSecurityEvent("Unauthorized Access", userId, clientIP, null, false, endpoint);
                    }
                    break;

                // Business/Domain exceptions
                case ValidationException validationEx:
                    if (_loggingConfig.Security.LogValidationErrors)
                    {
                        _logger.LogWarning("🔧 VALIDATION ERROR: {Message} in {Endpoint} | User: {UserId}", 
                            validationEx.Message, endpoint, userId);
                    }
                    break;

                case DomainException domainEx:
                    _logger.LogWarning("🔧 DOMAIN ERROR: {Message} in {Endpoint} | User: {UserId}", 
                        domainEx.Message, endpoint, userId);
                    break;

                // Database exceptions - Always log
                case SqlException sqlEx:
                    _logger.LogError("🗄️ DATABASE ERROR: SQL {ErrorNumber} in {Endpoint} | User: {UserId} | Message: {Message}", 
                        sqlEx.Number, endpoint, userId, sqlEx.Message);
                    break;

                // External service failures
                case HttpRequestException:
                case TimeoutException:
                    _logger.LogError("🌐 EXTERNAL SERVICE ERROR: {ExceptionType} in {Endpoint} | User: {UserId}", 
                        exception.GetType().Name, endpoint, userId);
                    break;

                // System/Critical errors - Always log with full details
                default:
                    _logger.LogError(exception, "🚨 SYSTEM ERROR: {ExceptionType} in {Endpoint} | User: {UserId} | Message: {Message}", 
                        exception.GetType().Name, endpoint, userId, exception.Message);
                    break;
            }

            // Log performance context if available and configured
            if (_loggingConfig.Performance.Enabled && 
                context.Items.TryGetValue("RequestStartTime", out var startTimeObj) && 
                startTimeObj is DateTime startTime)
            {
                var duration = DateTime.UtcNow - startTime;
                _logger.LogWarning("⏱️ Exception after {Duration:F1}ms in {Endpoint}", duration.TotalMilliseconds, endpoint);
            }
            
            return Task.CompletedTask;
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            // Set proper WWW-Authenticate header for authentication errors
            if (IsAuthenticationException(exception))
            {
                context.Response.Headers["WWW-Authenticate"] = "Bearer";
            }

            var response = exception switch
            {
                NotFoundException notFoundEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = notFoundEx.Message,
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                FileSizeExceededException fileSizeEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = fileSizeEx.Message,
                    Data = null,
                    Errors = new List<string> { $"Max allowed: {fileSizeEx.MaxAllowedSize:N0} bytes", $"Actual: {fileSizeEx.ActualSize:N0} bytes" },
                    TraceId = context.TraceIdentifier
                },
                InvalidFileTypeException fileTypeEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = fileTypeEx.Message,
                    Data = null,
                    Errors = new List<string> { $"File extension: {fileTypeEx.FileExtension}", $"Allowed: {string.Join(", ", fileTypeEx.AllowedExtensions)}" },
                    TraceId = context.TraceIdentifier
                },
                FileOperationException fileOpEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = fileOpEx.Message,
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                ValidationException validationEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed: "+ validationEx.Message,
                    Data = null,
                    Errors = validationEx.ValidationErrors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList(),
                    TraceId = context.TraceIdentifier
                },
                DomainException domainEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = domainEx.Message,
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                // ADD: Authentication-specific errors
                InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("authenticationScheme") || 
                                                            invalidOpEx.Message.Contains("DefaultChallengeScheme") => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Authentication required. Please provide a valid authorization token. " +invalidOpEx.Message,
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                UnauthorizedAccessException unauthorizedEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Access denied. You do not have permission to access this resource.",
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                TimeoutException timeoutEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Request timeout - please try again later",
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                HttpRequestException httpEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "External service unavailable",
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                SqlException sqlEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Database operation failed:"+ sqlEx.Message,
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                ArgumentException argEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid request parameter:"+argEx.Message,
                    Data = null,
                    Errors = new List<string> { argEx.ParamName ?? "Unknown parameter" },
                    TraceId = context.TraceIdentifier
                },
                InvalidOperationException invalidOpEx2 when !invalidOpEx2.Message.Contains("authenticationScheme") && !invalidOpEx2.Message.Contains("DefaultChallengeScheme") => new ApiResponse<object>
                {
                    Success = false,
                    Message = "An operation error occurred: " + invalidOpEx2.Message,
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                _ => new ApiResponse<object>
                {
                    Success = false,
                    Message = "An internal server error occurred",
                    Data = null,
                    TraceId = context.TraceIdentifier
                }
            };

            context.Response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                FileSizeExceededException => (int)HttpStatusCode.BadRequest,
                InvalidFileTypeException => (int)HttpStatusCode.BadRequest,
                FileOperationException => (int)HttpStatusCode.InternalServerError,
                ValidationException => (int)HttpStatusCode.BadRequest,
                DomainException => (int)HttpStatusCode.BadRequest,
                InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("authenticationScheme") || 
                                                            invalidOpEx.Message.Contains("DefaultChallengeScheme") => (int)HttpStatusCode.Unauthorized,
                UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                HttpRequestException => (int)HttpStatusCode.ServiceUnavailable,
                SqlException => (int)HttpStatusCode.InternalServerError,
                InvalidOperationException => (int)HttpStatusCode.InternalServerError, // Handle other InvalidOperationException cases
                _ => (int)HttpStatusCode.InternalServerError
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(jsonResponse);
        }
        
        private static bool IsAuthenticationException(Exception exception)
        {
            return exception switch
            {
                InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("authenticationScheme") || 
                                                            invalidOpEx.Message.Contains("DefaultChallengeScheme") => true,
                UnauthorizedAccessException => true,
                _ => false
            };
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