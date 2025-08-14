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
/// Title:       Broker Branch Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Broker Branch Operations
/// Creation:    13/Aug/2025
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
/// Broker Branch controller for managing branch information
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class BrokerBranchController : BaseController
{
    private readonly IBrokerBranchService _brokerBranchService;
    private readonly ILogger<BrokerBranchController> _logger;

    public BrokerBranchController(
        IBrokerBranchService brokerBranchService,
        IConfigurationService configurationService,
        ILogger<BrokerBranchController> logger)
        : base(configurationService)
    {
        _brokerBranchService = brokerBranchService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Market Stock Company Branches with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for branch code or description</param>
    /// <param name="coCode">Filter by specific company code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of branches</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MstCoBrchDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MstCoBrchDto>>>> GetMstCoBrchList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? coCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting MstCoBrch list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _brokerBranchService.GetMstCoBrchListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            coCode: coCode,
            cancellationToken: cancellationToken);

        return Ok(result, "Market Stock Company Branches retrieved successfully");
    }

    /// <summary>
    /// Get Market Stock Company Branch by composite key (company code and branch code)
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="coBrchCode">Branch code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Branch information</returns>
    [HttpGet("{coCode}/{coBrchCode}")]
    [ProducesResponseType(typeof(ApiResponse<MstCoBrchDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<MstCoBrchDto>>> GetMstCoBrchById(
        [FromRoute, Required, MaxLength(5)] string coCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting MstCoBrch by ID: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        var branch = await _brokerBranchService.GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);

        if (branch == null)
        {
            return NotFound<MstCoBrchDto>($"Market Stock Company Branch with code '{coCode}-{coBrchCode}' not found");
        }

        return Ok(branch, "Market Stock Company Branch retrieved successfully");
    }

    /// <summary>
    /// Create new Market Stock Company Branch
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created branch information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MstCoBrchDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<MstCoBrchDto>>> CreateMstCoBrch(
        [FromBody, Required] CreateMstCoBrchRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating MstCoBrch: {CoCode}-{CoBrchCode}", request.CoCode, request.CoBrchCode);

        var createdBranch = await _brokerBranchService.CreateMstCoBrchAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetMstCoBrchById),
            new { coCode = createdBranch.CoCode, coBrchCode = createdBranch.CoBrchCode },
            new ApiResponse<MstCoBrchDto>
            {
                Success = true,
                Data = createdBranch,
                Message = "Market Stock Company Branch created successfully"
            });
    }

    /// <summary>
    /// Update Market Stock Company Branch information
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="coBrchCode">Branch code</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated branch information</returns>
    [HttpPut("{coCode}/{coBrchCode}")]
    [ProducesResponseType(typeof(ApiResponse<MstCoBrchDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<MstCoBrchDto>>> UpdateMstCoBrch(
        [FromRoute, Required, MaxLength(5)] string coCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        [FromBody, Required] UpdateMstCoBrchRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        request.CoCode = coCode;
        request.CoBrchCode = coBrchCode;

        var updatedBranch = await _brokerBranchService.UpdateMstCoBrchAsync(coCode, coBrchCode, request, cancellationToken);

        return Ok(updatedBranch, "Market Stock Company Branch updated successfully");
    }

    /// <summary>
    /// Delete Market Stock Company Branch (soft delete)
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="coBrchCode">Branch code</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{coCode}/{coBrchCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMstCoBrch(
        [FromRoute, Required, MaxLength(5)] string coCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        [FromBody] DeleteMstCoBrchRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        request ??= new DeleteMstCoBrchRequest { CoCode = coCode, CoBrchCode = coBrchCode };
        request.CoCode = coCode;
        request.CoBrchCode = coBrchCode;

        var result = await _brokerBranchService.DeleteMstCoBrchAsync(coCode, coBrchCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete Market Stock Company Branch");
        }

        return Ok(new object(), "Market Stock Company Branch deleted successfully");
    }

    /// <summary>
    /// Check if Market Stock Company Branch exists
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="coBrchCode">Branch code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("{coCode}/{coBrchCode}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult> CheckMstCoBrchExists(
        [FromRoute, Required, MaxLength(5)] string coCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        CancellationToken cancellationToken = default)
    {
        var exists = await _brokerBranchService.MstCoBrchExistsAsync(coCode, coBrchCode, cancellationToken);
        
        return exists ? Ok() : NotFound();
    }
}