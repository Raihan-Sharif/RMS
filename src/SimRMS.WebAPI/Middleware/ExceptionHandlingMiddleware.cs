using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using SimRMS.Domain.Exceptions;
using SimRMS.Shared.Models;

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

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

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
                    Message = "Validation failed",
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
                // ADD: More specific error types
                UnauthorizedAccessException unauthorizedEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Access denied",
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
                    Message = "Database operation failed",
                    Data = null,
                    TraceId = context.TraceIdentifier
                },
                ArgumentException argEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid request parameter",
                    Data = null,
                    Errors = new List<string> { argEx.ParamName ?? "Unknown parameter" },
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
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                HttpRequestException => (int)HttpStatusCode.ServiceUnavailable,
                SqlException => (int)HttpStatusCode.InternalServerError,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}