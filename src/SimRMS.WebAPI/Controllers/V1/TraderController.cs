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
/// Title:       Trader Controller
/// Author:      Asif Zaman
/// Purpose:     Manage Trader Operations
/// Creation:    20/Aug/2025
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
/// Trader controller for managing trader information
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class TraderController : BaseController
{
    private readonly ITraderService _traderService;
    private readonly ILogger<TraderController> _logger;

    public TraderController(
        ITraderService traderService,
        IConfigurationService configurationService,
        ILogger<TraderController> logger)
        : base(configurationService)
    {
        _traderService = traderService;
        _logger = logger;
    }

    #region Traders List operations
    /// <summary>
    /// Get paginated list of Market Stock Traders with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for exchange code or dealer code</param>
    /// <param name="xchgCode">Filter by specific exchange code</param>
    /// <param name="sortDirection">Sort direction (optional, e.g. "asc" or "desc")</param>"
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of traders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MstTraderDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MstTraderDto>>>> GetMstTraderList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? xchgCode = null,
        [FromQuery] string? sortDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting MstTrader list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _traderService.GetMstTraderListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            xchgCode: xchgCode,
            sortDirection: sortDirection,
            cancellationToken: cancellationToken);

        return Ok(result, "Market Stock Traders retrieved successfully");
    }

    /// <summary>
    /// Get Market Stock Trader by exchange code and dealer code
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="dlrCode">Dealer code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trader information</returns>
    [HttpGet("{xchgCode}/{dlrCode}")]
    [ProducesResponseType(typeof(ApiResponse<MstTraderDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<MstTraderDto>>> GetMstTraderById(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(15)] string dlrCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting MstTrader by ID: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        var trader = await _traderService.GetMstTraderByIdAsync(xchgCode, dlrCode, cancellationToken);

        if (trader == null)
        {
            return NotFound<MstTraderDto>($"Market Stock Trader with code '{xchgCode}-{dlrCode}' not found");
        }

        return Ok(trader, "Market Stock Trader retrieved successfully");
    }
    #endregion

    #region Traders CRUD operations
    /// <summary>
    /// Create new Market Stock Trader
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created trader information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MstTraderDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<MstTraderDto>>> CreateMstTrader(
        [FromBody, Required] CreateMstTraderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating MstTrader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);

        var createdTrader = await _traderService.CreateMstTraderAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetMstTraderById),
            new { xchgCode = createdTrader.XchgCode, dlrCode = createdTrader.DlrCode },
            new ApiResponse<MstTraderDto>
            {
                Success = true,
                Data = createdTrader,
                Message = "Market Stock Trader created successfully"
            });
    }

    /// <summary>
    /// Update Market Stock Trader information
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="dlrCode">Dealer code</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated trader information</returns>
    [HttpPut("{xchgCode}/{dlrCode}")]
    [ProducesResponseType(typeof(ApiResponse<MstTraderDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<MstTraderDto>>> UpdateMstTrader(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(15)] string dlrCode,
        [FromBody, Required] UpdateMstTraderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        request.XchgCode = xchgCode;
        request.DlrCode = dlrCode;

        var updatedTrader = await _traderService.UpdateMstTraderAsync(xchgCode, dlrCode, request, cancellationToken);

        return Ok(updatedTrader, "Market Stock Trader updated successfully");
    }


    /// <summary>
    /// Delete Market Stock Trader (soft delete)
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="dlrCode">Dealer code</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{xchgCode}/{dlrCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMstTrader(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(15)] string dlrCode,
        [FromBody] DeleteMstTraderRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        request ??= new DeleteMstTraderRequest { XchgCode = xchgCode, DlrCode = dlrCode };
        request.XchgCode = xchgCode;
        request.DlrCode = dlrCode;

        var result = await _traderService.DeleteMstTraderAsync(xchgCode, dlrCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete Market Stock Trader");
        }

        return Ok(new object(), "Market Stock Trader deleted successfully");
    }
    #endregion


    #region Trader Work Flow 
    /// <summary>
    /// Get paginated list of unauthorized Market Stock Traders for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="sortDirection">Sort direction (optional, e.g. "asc" or "desc")</param>"
    /// <param name="searchTerm">Search term for dealer code</param>
    /// <param name="xchgCode">Filter by specific exchange code</param>
    /// <param name="isAuth">Authorization status filter (0=unauthorized, 2=denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized traders</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MstTraderDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MstTraderDto>>>> GetTraderUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? sortDirection = "ASC",
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? xchgCode = null,
        [FromQuery, Range(0, 2)] int isAuth = (byte) AuthTypeEnum.UnAuthorize,
        CancellationToken cancellationToken = default)
    {
        string authAction = isAuth == (byte) AuthTypeEnum.UnAuthorize ? AuthTypeEnum.UnAuthorize.ToString() : AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting unauthorized MstTrader list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _traderService.GetTraderUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            sortDirection: sortDirection,
            searchTerm: searchTerm,
            xchgCode: xchgCode,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"Market Stock Traders ({authAction}) data retrieved successfully");
    }

    /// <summary>
    /// Authorize Market Stock Trader in workflow
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="dlrCode">Dealer code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{xchgCode}/{dlrCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeTraderWF(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(15)] string dlrCode,
        [FromBody, Required] AuthorizeMstTraderRequest request,
        CancellationToken cancellationToken = default)
    {
        string actionName = ((ActionTypeEnum)request.ActionType).ToString();
       
        _logger.LogInformation("Authorizing MstTrader in workflow: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        request.XchgCode = xchgCode;
        request.DlrCode = dlrCode;

        var result = await _traderService.AuthorizeTraderAsync(xchgCode, dlrCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {actionName} Market Stock Trader");
        }
        
        return Ok(new object(), $"Market Stock Trader {actionName} successfully");

    }

    #endregion
}