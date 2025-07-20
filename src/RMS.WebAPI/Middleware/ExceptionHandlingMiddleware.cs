using System.Net;
using System.Text.Json;
using RMS.Domain.Exceptions;
using RMS.Shared.Models;

namespace RMS.WebAPI.Middleware
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
                _logger.LogError(ex, "An unhandled exception occurred");
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
                    TraceId = context.TraceIdentifier
                },
                ValidationException validationEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = validationEx.Message,
                    Errors = validationEx.ValidationErrors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList(),
                    TraceId = context.TraceIdentifier,
                    Data = new { ValidationErrors = validationEx.ValidationErrors }
                },
                DomainException domainEx => new ApiResponse<object>
                {
                    Success = false,
                    Message = domainEx.Message,
                    TraceId = context.TraceIdentifier
                },
                _ => new ApiResponse<object>
                {
                    Success = false,
                    Message = "An internal server error occurred",
                    TraceId = context.TraceIdentifier
                }
            };

            context.Response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationException => (int)HttpStatusCode.BadRequest,
                DomainException => (int)HttpStatusCode.BadRequest,
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
