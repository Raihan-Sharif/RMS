using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace SimRMS.WebAPI.Controllers.V1
{
    /// <summary>
    /// User Information Management API
    /// Shows both simple and complex query validation approaches
    /// Uses single request model for Create/Update operations
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize]
    public class UsrInfoController : BaseController
    {
        private readonly IUsrInfoService _usrInfoService;
        private readonly IValidator<GetUsersQuery> _getUsersQueryValidator;
        private readonly ILogger<UsrInfoController> _logger;

        public UsrInfoController(
            IUsrInfoService usrInfoService,
            IValidator<GetUsersQuery> getUsersQueryValidator,
            IConfigurationService configurationService,
            ILogger<UsrInfoController> logger)
            : base(configurationService)
        {
            _usrInfoService = usrInfoService;
            _getUsersQueryValidator = getUsersQueryValidator;
            _logger = logger;
        }

        /// <summary>
        /// APPROACH A: Simple Query Validation (Controller-level)
        /// Get paginated list of users with basic query parameters
        /// Validation done directly in controller
        /// </summary>
        [HttpGet("simple")]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsrInfoDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsrInfoDto>>>> GetUsersSimple(
            [FromQuery][Range(1, 1000)] int pageNumber = 1,
            [FromQuery][Range(1, 100)] int pageSize = 10,
            [FromQuery] string? usrStatus = null,
            [FromQuery][StringLength(10)] string? coCode = null,
            [FromQuery][StringLength(10)] string? dlrCode = null,
            [FromQuery][StringLength(10)] string? rmsType = null,
            [FromQuery][StringLength(100, MinimumLength = 2)] string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting users (simple) - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            // Additional controller-level validation
            if (!string.IsNullOrEmpty(usrStatus) && !new[] { "A", "S", "C" }.Contains(usrStatus))
            {
                return BadRequest<IEnumerable<UsrInfoDto>>("User status must be A (Active), S (Suspend), or C (Close)");
            }

            try
            {
                var result = await _usrInfoService.GetUsersAsync(
                    pageNumber, pageSize, usrStatus, coCode, dlrCode, rmsType, searchTerm, cancellationToken);

                return Ok(result, $"Retrieved {result.Data.Count()} records out of {result.TotalCount} total");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users list (simple)");
                throw;
            }
        }

        /// <summary>
        /// APPROACH B: Complex Query Validation (FluentValidation)
        /// Get paginated list of users with complex query object
        /// Validation done using FluentValidation
        /// </summary>
        [HttpGet("advanced")]
        [MapToApiVersion("2.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsrInfoDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsrInfoDto>>>> GetUsersAdvanced(
            [FromQuery] GetUsersQuery query,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting users (advanced) - Page: {PageNumber}, Size: {PageSize}", query.PageNumber, query.PageSize);

            // Validate complex query using FluentValidation
            var validationResult = await _getUsersQueryValidator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
                return BadRequest<IEnumerable<UsrInfoDto>>("Invalid query parameters", errors);
            }

            try
            {
                var result = await _usrInfoService.GetUsersAsync(
                    query.PageNumber, query.PageSize, query.UsrStatus, query.CoCode,
                    query.DlrCode, query.RmsType, query.SearchTerm, cancellationToken);

                return Ok(result, $"Retrieved {result.Data.Count()} records out of {result.TotalCount} total");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users list (advanced)");
                throw;
            }
        }

        /// <summary>
        /// Standard GET endpoint (uses simple approach)
        /// </summary>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsrInfoDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsrInfoDto>>>> GetUsrInfos(
            [FromQuery][Range(1, 1000)] int pageNumber = 1,
            [FromQuery][Range(1, 100)] int pageSize = 10,
            [FromQuery] string? usrStatus = null,
            [FromQuery][StringLength(10)] string? coCode = null,
            [FromQuery][StringLength(10)] string? dlrCode = null,
            [FromQuery][StringLength(10)] string? rmsType = null,
            [FromQuery][StringLength(100, MinimumLength = 2)] string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            // Simple validation approach (same as GetUsersSimple)
            return await GetUsersSimple(pageNumber, pageSize, usrStatus, coCode, dlrCode, rmsType, searchTerm, cancellationToken);
        }

        /// <summary>
        /// Get user information by ID
        /// </summary>
        [HttpGet("{usrId}")]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<UsrInfoDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> GetUsrInfo(
            [Required][StringLength(50, MinimumLength = 1)] string usrId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
            {
                return BadRequest<UsrInfoDto>("User ID is required");
            }

            _logger.LogInformation("Getting UsrInfo by ID: {UsrId}", usrId);

            try
            {
                var result = await _usrInfoService.GetUserByIdAsync(usrId, cancellationToken);
                return Ok(result, "User information retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UsrId}", usrId);
                throw;
            }
        }

        /// <summary>
        /// Create new user information
        /// Uses single request model with validation
        /// </summary>
        [HttpPost]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<UsrInfoDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> CreateUsrInfo(
            [FromBody] UsrInfoRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                return BadRequest<UsrInfoDto>("Request body is required");
            }

            _logger.LogInformation("Creating new UsrInfo: {UsrId}", request.UsrId);

            try
            {
                var result = await _usrInfoService.CreateUserAsync(request, cancellationToken);
                return Ok(result, "User information created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {UsrId}", request.UsrId);
                throw;
            }
        }

        /// <summary>
        /// Update existing user information
        /// Uses single request model with validation
        /// </summary>
        [HttpPut("{usrId}")]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<UsrInfoDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> UpdateUsrInfo(
            [Required][StringLength(50, MinimumLength = 1)] string usrId,
            [FromBody] UsrInfoRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
            {
                return BadRequest<UsrInfoDto>("User ID is required");
            }

            if (request == null)
            {
                return BadRequest<UsrInfoDto>("Request body is required");
            }

            _logger.LogInformation("Updating UsrInfo: {UsrId}", usrId);

            try
            {
                var result = await _usrInfoService.UpdateUserAsync(usrId, request, cancellationToken);
                return Ok(result, "User information updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UsrId}", usrId);
                throw;
            }
        }

        /// <summary>
        /// Delete user information
        /// </summary>
        [HttpDelete("{usrId}")]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUsrInfo(
            [Required][StringLength(50, MinimumLength = 1)] string usrId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
            {
                return BadRequest<bool>("User ID is required");
            }

            _logger.LogInformation("Deleting UsrInfo: {UsrId}", usrId);

            try
            {
                var result = await _usrInfoService.DeleteUserAsync(usrId, cancellationToken);
                return Ok(result, "User information deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UsrId}", usrId);
                throw;
            }
        }

        /// <summary>
        /// Check if user exists (HEAD request)
        /// </summary>
        [HttpHead("{usrId}")]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> CheckUsrInfoExists(
            [Required][StringLength(50, MinimumLength = 1)] string usrId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
            {
                return BadRequest();
            }

            try
            {
                var exists = await _usrInfoService.UserExistsAsync(usrId, cancellationToken);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists: {UsrId}", usrId);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get user information statistics
        /// </summary>
        [HttpGet("statistics")]
        [MapToApiVersion("2.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<UserStatisticsDto>), 200)]
        public async Task<ActionResult<ApiResponse<UserStatisticsDto>>> GetUsrInfoStatistics(
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting UsrInfo statistics");

            try
            {
                var statistics = await _usrInfoService.GetUserStatisticsAsync(cancellationToken);
                return Ok(statistics, "Statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                throw;
            }
        }
    }
}