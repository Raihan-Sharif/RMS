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
/// Title:       Exchange Controller
/// Author:      Raihan
/// Purpose:     Manage Exchange Operations
/// Creation:    01/Dec/2025
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
/// Exchange controller for managing exchange information
/// </summary>
[Route("api/v{version:apiVersion}/exchange")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ExchangeController : BaseController
{
    private readonly IExchangeService _exchangeService;
    private readonly ILogger<ExchangeController> _logger;

    public ExchangeController(
        IExchangeService exchangeService,
        IConfigurationService configurationService,
        ILogger<ExchangeController> logger)
        : base(configurationService)
    {
        _exchangeService = exchangeService;
        _logger = logger;
    }

    #region Exchanges List operations
    /// <summary>
    /// Get paginated list of Exchanges with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for exchange code or broker code</param>
    /// <param name="xchgCode">Filter by specific exchange code</param>
    /// <param name="sortDirection">Sort direction (optional, e.g. "asc" or "desc")"</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of exchanges</returns>
    [HttpGet("exchange-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExchangeDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ExchangeDto>>>> GetExchangeList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchText = null,
        [FromQuery] string? searchColumn = null,
        [FromQuery] string? xchgCode = null,
        [FromQuery] string? sortDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Exchange list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _exchangeService.GetExchangeListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchText: searchText,
            searchColumn: searchColumn,
            xchgCode: xchgCode,
            sortDirection: sortDirection,
            cancellationToken: cancellationToken);

        return Ok(result, "Exchanges retrieved successfully");
    }

    /// <summary>
    /// Get Exchange by exchange code, prefix and broker code
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="xchgPrefix">Exchange prefix</param>
    /// <param name="brokerCode">Broker code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exchange information</returns>
    [HttpGet("exchange-by-id/{xchgCode}/{xchgPrefix}/{brokerCode}")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ExchangeDto>>> GetExchangeById(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required] int xchgPrefix,
        [FromRoute, Required, MaxLength(10)] string brokerCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving Exchange by ID: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        var exchange = await _exchangeService.GetExchangeByIdAsync(xchgCode, xchgPrefix, brokerCode, cancellationToken);

        if (exchange == null)
        {
            return NotFound<ExchangeDto>($"The Exchange with code '{xchgCode}-{xchgPrefix}-{brokerCode}' not found");
        }

        return Ok(exchange, "Exchange retrieved successfully");
    }
    #endregion

    #region Exchanges CRUD operations
    /// <summary>
    /// Create new Exchange
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created exchange information</returns>
    [HttpPost("create-exchange")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<ExchangeDto>>> CreateExchange(
        [FromBody, Required] CreateExchangeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", request.XchgCode, request.XchgPrefix, request.BrokerCode);

        var createdExchange = await _exchangeService.CreateExchangeAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetExchangeById),
            new { xchgCode = createdExchange.XchgCode, xchgPrefix = createdExchange.XchgPrefix, brokerCode = createdExchange.BrokerCode },
            new ApiResponse<ExchangeDto>
            {
                Success = true,
                Data = createdExchange,
                Message = "Exchange created successfully"
            });
    }

    /// <summary>
    /// Update Exchange information
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="xchgPrefix">Exchange prefix</param>
    /// <param name="brokerCode">Broker code</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated exchange information</returns>
    [HttpPut("update-exchange/{xchgCode}/{xchgPrefix}/{brokerCode}")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ExchangeDto>>> UpdateExchange(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required] int xchgPrefix,
        [FromRoute, Required, MaxLength(10)] string brokerCode,
        [FromBody, Required] UpdateExchangeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        request.XchgCode = xchgCode;
        request.XchgPrefix = xchgPrefix;
        request.BrokerCode = brokerCode;

        var updatedExchange = await _exchangeService.UpdateExchangeAsync(xchgCode, xchgPrefix, brokerCode, request, cancellationToken);

        return Ok(updatedExchange, "Exchange updated successfully");
    }


    /// <summary>
    /// Delete Exchange (soft delete)
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="xchgPrefix">Exchange prefix</param>
    /// <param name="brokerCode">Broker code</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete-exchange/{xchgCode}/{xchgPrefix}/{brokerCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteExchange(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required] int xchgPrefix,
        [FromRoute, Required, MaxLength(10)] string brokerCode,
        [FromBody] DeleteExchangeRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        request ??= new DeleteExchangeRequest { XchgCode = xchgCode, XchgPrefix = xchgPrefix, BrokerCode = brokerCode };
        request.XchgCode = xchgCode;
        request.XchgPrefix = xchgPrefix;
        request.BrokerCode = brokerCode;

        var result = await _exchangeService.DeleteExchangeAsync(xchgCode, xchgPrefix, brokerCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete Exchange");
        }

        return Ok(new object(), "Exchange deleted successfully");
    }
    #endregion


    #region Exchange Work Flow 
    /// <summary>
    /// Get paginated list of unauthorized Exchanges for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="sortDirection">Sort direction (optional, e.g. "asc" or "desc")"</param>
    /// <param name="searchTerm">Search term for broker code</param>
    /// <param name="xchgCode">Filter by specific exchange code</param>
    /// <param name="isAuth">Authorization status filter (0=unauthorized, 2=denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized exchanges</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExchangeDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ExchangeDto>>>> GetExchangeUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? sortDirection = "ASC",
        [FromQuery] string? searchText = null,
        [FromQuery] string? searchColumn = null,
        [FromQuery] string? xchgCode = null,
        [FromQuery, Range(0, 2)] int isAuth = (byte) AuthTypeEnum.UnAuthorize,
        CancellationToken cancellationToken = default)
    {
        string authAction = isAuth == (byte) AuthTypeEnum.UnAuthorize ? AuthTypeEnum.UnAuthorize.ToString() : AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting unauthorized Exchange list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _exchangeService.GetExchangeUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            sortDirection: sortDirection,
            searchText: searchText,
            searchColumn: searchColumn,
            xchgCode: xchgCode,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"Exchanges ({authAction}) data retrieved successfully");
    }

    /// <summary>
    /// Authorize Exchange in workflow
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="xchgPrefix">Exchange prefix</param>
    /// <param name="brokerCode">Broker code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{xchgCode}/{xchgPrefix}/{brokerCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeExchangeWF(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required] int xchgPrefix,
        [FromRoute, Required, MaxLength(10)] string brokerCode,
        [FromBody, Required] AuthorizeExchangeRequest request,
        CancellationToken cancellationToken = default)
    {
        string actionName = ((ActionTypeEnum)request.ActionType).ToString();
       
        _logger.LogInformation("Authorizing Exchange in workflow: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        request.XchgCode = xchgCode;
        request.XchgPrefix = xchgPrefix;
        request.BrokerCode = brokerCode;

        var result = await _exchangeService.AuthorizeExchangeAsync(xchgCode, xchgPrefix, brokerCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {actionName} Exchange");
        }
        
        return Ok(new object(), $"Exchange {actionName} successfully");

    }

    #endregion
}
