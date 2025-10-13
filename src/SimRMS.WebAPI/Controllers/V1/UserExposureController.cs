using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Exposure Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage User Exposure Operations
/// Creation:    04/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.WebAPI.Controllers.V1;

/// <summary>
/// User Exposure controller for managing user exposure information
/// </summary>
[Route("api/v{version:apiVersion}/user-exposure")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class UserExposureController : BaseController
{
    private readonly IUserExposureService _userExposureService;
    private readonly ILogger<UserExposureController> _logger;

    public UserExposureController(
        IUserExposureService userExposureService,
        IConfigurationService configurationService,
        ILogger<UserExposureController> logger)
        : base(configurationService)
    {
        _userExposureService = userExposureService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of User Exposures with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for user ID or remarks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of user exposures</returns>
    [HttpGet("exposure-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserExposureDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserExposureDto>>>> GetUserExposureList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting UserExposure list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _userExposureService.GetUserExposureListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            cancellationToken: cancellationToken);

        return Ok(result, "User Exposures retrieved successfully");
    }

    /// <summary>
    /// Get User Exposure by User ID
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User exposure information</returns>
    [HttpGet("exposure-by-id/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<UserExposureDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<UserExposureDto>>> GetUserExposureById(
        [FromRoute, Required, MaxLength(25)] string usrId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting User Exposure by ID: {UsrId}", usrId);

        var exposure = await _userExposureService.GetUserExposureByIdAsync(usrId, cancellationToken);

        if (exposure == null)
        {
            return NotFound<UserExposureDto>($"User Exposure with ID '{usrId}' not found");
        }

        return Ok(exposure, "User Exposure retrieved successfully");
    }

    /// <summary>
    /// Create new User Exposure
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user exposure information</returns>
    [HttpPost("create-exposure")]
    [ProducesResponseType(typeof(ApiResponse<UserExposureDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<UserExposureDto>>> CreateUserExposure(
        [FromBody, Required] CreateUserExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating User Exposure: {UsrId}", request.UsrId);

        var createdExposure = await _userExposureService.CreateUserExposureAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetUserExposureById),
            new { usrId = createdExposure.UsrId },
            new ApiResponse<UserExposureDto>
            {
                Success = true,
                Data = createdExposure,
                Message = "User Exposure created successfully"
            });
    }

    /// <summary>
    /// Update User Exposure information
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user exposure information</returns>
    [HttpPut("update-exposure/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<UserExposureDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<UserExposureDto>>> UpdateUserExposure(
        [FromRoute, Required, MaxLength(25)] string usrId,
        [FromBody, Required] UpdateUserExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating User Exposure: {UsrId}", usrId);

        request.UsrId = usrId;

        var updatedExposure = await _userExposureService.UpdateUserExposureAsync(usrId, request, cancellationToken);

        return Ok(updatedExposure, "User Exposure updated successfully");
    }

    /// <summary>
    /// Delete User Exposure (soft delete)
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete-exposure/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUserExposure(
        [FromRoute, Required, MaxLength(25)] string usrId,
        [FromBody] DeleteUserExposureRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting User Exposure: {UsrId}", usrId);

        request ??= new DeleteUserExposureRequest { UsrId = usrId };
        request.UsrId = usrId;

        var result = await _userExposureService.DeleteUserExposureAsync(usrId, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete User Exposure");
        }

        return Ok(new object(), "User Exposure deleted successfully");
    }

    /// <summary>
    /// Check if User Exposure exists
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("exposure-exists/{usrId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult> CheckUserExposureExists(
        [FromRoute, Required, MaxLength(25)] string usrId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _userExposureService.UserExposureExistsAsync(usrId, cancellationToken);
        
        return exists ? Ok() : NotFound();
    }

    #region Work Flow

    /// <summary>
    /// Get paginated list of unauthorized User Exposures for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for user ID or remarks</param>
    /// <param name="isAuth">Authorization status (0: Unauthorized, 2: Denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized user exposures</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserExposureDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserExposureDto>>>> GetUserExposureUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery, Range(0, 2)] int isAuth = 0,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} User Exposure list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        var result = await _userExposureService.GetUserExposureUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"User Exposures {authAction} data retrieved successfully");
    }

    /// <summary>
    /// Authorize User Exposure in workflow
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeUserExposureWF(
        [FromRoute, Required, MaxLength(25)] string usrId,
        [FromBody, Required] AuthorizeUserExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing User Exposure in workflow: {UsrId} Auth Action: {authAction}", usrId, authAction);

        request.UsrId = usrId;

        var result = await _userExposureService.AuthorizeUserExposureAsync(usrId, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {authAction} User Exposure");
        }

        return Ok(new object(), $"User Exposure {authAction} successfully");
    }

    #endregion
}