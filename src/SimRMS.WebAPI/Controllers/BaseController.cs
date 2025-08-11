using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Models;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Base Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Facilitate Common Functionality for All Controllers through Inheritance in ASP.NET Core Web API
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

        // For single objects
        protected ActionResult<ApiResponse<T>> Ok<T>(T data, string message = "Success")
        {
            var response = ApiResponse<T>.SuccessResult(data, message);
            response.TraceId = HttpContext.TraceIdentifier;
            response.Version = _configurationService.GetApiVersion();
            return base.Ok(response);
        }

        // For paged results - converts PagedResult to standardized ApiResponse
        protected ActionResult<ApiResponse<IEnumerable<T>>> Ok<T>(PagedResult<T> pagedResult, string message = "Success")
        {
            var pagination = new PaginationInfo
            {
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasPreviousPage = pagedResult.HasPreviousPage,
                HasNextPage = pagedResult.HasNextPage
            };

            var response = ApiResponse<IEnumerable<T>>.SuccessResult(pagedResult.Data, message, pagination);
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