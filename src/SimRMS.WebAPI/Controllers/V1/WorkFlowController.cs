using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Shared.Models;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       WorkFlow Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage WorkFlow Operations (pending authorizations and denied items)
/// Creation:    21/Aug/2025
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
/// WorkFlow controller for managing pending authorizations and denied items
/// </summary>
[Route("api/v{version:apiVersion}/workflow")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class WorkFlowController : BaseController
{
    private readonly IWorkFlowService _workFlowService;
    private readonly ILogger<WorkFlowController> _logger;

    public WorkFlowController(
        IWorkFlowService workFlowService,
        IConfigurationService configurationService,
        ILogger<WorkFlowController> logger)
        : base(configurationService)
    {
        _workFlowService = workFlowService;
        _logger = logger;
    }

    /// <summary>
    /// Get list of pending authorization workflow items
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of workflow items pending authorization</returns>
    [HttpGet("pending-authorizations")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkFlowItemDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkFlowItemDto>>>> GetPendingAuthorizations(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting pending authorization workflow items");

        var result = await _workFlowService.GetPendingAuthorizationsAsync(cancellationToken);

        return Ok(result, "Pending authorization items retrieved successfully");
    }

    /// <summary>
    /// Get list of denied workflow items
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of denied workflow items</returns>
    [HttpGet("denied-items")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkFlowItemDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkFlowItemDto>>>> GetDeniedItems(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting denied workflow items");

        var result = await _workFlowService.GetDeniedItemsAsync(cancellationToken);

        return Ok(result, "Denied workflow items retrieved successfully");
    }

    /// <summary>
    /// Get combined workflow summary including both pending authorizations and denied items
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete workflow summary with totals and item details</returns>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<WorkFlowSummaryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<WorkFlowSummaryDto>>> GetWorkFlowSummary(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting combined workflow summary");

        var result = await _workFlowService.GetWorkFlowSummaryAsync(cancellationToken);

        return Ok(result, "Workflow summary retrieved successfully");
    }
}