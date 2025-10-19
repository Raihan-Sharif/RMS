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
/// Title:       User Controller
/// Author:      Asif Zaman
/// Purpose:     Manage User Operations using LB_SP_GetUserList
/// Creation:    03/Sep/2025
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
/// User controller for managing user information
/// </summary>
[Route("api/v{version:apiVersion}/user")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        IConfigurationService configurationService,
        ILogger<UserController> logger)
        : base(configurationService)
    {
        _userService = userService;
        _logger = logger;
    }

    #region User List operations
    /// <summary>
    /// Get the list of authorized users
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchText">Search term for multiple fields (UsrName, UsrEmail, UsrNICNo, UsrPhone, UsrMobile, UsrID, DlrCode)</param>
    /// <param name="usrStatus">Filter by user status</param>
    /// <param name="dlrCode">Filter by dealer code</param>
    /// <param name="coCode">Filter by company code</param>
    /// <param name="coBrchCode">Filter by company branch code</param>
    /// <param name="sortColumn">Sort column (default: UsrID)</param>
    /// <param name="sortDirection">Sort direction (ASC/DESC, default: ASC)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet("user-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUserList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchText = null,
        [FromQuery] string? usrStatus = null,
        [FromQuery] string? dlrCode = null,
        [FromQuery] string? coCode = null,
        [FromQuery] string? coBrchCode = null,
        [FromQuery] string? sortColumn = "UsrID",
        [FromQuery] string? sortDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting User list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _userService.GetUserListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchText: searchText,
            usrStatus: usrStatus,
            dlrCode: dlrCode,
            coCode: coCode,
            coBrchCode: coBrchCode,
            sortColumn: sortColumn,
            sortDirection: sortDirection,
            cancellationToken: cancellationToken);

        return Ok(result, "Users retrieved successfully");
    }


    /// <summary>
    /// Get User by ID
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User information with detailed status and expiry information</returns>
    [HttpGet("user-by-id/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetUserById(
        [FromRoute, Required, MaxLength(50)] string usrId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving User by ID: {UsrId}", usrId);

        var user = await _userService.GetUserByIdAsync(usrId, cancellationToken);

        if (user == null)
        {
            return NotFound<UserDetailDto>($"The User with ID '{usrId}' not found");
        }

        return Ok(user, "User retrieved successfully");
    }

    #endregion

    #region User CRUD operations
    /// <summary>
    /// Create new User
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user information</returns>
    [HttpPost("create-user")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(
        [FromBody, Required] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating User: {UsrID}", request.UsrID);

        var createdUser = await _userService.CreateUserAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetUserById),
            new { usrId = createdUser.UsrID },
            new ApiResponse<UserDto>
            {
                Success = true,
                Data = createdUser,
                Message = "User created successfully"
            });
    }

    /// <summary>
    /// Update User information
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user information</returns>
    [HttpPut("update-user/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(
        [FromRoute, Required, MaxLength(25)] string usrId,
        [FromBody, Required] UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating User: {UsrID}", usrId);

        request.UsrID = usrId;

        var updatedUser = await _userService.UpdateUserAsync(usrId, request, cancellationToken);

        return Ok(updatedUser, "User updated successfully");
    }

    /// <summary>
    /// Delete User (soft delete)
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete-user/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(
        [FromRoute, Required, MaxLength(25)] string usrId,
        [FromBody] DeleteUserRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting User: {UsrID}", usrId);

        request ??= new DeleteUserRequest { UsrID = usrId };
        request.UsrID = usrId;

        var result = await _userService.DeleteUserAsync(usrId, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete User");
        }

        return Ok(new object(), "User deleted successfully");
    }

    /// <summary>
    /// Check if User exists by User ID
    /// </summary>
    /// <param name="usrId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Existence status</returns>
    [HttpGet("user-exists/{usrId}")]
    [ProducesResponseType(typeof(ApiResponse<UserExistenceDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<UserExistenceDto>>> UserExists(
        [FromRoute, Required, MaxLength(50)] string usrId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking if User exists with UsrID: {UsrID}", usrId);

        var exists = await _userService.UserExistsAsync(usrId, cancellationToken);

        var result = new UserExistenceDto
        {
            UsrID = usrId,
            IsExist = exists
        };

        var message = exists
            ? $"User with ID '{usrId}' exists"
            : $"User with ID '{usrId}' does not exist";

        return Ok(result, message);
    }
    #endregion

    #region User Work Flow 
    /// <summary>
    /// Get paginated list of unauthorized or denied Users for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="sortColumn">Sort column (default: UsrID)</param>
    /// <param name="sortDirection">Sort direction (optional, e.g. "ASC" or "DESC")</param>"
    /// <param name="searchText">Search term for multiple fields</param>
    /// <param name="usrStatus">Filter by user status</param>
    /// <param name="dlrCode">Filter by dealer code</param>
    /// <param name="coCode">Filter by company code</param>
    /// <param name="coBrchCode">Filter by company branch code</param>
    /// <param name="isAuth">Authorization status filter (0=unauthorized, 2=denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized or denied users</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUserUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? sortColumn = "UsrID",
        [FromQuery] string? sortDirection = "ASC",
        [FromQuery] string? searchText = null,
        [FromQuery] string? usrStatus = null,
        [FromQuery] string? dlrCode = null,
        [FromQuery] string? coCode = null,
        [FromQuery] string? coBrchCode = null,
        [FromQuery, Range(0, 2)] int isAuth = (byte)AuthTypeEnum.UnAuthorize,
        CancellationToken cancellationToken = default)
    {
        string authAction = isAuth == (byte)AuthTypeEnum.UnAuthorize ? AuthTypeEnum.UnAuthorize.ToString() : AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting user workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}", pageNumber, pageSize, isAuth);

        var result = await _userService.GetUserUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            sortColumn: sortColumn,
            sortDirection: sortDirection,
            searchText: searchText,
            usrStatus: usrStatus,
            dlrCode: dlrCode,
            coCode: coCode,
            coBrchCode: coBrchCode,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"Users ({authAction}) data retrieved successfully");
    }
    /// <summary>
    /// Authorize User in workflow
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
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeUserWF(
        [FromRoute, Required, MaxLength(25)] string usrId,
        [FromBody, Required] AuthorizeUserRequest request,
        CancellationToken cancellationToken = default)
    {
        string actionName = ((ActionTypeEnum)request.ActionType).ToString();

        _logger.LogInformation("Authorizing User in workflow: {UsrId}", usrId);

        request.UsrID = usrId;

        var result = await _userService.AuthorizeUserAsync(usrId, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {actionName} User");
        }

        return Ok(new object(), $"User {actionName} successfully");
    }
    #endregion

}