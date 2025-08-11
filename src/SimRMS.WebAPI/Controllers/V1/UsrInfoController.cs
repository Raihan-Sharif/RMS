using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;
using System.ComponentModel.DataAnnotations;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Information Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage User Information Operations
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

namespace SimRMS.WebAPI.Controllers.V1;

    /// <summary>
    /// Simplified UsrInfo controller - much cleaner and easier to maintain
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class UsrInfoController : BaseController
    {
        private readonly IUsrInfoService _usrInfoService;
        private readonly ILogger<UsrInfoController> _logger;

        public UsrInfoController(
            IUsrInfoService usrInfoService,
            IConfigurationService configurationService,
            ILogger<UsrInfoController> logger)
            : base(configurationService)
        {
            _usrInfoService = usrInfoService;
            _logger = logger;
        }

        /// <summary>
        /// APPROACH: 
        /// Get paginated list of users with basic query parameters
        /// Validation done directly in controller
        /// </summary>
        [HttpGet("simple")]
        [MapToApiVersion("1.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsrInfoDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsrInfoDto>>>> GetUsers(
            [FromQuery][Range(1, 1000)] int pageNumber = 1,
            [FromQuery][Range(1, 100)] int pageSize = 10,
            [FromQuery] string? usrStatus = null,
            [FromQuery] string? coCode = null,
            [FromQuery] string? dlrCode = null,
            [FromQuery] string? rmsType = null,
            [FromQuery] string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _usrInfoService.GetUsersAsync(pageNumber, pageSize, usrStatus, coCode, dlrCode, rmsType, searchTerm, cancellationToken);
            return Ok(result, $"Retrieved {result.Data.Count()} users");
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
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> GetUser(
            [Required] string usrId,
            CancellationToken cancellationToken = default)
        {
            var result = await _usrInfoService.GetUserByIdAsync(usrId, cancellationToken);
            if (result == null)
                return NotFound<UsrInfoDto>("User not found");

            return Ok(result, "User retrieved successfully");
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
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> CreateUser(
            [FromBody] UsrInfoRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _usrInfoService.CreateUserAsync(request, cancellationToken);
            return Ok(result, "User created successfully");
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
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> UpdateUser(
            [Required] string usrId,
            [FromBody] UsrInfoRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _usrInfoService.UpdateUserAsync(usrId, request, cancellationToken);
            return Ok(result, "User updated successfully");
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
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(
            [Required] string usrId,
            CancellationToken cancellationToken = default)
        {
            var result = await _usrInfoService.DeleteUserAsync(usrId, cancellationToken);
            return Ok(result, "User deleted successfully");
        }

        /// <summary>
        /// Get user information statistics
        /// </summary>
        [HttpGet("statistics")]
        [MapToApiVersion("2.0")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<UserStatisticsDto>), 200)]
        public async Task<ActionResult<ApiResponse<UserStatisticsDto>>> GetStatistics(
            CancellationToken cancellationToken = default)
        {
            var result = await _usrInfoService.GetUserStatisticsAsync(cancellationToken);
            return Ok(result, "Statistics retrieved successfully");
        }
    }
