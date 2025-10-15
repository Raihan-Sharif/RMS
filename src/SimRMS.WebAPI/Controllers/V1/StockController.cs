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
/// Title:       Stock Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage Stock Operations
/// Creation:    23/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
///
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.WebAPI.Controllers.V1;

/// <summary>
/// Stock controller for managing stock information
/// </summary>
[Route("api/v{version:apiVersion}/stock")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class StockController : BaseController
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockController> _logger;

    public StockController(
        IStockService stockService,
        IConfigurationService configurationService,
        ILogger<StockController> logger)
        : base(configurationService)
    {
        _stockService = stockService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Stocks with optional search and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="xchgCode">Exchange code filter</param>
    /// <param name="stkCode">Stock code filter</param>
    /// <param name="searchTerm">Search term for stock name</param>
    /// <param name="sortColumn">Sort column (default: StkCode)</param>
    /// <param name="sortDirection">Sort direction (default: ASC)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of stocks</returns>
    [HttpGet("stock-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockDto>>>> GetStockList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery, MaxLength(10)] string? xchgCode = null,
        [FromQuery, MaxLength(20)] string? stkCode = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string sortColumn = "StkCode",
        [FromQuery] string sortDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Stock list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _stockService.GetStockListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            xchgCode: xchgCode,
            stkCode: stkCode,
            searchTerm: searchTerm,
            sortColumn: sortColumn,
            sortDirection: sortDirection,
            cancellationToken: cancellationToken);

        return Ok(result, "Stocks retrieved successfully");
    }

    /// <summary>
    /// Get Stock by Exchange Code and Stock Code
    /// </summary>
    /// <param name="xchgCode">Exchange Code</param>
    /// <param name="stkCode">Stock Code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stock information</returns>
    [HttpGet("stock-by-key/{xchgCode}/{stkCode}")]
    [ProducesResponseType(typeof(ApiResponse<StockDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<StockDto>>> GetStockByKey(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(20)] string stkCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving Stock by key: {XchgCode}-{StkCode}", xchgCode, stkCode);

        var stock = await _stockService.GetStockByKeyAsync(xchgCode, stkCode, cancellationToken);

        if (stock == null)
        {
            return NotFound<StockDto>($"The Stock with key '{xchgCode}-{stkCode}' not found");
        }

        return Ok(stock, "Stock retrieved successfully");
    }

    /// <summary>
    /// Create new Stock
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created stock information</returns>
    [HttpPost("create-stock")]
    [ProducesResponseType(typeof(ApiResponse<StockDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    [ProducesResponseType(typeof(ApiResponse<object>), 501)]
    public async Task<ActionResult<ApiResponse<StockDto>>> CreateStock(
        [FromBody, Required] CreateStockRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Stock: {XchgCode}-{StkCode}", request.XchgCode, request.StkCode);

        var createdStock = await _stockService.CreateStockAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetStockByKey),
            new { xchgCode = createdStock.XchgCode, stkCode = createdStock.StkCode },
            new ApiResponse<StockDto>
            {
                Success = true,
                Data = createdStock,
                Message = "Stock created successfully"
            });
    }

    /// <summary>
    /// Update Stock information
    /// </summary>
    /// <param name="xchgCode">Exchange Code</param>
    /// <param name="stkCode">Stock Code</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated stock information</returns>
    [HttpPut("update-stock/{xchgCode}/{stkCode}")]
    [ProducesResponseType(typeof(ApiResponse<StockDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 501)]
    public async Task<ActionResult<ApiResponse<StockDto>>> UpdateStock(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(20)] string stkCode,
        [FromBody, Required] UpdateStockRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);

        var updatedStock = await _stockService.UpdateStockAsync(xchgCode, stkCode, request, cancellationToken);

        return Ok(updatedStock, "Stock updated successfully");
    }

    /// <summary>
    /// Delete Stock (soft delete)
    /// </summary>
    /// <param name="xchgCode">Exchange Code</param>
    /// <param name="stkCode">Stock Code</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete-stock/{xchgCode}/{stkCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 501)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStock(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(20)] string stkCode,
        [FromBody] DeleteStockRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);

        request ??= new DeleteStockRequest { XchgCode = xchgCode, StkCode = stkCode };
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;

        var result = await _stockService.DeleteStockAsync(xchgCode, stkCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete Stock");
        }

        return Ok(new object(), "Stock deleted successfully");
    }

    /// <summary>
    /// Check if Stock exists
    /// </summary>
    /// <param name="xchgCode">Exchange Code</param>
    /// <param name="stkCode">Stock Code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("stock-exists/{xchgCode}/{stkCode}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult> CheckStockExists(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(20)] string stkCode,
        CancellationToken cancellationToken = default)
    {
        var exists = await _stockService.StockExistsAsync(xchgCode, stkCode, cancellationToken);

        return exists ? Ok() : NotFound();
    }

    #region Work Flow

    /// <summary>
    /// Get paginated list of unauthorized Stocks for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for stock name</param>
    /// <param name="isAuth">Authorization status (0: Unauthorized, 2: Denied)</param>
    /// <param name="sortColumn">Sort column (default: StkCode)</param>
    /// <param name="sortDirection">Sort direction (default: ASC)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized stocks</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 501)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockDto>>>> GetStockUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery, Range(0, 2)] int isAuth = 0,
        [FromQuery] string sortColumn = "StkCode",
        [FromQuery] string sortDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} Stock list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        var result = await _stockService.GetStockUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            isAuth: isAuth,
            sortColumn: sortColumn,
            sortDirection: sortDirection,
            cancellationToken: cancellationToken);

        return Ok(result, $"Stocks {authAction} data retrieved successfully");
    }

    /// <summary>
    /// Authorize Stock in workflow
    /// </summary>
    /// <param name="xchgCode">Exchange Code</param>
    /// <param name="stkCode">Stock Code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{xchgCode}/{stkCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 501)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeStockWF(
        [FromRoute, Required, MaxLength(10)] string xchgCode,
        [FromRoute, Required, MaxLength(20)] string stkCode,
        [FromBody, Required] AuthorizeStockRequest request,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing Stock in workflow: {XchgCode}-{StkCode} Auth Action: {authAction}", xchgCode, stkCode, authAction);

        request.XchgCode = xchgCode;
        request.StkCode = stkCode;

        var result = await _stockService.AuthorizeStockAsync(xchgCode, stkCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {authAction} Stock");
        }

        return Ok(new object(), $"Stock {authAction} successfully");
    }

    #endregion
}