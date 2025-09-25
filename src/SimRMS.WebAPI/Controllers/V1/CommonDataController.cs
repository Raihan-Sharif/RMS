using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Shared.Models;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Common Data Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Provide Read-Only Common Data Endpoints for Dropdowns and Lists
/// Creation:    01/Sep/2025
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
/// Common Data controller for read-only operations like dropdowns and lists
/// </summary>
[Route("api/v{version:apiVersion}/common-data")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class CommonDataController : BaseController
{
    private readonly ICommonDataService _commonDataService;
    private readonly ILogger<CommonDataController> _logger;

    public CommonDataController(
        ICommonDataService commonDataService,
        IConfigurationService configurationService,
        ILogger<CommonDataController> logger)
        : base(configurationService)
    {
        _commonDataService = commonDataService;
        _logger = logger;
    }

    /// <summary>
    /// Get list of branches for dropdown/selection
    /// </summary>
    /// <param name="companyCode">Optional filter by company code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of branches</returns>
    [HttpGet("branch-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BranchListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BranchListDto>>>> GetBranchList(
        [FromQuery] string? companyCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting branch list with companyCode filter: {CompanyCode}", companyCode);

        var branches = await _commonDataService.GetBranchListAsync(companyCode, cancellationToken);
        return Ok(branches, "Branch list retrieved successfully");
    }

    /// <summary>
    /// Get list of companies for dropdown/selection
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of companies</returns>
    [HttpGet("company-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompanyListDto>>>> GetCompanyList(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting company list");

        var companies = await _commonDataService.GetCompanyListAsync(cancellationToken);
        return Ok(companies, "Company list retrieved successfully");
    }

    /// <summary>
    /// Get list of traders for dropdown/selection
    /// </summary>
    /// <param name="exchangeCode">Optional filter by exchange code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of traders</returns>
    [HttpGet("trader-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TraderListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TraderListDto>>>> GetTraderList(
        [FromQuery] string? exchangeCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting trader list with exchangeCode filter: {ExchangeCode}", exchangeCode);

        var traders = await _commonDataService.GetTraderListAsync(exchangeCode, cancellationToken);
        return Ok(traders, "Trader list retrieved successfully");
    }

    /// <summary>
    /// Get list of users for dropdown/selection
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    [HttpGet("user-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserListDto>>>> GetUserList(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user list");

        var users = await _commonDataService.GetUserListAsync(cancellationToken);
        return Ok(users, "User list retrieved successfully");
    }

    /// <summary>
    /// Get list of countries for dropdown/selection
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of countries</returns>
    [HttpGet("country-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CountryListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CountryListDto>>>> GetCountryList(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting country list");

        var countries = await _commonDataService.GetCountryListAsync(cancellationToken);
        return Ok(countries, "Country list retrieved successfully");
    }

    /// <summary>
    /// Get list of client types for dropdown/selection
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of client types</returns>
    [HttpGet("client-type-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientTypeListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientTypeListDto>>>> GetClientTypeList(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting client type list");

        var clientTypes = await _commonDataService.GetClientTypeListAsync(cancellationToken);
        return Ok(clientTypes, "Client type list retrieved successfully");
    }

    /// <summary>
    /// Get list of clients for dropdown/selection
    /// </summary>
    /// <param name="branchCode">Optional branch code filter - if provided, returns only clients for that branch; otherwise returns all clients</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of clients with their details</returns>
    [HttpGet("client-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientListDto>>>> GetClientList(
        [FromQuery] string? branchCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting client list {BranchFilter}",
            string.IsNullOrEmpty(branchCode) ? "for all branches" : $"for branch: {branchCode}");

        var clients = await _commonDataService.GetClientListAsync(branchCode, cancellationToken);
        return Ok(clients, string.IsNullOrEmpty(branchCode)
            ? "Client list retrieved successfully"
            : $"Client list retrieved successfully for branch: {branchCode}");
    }

    /// <summary>
    /// Get list of stock exchanges for dropdown/selection
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of stock exchanges</returns>
    [HttpGet("stock-exchange-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockExchangeListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockExchangeListDto>>>> GetStockExchangeList(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting stock exchange list");

        var stockExchanges = await _commonDataService.GetStockExchangeListAsync(cancellationToken);
        return Ok(stockExchanges, "Stock exchange list retrieved successfully");
    }

    /// <summary>
    /// Get list of stock boards for dropdown/selection
    /// </summary>
    /// <param name="exchangeCode">Optional filter by exchange code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of stock boards</returns>
    [HttpGet("stock-board-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockBoardListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockBoardListDto>>>> GetStockBoardList(
        [FromQuery] string? exchangeCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting stock board list with exchangeCode filter: {ExchangeCode}", exchangeCode);

        var stockBoards = await _commonDataService.GetStockBoardListAsync(exchangeCode, cancellationToken);
        return Ok(stockBoards, "Stock board list retrieved successfully");
    }

    /// <summary>
    /// Get list of stock board markets for dropdown/selection
    /// </summary>
    /// <param name="exchangeCode">Optional filter by exchange code</param>
    /// <param name="boardCode">Optional filter by board code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of stock board markets</returns>
    [HttpGet("stock-board-market-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockBoardMarketListDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockBoardMarketListDto>>>> GetStockBoardMarketList(
        [FromQuery] string? exchangeCode = null,
        [FromQuery] string? boardCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting stock board market list with exchangeCode: {ExchangeCode}, boardCode: {BoardCode}", exchangeCode, boardCode);

        var stockBoardMarkets = await _commonDataService.GetStockBoardMarketListAsync(exchangeCode, boardCode, cancellationToken);
        return Ok(stockBoardMarkets, "Stock board market list retrieved successfully");
    }
}