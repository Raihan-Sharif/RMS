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
/// Title:       Market Stock Company Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Market Stock Company Operations
/// Creation:    12/Aug/2025
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
/// Market Stock Company controller for managing company information
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class MstCoController : BaseController
{
    private readonly IMstCoService _mstCoService;
    private readonly ILogger<MstCoController> _logger;

    public MstCoController(
        IMstCoService mstCoService,
        IConfigurationService configurationService,
        ILogger<MstCoController> logger)
        : base(configurationService)
    {
        _mstCoService = mstCoService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Market Stock Companies with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for company code or description</param>
    /// <param name="coCode">Filter by specific company code</param>
    /// <param name="coDesc">Filter by company description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of companies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MstCoDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MstCoDto>>>> GetMstCoList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? coCode = null,
        [FromQuery] string? coDesc = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting MstCo list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            var result = await _mstCoService.GetMstCoListAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                searchTerm: searchTerm,
                coCode: coCode,
                coDesc: coDesc,
                cancellationToken: cancellationToken);

            return Ok(result, "Market Stock Companies retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MstCo list");
            return BadRequest<IEnumerable<MstCoDto>>("Failed to retrieve Market Stock Companies");
        }
    }

    /// <summary>
    /// Get Market Stock Company by company code
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company information</returns>
    [HttpGet("{coCode}")]
    [ProducesResponseType(typeof(ApiResponse<MstCoDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<MstCoDto>>> GetMstCoById(
        [FromRoute, Required, MaxLength(5)] string coCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting MstCo by ID: {CoCode}", coCode);

            var company = await _mstCoService.GetMstCoByIdAsync(coCode, cancellationToken);

            if (company == null)
            {
                return NotFound<MstCoDto>($"Market Stock Company with code '{coCode}' not found");
            }

            return Ok(company, "Market Stock Company retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid company code: {CoCode}", coCode);
            return BadRequest<MstCoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MstCo by ID: {CoCode}", coCode);
            return BadRequest<MstCoDto>("Failed to retrieve Market Stock Company");
        }
    }

    /// <summary>
    /// Update Market Stock Company information (only CoDesc and EnableExchangeWideSellProceed)
    /// Supports workflow-based authorization via WFName parameter
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="request">Update request with optional WFName for workflow authorization</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated company information</returns>
    [HttpPut("{coCode}")]
    [ProducesResponseType(typeof(ApiResponse<MstCoDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<MstCoDto>>> UpdateMstCo(
        [FromRoute, Required, MaxLength(5)] string coCode,
        [FromBody, Required] UpdateMstCoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating MstCo: {CoCode}", coCode);

            // Ensure the coCode in route matches the request
            request.CoCode = coCode;

            var updatedCompany = await _mstCoService.UpdateMstCoAsync(coCode, request, cancellationToken);

            return Ok(updatedCompany, "Market Stock Company updated successfully");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid update request for MstCo: {CoCode}", coCode);
            return BadRequest<MstCoDto>(ex.Message);
        }
        catch (SimRMS.Domain.Exceptions.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for MstCo update: {CoCode}", coCode);
            var errors = ex.ValidationErrors?.Select(e => e.ErrorMessage).ToList();
            return BadRequest<MstCoDto>("Validation failed", errors);
        }
        catch (SimRMS.Domain.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning(ex, "MstCo not found: {CoCode}", coCode);
            return NotFound<MstCoDto>(ex.Message);
        }
        catch (SimRMS.Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Business rule violation for MstCo: {CoCode}", coCode);
            return BadRequest<MstCoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating MstCo: {CoCode}", coCode);
            return BadRequest<MstCoDto>("Failed to update Market Stock Company");
        }
    }

    /// <summary>
    /// Check if Market Stock Company exists
    /// </summary>
    /// <param name="coCode">Company code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("{coCode}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult> CheckMstCoExists(
        [FromRoute, Required, MaxLength(5)] string coCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _mstCoService.MstCoExistsAsync(coCode, cancellationToken);
            
            return exists ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking MstCo existence: {CoCode}", coCode);
            return BadRequest();
        }
    }
}