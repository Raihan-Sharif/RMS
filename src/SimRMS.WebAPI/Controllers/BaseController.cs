using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;

namespace SimRMS.WebAPI.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        protected BaseController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        protected string GetDynamicRoute()
        {
            var isVersioningEnabled = _configurationService.IsApiVersioningEnabled;
            var defaultVersion = _configurationService.GetApiVersion();

            if (isVersioningEnabled)
            {
                return $"api/v{defaultVersion}/[controller]";
            }

            return "api/[controller]";
        }

        protected ActionResult<ApiResponse<T>> Ok<T>(T data, string message = "Success")
        {
            var response = ApiResponse<T>.SuccessResult(data, message);
            response.TraceId = HttpContext.TraceIdentifier;
            response.Version = _configurationService.GetApiVersion();
            return base.Ok(response);
        }

        protected ActionResult<ApiResponse<T>> BadRequest<T>(string message, List<string>? errors = null)
        {
            var response = ApiResponse<T>.ErrorResult(message, errors);
            response.TraceId = HttpContext.TraceIdentifier;
            response.Version = _configurationService.GetApiVersion();
            return base.BadRequest(response);
        }

        protected ActionResult<ApiResponse<T>> NotFound<T>(string message = "Resource not found")
        {
            var response = ApiResponse<T>.ErrorResult(message);
            response.TraceId = HttpContext.TraceIdentifier;
            response.Version = _configurationService.GetApiVersion();
            return base.NotFound(response);
        }

        protected ActionResult<ApiResponse<T>> Unauthorized<T>(string message = "Unauthorized")
        {
            var response = ApiResponse<T>.ErrorResult(message);
            response.TraceId = HttpContext.TraceIdentifier;
            response.Version = _configurationService.GetApiVersion();
            return base.Unauthorized(response);
        }

        protected ActionResult<ApiResponse<T>> Forbidden<T>(string message = "Forbidden")
        {
            var response = ApiResponse<T>.ErrorResult(message);
            response.TraceId = HttpContext.TraceIdentifier;
            response.Version = _configurationService.GetApiVersion();
            return base.StatusCode(403, response);
        }
    }
}
