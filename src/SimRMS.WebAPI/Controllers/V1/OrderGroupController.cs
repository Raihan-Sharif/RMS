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
/// Title:       Order Group Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage Order Group Operations
/// Creation:    08/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.WebAPI.Controllers.V1;

/// <summary>
/// Order Group controller for managing order group information
/// </summary>
[Route("api/v{version:apiVersion}/order-group")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class OrderGroupController : BaseController
{
    private readonly IOrderGroupService _orderGroupService;
    private readonly ILogger<OrderGroupController> _logger;

    public OrderGroupController(
        IOrderGroupService orderGroupService,
        IConfigurationService configurationService,
        ILogger<OrderGroupController> logger)
        : base(configurationService)
    {
        _orderGroupService = orderGroupService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Order Groups with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for group description or remarks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of order groups</returns>
    [HttpGet("group-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderGroupDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderGroupDto>>>> GetOrderGroupList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting OrderGroup list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _orderGroupService.GetOrderGroupListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            cancellationToken: cancellationToken);

        return Ok(result, "Order Groups retrieved successfully");
    }

    /// <summary>
    /// Get Order Group by Group Code
    /// </summary>
    /// <param name="groupCode">Group Code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order group information</returns>
    [HttpGet("group-by-code/{groupCode}")]
    [ProducesResponseType(typeof(ApiResponse<OrderGroupDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<OrderGroupDto>>> GetOrderGroupByCode(
        [FromRoute, Required, Range(1, int.MaxValue)] int groupCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting OrderGroup by Code: {GroupCode}", groupCode);

        var orderGroup = await _orderGroupService.GetOrderGroupByCodeAsync(groupCode, cancellationToken);

        if (orderGroup == null)
        {
            return NotFound<OrderGroupDto>($"Order Group with Code '{groupCode}' not found");
        }

        return Ok(orderGroup, "Order Group retrieved successfully");
    }

    /// <summary>
    /// Create new Order Group
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order group information</returns>
    [HttpPost("create-group")]
    [ProducesResponseType(typeof(ApiResponse<OrderGroupDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<OrderGroupDto>>> CreateOrderGroup(
        [FromBody, Required] CreateOrderGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating OrderGroup: {GroupDesc}", request.GroupDesc);

        var createdGroup = await _orderGroupService.CreateOrderGroupAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetOrderGroupByCode),
            new { groupCode = createdGroup.GroupCode },
            new ApiResponse<OrderGroupDto>
            {
                Success = true,
                Data = createdGroup,
                Message = "Order Group created successfully"
            });
    }

    /// <summary>
    /// Update Order Group information
    /// </summary>
    /// <param name="groupCode">Group Code</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated order group information</returns>
    [HttpPut("update-group/{groupCode}")]
    [ProducesResponseType(typeof(ApiResponse<OrderGroupDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<OrderGroupDto>>> UpdateOrderGroup(
        [FromRoute, Required, Range(1, int.MaxValue)] int groupCode,
        [FromBody, Required] UpdateOrderGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating OrderGroup: {GroupCode}", groupCode);

        request.GroupCode = groupCode;

        var updatedGroup = await _orderGroupService.UpdateOrderGroupAsync(groupCode, request, cancellationToken);

        return Ok(updatedGroup, "Order Group updated successfully");
    }

    /// <summary>
    /// Delete Order Group (soft delete)
    /// </summary>
    /// <param name="groupCode">Group Code</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete-group/{groupCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteOrderGroup(
        [FromRoute, Required, Range(1, int.MaxValue)] int groupCode,
        [FromBody] DeleteOrderGroupRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting OrderGroup: {GroupCode}", groupCode);

        request ??= new DeleteOrderGroupRequest { GroupCode = groupCode };
        request.GroupCode = groupCode;

        var result = await _orderGroupService.DeleteOrderGroupAsync(groupCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete Order Group");
        }

        return Ok(new object(), "Order Group deleted successfully");
    }

    /// <summary>
    /// Check if Order Group exists
    /// </summary>
    /// <param name="groupCode">Group Code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("group-exists/{groupCode}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult> CheckOrderGroupExists(
        [FromRoute, Required, Range(1, int.MaxValue)] int groupCode,
        CancellationToken cancellationToken = default)
    {
        var exists = await _orderGroupService.OrderGroupExistsAsync(groupCode, cancellationToken);
        
        return exists ? Ok() : NotFound();
    }

    #region Work Flow

    /// <summary>
    /// Get paginated list of unauthorized Order Groups for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for group description or remarks</param>
    /// <param name="isAuth">Authorization status (0: Unauthorized, 2: Denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized order groups</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderGroupDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderGroupDto>>>> GetOrderGroupUnAuthDeniedListWF(
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

        _logger.LogInformation("Getting {authAction} OrderGroup list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        var result = await _orderGroupService.GetOrderGroupUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"Order Groups {authAction} data retrieved successfully");
    }

    /// <summary>
    /// Authorize Order Group in workflow
    /// </summary>
    /// <param name="groupCode">Group Code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{groupCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeOrderGroupWF(
        [FromRoute, Required, Range(1, int.MaxValue)] int groupCode,
        [FromBody, Required] AuthorizeOrderGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing OrderGroup in workflow: {GroupCode} Auth Action: {authAction}", groupCode, authAction);

        request.GroupCode = groupCode;

        var result = await _orderGroupService.AuthorizeOrderGroupAsync(groupCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {authAction} Order Group");
        }

        return Ok(new object(), $"Order Group {authAction} successfully");
    }

    #endregion
}