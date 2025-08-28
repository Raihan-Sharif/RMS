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
/// Title:       Company Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Company Operations (Read, Update, Authorization)
/// Creation:    28/Aug/2025
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
/// Company controller for managing company information
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class CompanyController : BaseController
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(
        ICompanyService companyService,
        IConfigurationService configurationService,
        ILogger<CompanyController> logger)
        : base(configurationService)
    {
        _companyService = companyService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Companies with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for company code or description</param>
    /// <param name="coCode">Filter by specific company code</param>
    /// <param name="coDesc">Filter by company description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of companies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompanyDto>>>> GetCompanyList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? coCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Company list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _companyService.GetCompanyListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            coCode: coCode,
            cancellationToken: cancellationToken);

        return Ok(result, "Companies retrieved successfully");
    }

    /// <summary>
    /// Get Company by company code
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company information</returns>
    [HttpGet("{coCode}")]
    [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<CompanyDto>>> GetCompanyById(
        [FromRoute, Required, MaxLength(5)] string coCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Company by ID: {CoCode}", coCode);

        var company = await _companyService.GetCompanyByIdAsync(coCode, cancellationToken);

        if (company == null)
        {
            return NotFound<CompanyDto>($"Company with code '{coCode}' not found");
        }

        return Ok(company, "Company retrieved successfully");
    }

    /// <summary>
    /// Update Company information (only CoDesc and EnableExchangeWideSellProceed)
    /// Supports workflow-based authorization via WFName parameter
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="request">Update request with optional WFName for workflow authorization</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated company information</returns>
    [HttpPut("{coCode}")]
    [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<CompanyDto>>> UpdateCompany(
        [FromRoute, Required, MaxLength(5)] string coCode,
        [FromBody, Required] UpdateCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Company: {CoCode}", coCode);

        request.CoCode = coCode;

        var updatedCompany = await _companyService.UpdateCompanyAsync(coCode, request, cancellationToken);

        return Ok(updatedCompany, "Company updated successfully");
    }

    #region Work Flow

    /// <summary>
    /// Get paginated list of unauthorized/denied Companies for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for company code or description</param>
    /// <param name="coCode">Filter by specific company code</param>
    /// <param name="isAuth">Authorization status filter (0: Unauthorized, 2: Denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized/denied companies</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompanyDto>>>> GetCompanyUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? coCode = null,
        [FromQuery, Range(0, 2)] int isAuth = 0,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} Company list for workflow - Page: {PageNumber}, Size: {PageSize}", authAction, pageNumber, pageSize);

        var result = await _companyService.GetCompanyUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            coCode: coCode,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"Companies {authAction} data retrieved successfully");
    }

    /// <summary>
    /// Authorize Company changes via workflow
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{coCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeCompanyWF(
        [FromRoute, Required, MaxLength(5)] string coCode,
        [FromBody, Required] AuthorizeCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing Company in workflow: {CoCode} Auth Action: {authAction}", coCode, authAction);

        request.CoCode = coCode;

        var result = await _companyService.AuthorizeCompanyAsync(coCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {authAction} Company");
        }

        return Ok(new object(), $"Company {authAction} successfully");
    }

    #endregion
}