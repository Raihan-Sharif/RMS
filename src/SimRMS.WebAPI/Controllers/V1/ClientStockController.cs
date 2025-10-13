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
/// Title:       Client Stock Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage Client Stock Operations
/// Creation:    29/Sep/2025
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
/// Client Stock controller for managing client stock information
/// </summary>
[Route("api/v{version:apiVersion}/client-stock")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ClientStockController : BaseController
{
    private readonly IClientStockService _clientStockService;
    private readonly ILogger<ClientStockController> _logger;

    public ClientStockController(
        IClientStockService clientStockService,
        IConfigurationService configurationService,
        ILogger<ClientStockController> logger)
        : base(configurationService)
    {
        _clientStockService = clientStockService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Client Stocks with optional search and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="branchCode">Optional branch code filter</param>
    /// <param name="clientCode">Optional client code filter</param>
    /// <param name="stockCode">Optional stock code filter</param>
    /// <param name="xchgCode">Optional exchange code filter</param>
    /// <param name="searchText">Search term for stock code or client code</param>
    /// <param name="sortColumn">Sort column (default: ClientCode)</param>
    /// <param name="sortDirection">Sort direction: ASC or DESC (default: ASC)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of client stocks</returns>
    [HttpGet("stock-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientStockDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientStockDto>>>> GetClientStockList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? branchCode = null,
        [FromQuery] string? clientCode = null,
        [FromQuery] string? stockCode = null,
        [FromQuery] string? xchgCode = null,
        [FromQuery] string? searchText = null,
        [FromQuery] string? sortColumn = "ClientCode",
        [FromQuery] string? sortDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client Stock list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var request = new GetClientStockListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            BranchCode = branchCode,
            ClientCode = clientCode,
            StockCode = stockCode,
            XchgCode = xchgCode,
            SearchText = searchText,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };

        var result = await _clientStockService.GetClientStockListAsync(request, cancellationToken);
        return Ok(result, "Client Stocks retrieved successfully");
    }

    /// <summary>
    /// Get Client Stock by composite key (BranchCode, ClientCode, StockCode)
    /// </summary>
    /// <param name="branchCode">Branch code</param>
    /// <param name="clientCode">Client code</param>
    /// <param name="stockCode">Stock code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Client stock information</returns>
    [HttpGet("stock-by-key/{branchCode}/{clientCode}/{stockCode}")]
    [ProducesResponseType(typeof(ApiResponse<ClientStockDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ClientStockDto>>> GetClientStockByKey(
        [FromRoute, Required, MaxLength(6)] string branchCode,
        [FromRoute, Required, MaxLength(50)] string clientCode,
        [FromRoute, Required, MaxLength(20)] string stockCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client Stock by key - Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            branchCode, clientCode, stockCode);

        var request = new GetClientStockByKeyRequest
        {
            BranchCode = branchCode,
            ClientCode = clientCode,
            StockCode = stockCode
        };

        var clientStock = await _clientStockService.GetClientStockByKeyAsync(request, cancellationToken);

        if (clientStock == null)
        {
            return NotFound($"Client Stock not found for Client: {clientCode}, Stock: {stockCode}");
        }

        return Ok(clientStock, "Client Stock retrieved successfully");
    }

    /// <summary>
    /// Create new Client Stock record
    /// </summary>
    /// <param name="request">Client Stock creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created Client Stock information</returns>
    [HttpPost("create-stock")]
    [ProducesResponseType(typeof(ApiResponse<ClientStockDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ClientStockDto>>> CreateClientStock(
        [FromBody] CreateClientStockRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Client Stock for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            request.BranchCode, request.ClientCode, request.StockCode);

        var result = await _clientStockService.CreateClientStockAsync(request, cancellationToken);
        return Created($"api/v1/client-stock/stock-by-key/{result.BranchCode}/{result.ClientCode}/{result.StockCode}",
            ApiResponse<ClientStockDto>.SuccessResult(result, "Client Stock created successfully"));
    }

    /// <summary>
    /// Update existing Client Stock record
    /// </summary>
    /// <param name="branchCode">Branch Code</param>
    /// <param name="clientCode">Client Code</param>
    /// <param name="stockCode">Stock Code</param>
    /// <param name="request">Client Stock update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated Client Stock information</returns>
    [HttpPut("update-stock/{branchCode}/{clientCode}/{stockCode}")]
    [ProducesResponseType(typeof(ApiResponse<ClientStockDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ClientStockDto>>> UpdateClientStock(
        [FromRoute, Required, MaxLength(6)] string branchCode,
        [FromRoute, Required, MaxLength(50)] string clientCode,
        [FromRoute, Required, MaxLength(20)] string stockCode,
        [FromBody, Required] UpdateClientStockRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Client Stock: {BranchCode}-{ClientCode}-{StockCode}",
            branchCode, clientCode, stockCode);

        request.BranchCode = branchCode;
        request.ClientCode = clientCode;
        request.StockCode = stockCode;

        var result = await _clientStockService.UpdateClientStockAsync(request, cancellationToken);
        return Ok(result, "Client Stock updated successfully");
    }

    /// <summary>
    /// Delete Client Stock record (soft delete)
    /// </summary>
    /// <param name="branchCode">Branch Code</param>
    /// <param name="clientCode">Client Code</param>
    /// <param name="stockCode">Stock Code</param>
    /// <param name="request">Client Stock deletion request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("delete-stock/{branchCode}/{clientCode}/{stockCode}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteClientStock(
        [FromRoute, Required, MaxLength(6)] string branchCode,
        [FromRoute, Required, MaxLength(50)] string clientCode,
        [FromRoute, Required, MaxLength(20)] string stockCode,
        [FromBody] DeleteClientStockRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Client Stock: {BranchCode}-{ClientCode}-{StockCode}",
            branchCode, clientCode, stockCode);

        request ??= new DeleteClientStockRequest { BranchCode = branchCode, ClientCode = clientCode, StockCode = stockCode };
        request.BranchCode = branchCode;
        request.ClientCode = clientCode;
        request.StockCode = stockCode;

        var result = await _clientStockService.DeleteClientStockAsync(request, cancellationToken);
        return Ok(result, "Client Stock deleted successfully");
    }

    /// <summary>
    /// Check if Client Stock record exists
    /// </summary>
    /// <param name="branchCode">Branch code</param>
    /// <param name="clientCode">Client code</param>
    /// <param name="stockCode">Stock code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Existence result</returns>
    [HttpGet("stock-exists/{branchCode}/{clientCode}/{stockCode}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<bool>>> ClientStockExists(
        [FromRoute, Required, MaxLength(6)] string branchCode,
        [FromRoute, Required, MaxLength(50)] string clientCode,
        [FromRoute, Required, MaxLength(20)] string stockCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking Client Stock existence for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            branchCode, clientCode, stockCode);

        var request = new GetClientStockByKeyRequest
        {
            BranchCode = branchCode,
            ClientCode = clientCode,
            StockCode = stockCode
        };

        var exists = await _clientStockService.ClientStockExistsAsync(request, cancellationToken);
        return Ok(exists, exists ? "Client Stock exists" : "Client Stock does not exist");
    }

    #region Work Flow

    /// <summary>
    /// Get paginated list of unauthorized/denied Client Stock records for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchText">Search term for stock code or client code</param>
    /// <param name="isAuth">Authorization status (0: Unauthorized, 2: Denied)</param>
    /// <param name="sortColumn">Sort column (default: ClientCode)</param>
    /// <param name="sortDirection">Sort direction (default: ASC)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized/denied client stocks</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientStockDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 501)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientStockDto>>>> GetClientStockUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchText = null,
        [FromQuery, Range(0, 2)] int isAuth = 0,
        [FromQuery] string sortColumn = "ClientCode",
        [FromQuery] string sortDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} Client Stock list for workflow - Page: {PageNumber}, Size: {PageSize}", authAction, pageNumber, pageSize);

        var request = new GetClientStockWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IsAuth = (AuthTypeEnum)isAuth,
            BranchCode = null,
            ClientCode = null,
            StockCode = null,
            XchgCode = null,
            SearchText = searchText,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };

        var result = await _clientStockService.GetClientStockWorkflowListAsync(request, cancellationToken);
        return Ok(result, $"Client Stocks {authAction} data retrieved successfully");
    }

    /// <summary>
    /// Authorize Client Stock in workflow
    /// </summary>
    /// <param name="branchCode">Branch Code</param>
    /// <param name="clientCode">Client Code</param>
    /// <param name="stockCode">Stock Code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{branchCode}/{clientCode}/{stockCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 501)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeClientStockWF(
        [FromRoute, Required, MaxLength(6)] string branchCode,
        [FromRoute, Required, MaxLength(50)] string clientCode,
        [FromRoute, Required, MaxLength(20)] string stockCode,
        [FromBody, Required] AuthorizeClientStockRequest request,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.AuthAction == AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.AuthAction == AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing Client Stock in workflow: {BranchCode}-{ClientCode}-{StockCode} Auth Action: {authAction}", branchCode, clientCode, stockCode, authAction);

        request.BranchCode = branchCode;
        request.ClientCode = clientCode;
        request.StockCode = stockCode;

        var result = await _clientStockService.AuthorizeClientStockAsync(request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {authAction} Client Stock");
        }

        return Ok(new object(), $"Client Stock {authAction} successfully");
    }

    #endregion
}