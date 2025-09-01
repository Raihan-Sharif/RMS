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
}