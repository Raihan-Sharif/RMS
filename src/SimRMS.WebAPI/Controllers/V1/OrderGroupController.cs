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
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Order Group
/// Creation:    11/Sep/2025
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

namespace SimRMS.WebAPI.Controllers.V1
{
    /// <summary>
    /// Order Group Controller - handles master-detail operations via single SP
    /// Supports separate workflows for group management and user management
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
            ILogger<OrderGroupController> logger,
            IConfigurationService configurationService) : base(configurationService)
        {
            _orderGroupService = orderGroupService ?? throw new ArgumentNullException(nameof(orderGroupService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Common Operations

        /// <summary>
        /// Get Order Group list with nested user data structure
        /// </summary>
        [HttpGet("group-list")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderGroupDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderGroupDto>>>> GetOrderGroupList(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchText = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting OrderGroup list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            var result = await _orderGroupService.GetOrderGroupListAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                searchText: searchText,
                cancellationToken: cancellationToken);

            return Ok(result, "Order Groups retrieved successfully");
        }

        /// <summary>
        /// Get Order Group by code - returns all users in the group as a list
        /// </summary>
        [HttpGet("group-by-code/{groupCode:int}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderGroupDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderGroupDto>>>> GetOrderGroupByCode(
            [FromRoute, Required] int groupCode,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting OrderGroup by code: {GroupCode}", groupCode);

            var orderGroup = await _orderGroupService.GetOrderGroupByCodeAsync(groupCode, cancellationToken);

            if (orderGroup == null || !orderGroup.Any())
                return NotFound<IEnumerable<OrderGroupDto>>($"OrderGroup with code '{groupCode}' not found.");

            return Ok(orderGroup, "Order Group retrieved successfully");
        }

        /// <summary>
        /// Get specific user in Order Group - returns single record
        /// </summary>
        [HttpGet("group-by-code/{groupCode:int}/user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<OrderGroupDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<OrderGroupDto>>> GetOrderGroupUserByCode(
            [FromRoute, Required] int groupCode,
            [FromRoute, Required] string userId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting OrderGroup user by code: {GroupCode}, UsrId: {UsrId}", groupCode, userId);

            var orderGroup = await _orderGroupService.GetOrderGroupUserByCodeAsync(groupCode, userId, cancellationToken);

            if (orderGroup == null)
                return NotFound<OrderGroupDto>($"User '{userId}' not found in OrderGroup '{groupCode}'.");

            return Ok(orderGroup, "Order Group user retrieved successfully");
        }

        /// <summary>
        /// Check if Order Group can be deleted (validates no users exist)
        /// </summary>
        [HttpGet("group-by-code/{groupCode:int}/delete-validation")]
        [ProducesResponseType(typeof(ApiResponse<OrderGroupDeleteResultDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<OrderGroupDeleteResultDto>>> CheckGroupDeleteValidation(
            [FromRoute, Required] int groupCode,
            CancellationToken cancellationToken = default)
        {
            var validation = await _orderGroupService.CheckGroupDeleteValidationAsync(groupCode, cancellationToken);
            return Ok(validation, "Delete validation completed");
        }

        #endregion

        #region Master-Detail CRUD Operations (Single SP)

        /// <summary>
        /// Create Order Group (master only or master + user)
        /// </summary>
        [HttpPost("create-group")]
        [ProducesResponseType(typeof(ApiResponse<OrderGroupDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<OrderGroupDto>>> CreateOrderGroup(
            [FromBody] CreateOrderGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating OrderGroup: {GroupDesc}", request.GroupDesc);

            var result = await _orderGroupService.CreateOrderGroupAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetOrderGroupByCode),
                new { groupCode = result.GroupCode },
                new ApiResponse<OrderGroupDto>
                {
                    Success = true,
                    Data = result,
                    Message = "Order Group created successfully"
                });
        }

        /// <summary>
        /// Update Order Group (master only or master + user)
        /// </summary>
        [HttpPut("update-group/{groupCode:int}")]
        [ProducesResponseType(typeof(ApiResponse<OrderGroupDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<OrderGroupDto>>> UpdateOrderGroup(
            [FromRoute, Required] int groupCode,
            [FromBody] UpdateOrderGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating OrderGroup: {GroupCode}", groupCode);

            var result = await _orderGroupService.UpdateOrderGroupAsync(groupCode, request, cancellationToken);
            return Ok(result, "Order Group updated successfully");
        }

        /// <summary>
        /// Delete Order Group (master only or master + user)
        /// </summary>
        [HttpDelete("delete-group/{groupCode:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteOrderGroup(
            [FromRoute, Required] int groupCode,
            [FromBody] DeleteOrderGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting OrderGroup: {GroupCode}", groupCode);

            var success = await _orderGroupService.DeleteOrderGroupAsync(groupCode, request, cancellationToken);
            return Ok((object)new { GroupCode = groupCode }, "Order Group deleted successfully");
        }

        #endregion

        #region Workflow Operations

        /// <summary>
        /// Get Order Group workflow list - unified endpoint for unauthorized/denied records
        /// Similar to user-exposure workflow pattern: /api/v1/order-group/wf/unauth-denied-list?isAuth=0&pageNumber=1&pageSize=10&searchTerm
        /// </summary>
        [HttpGet("wf/unauth-denied-list")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderGroupDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderGroupDto>>>> GetWorkflowList(
            [FromQuery, Range(0, 2)] int isAuth = 0,
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string workflowType = "all", // "master", "detail", "all"
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting Order Group workflow list - IsAuth: {IsAuth}, Type: {WorkflowType}, Page: {PageNumber}", 
                isAuth, workflowType, pageNumber);

            // For now, return the regular list but filtered - can be enhanced later with specific workflow SPs
            var result = await _orderGroupService.GetOrderGroupListAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                searchText: searchTerm,
                cancellationToken: cancellationToken);

            return Ok(result, "Order Group workflow list retrieved successfully");
        }

        /// <summary>
        /// Authorize Order Group changes (master or detail)
        /// </summary>
        [HttpPost("{groupCode:int}/authorize")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<object>>> AuthorizeOrderGroup(
            [FromRoute, Required] int groupCode,
            [FromBody] AuthorizeOrderGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Authorizing OrderGroup: {GroupCode}, IsAuth: {IsAuth}", 
                groupCode, request.IsAuth);

            // Use the unified authorize method from service
            var success = await _orderGroupService.AuthorizeOrderGroupAsync(groupCode, request, cancellationToken);
            var action = request.IsAuth == (byte)AuthTypeEnum.Approve ? "approved" : "denied";
            return Ok((object)new { GroupCode = groupCode }, $"Order Group {action} successfully");
        }

        #endregion
    }
}