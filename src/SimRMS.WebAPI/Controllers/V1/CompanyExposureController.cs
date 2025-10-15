using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Models;
using SimRMS.WebAPI.Controllers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company Exposure Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Company with Exposure Operations
/// Creation:    11/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 


namespace SimRMS.WebAPI.Controllers.V1
{
    /// <summary>
    /// Controller for Company with Exposure operations
    /// Demonstrates Table Value Parameter usage with bulk operations
    /// </summary>
    [Route("api/v{version:apiVersion}/company-exposure")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class CompanyExposureController : BaseController
    {
        private readonly ICompanyExposureService _companyExposureService;

        public CompanyExposureController(ICompanyExposureService companyExposureService, IConfigurationService configurationService) 
            : base(configurationService)
        {
            _companyExposureService = companyExposureService;
        }

        /// <summary>
        /// Get paginated list of companies with exposure
        /// </summary>
        [HttpGet("company-list")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MstCompanyWithExposureDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<MstCompanyWithExposureDto>>>> GetCompaniesWithExposure(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 1000)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _companyExposureService.GetCompaniesWithExposurePagedAsync(
                pageNumber, pageSize, searchTerm, cancellationToken);

            return Ok(result, "The Companies with exposure retrieved successfully");
        }

        /// <summary>
        /// Get company with exposure by code
        /// </summary>
        [HttpGet("company-by-id/{coCode}")]
        [ProducesResponseType(typeof(ApiResponse<MstCompanyWithExposureDto>), 200)]
        public async Task<ActionResult<ApiResponse<MstCompanyWithExposureDto>>> GetCompanyWithExposure(
            [Required] string coCode,
            CancellationToken cancellationToken = default)
        {
            var result = await _companyExposureService.GetCompanyWithExposureByCodeAsync(coCode, cancellationToken);

            if (result == null)
            {
                return NotFound<MstCompanyWithExposureDto>($"The Company with code {coCode} was not found");
            }

            return Ok(result, "The Company with exposure retrieved successfully");
        }

        /// <summary>
        /// Bulk create companies with exposure using Table Value Parameters
        /// Demonstrates high-performance bulk operations with your custom LB DAL
        /// </summary>
        [HttpPost("bulk-create")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), 200)]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkCreateCompaniesWithExposure(
            [FromBody, Required] IEnumerable<CreateCompanyExposureRequest> requests,
            CancellationToken cancellationToken = default)
        {
            if (!requests.Any())
            {
                return BadRequest<BulkOperationResult>("At least one company is required for bulk creation");
            }

            if (requests.Count() > 1000)
            {
                return BadRequest<BulkOperationResult>("Maximum 1000 companies allowed per bulk operation");
            }

            var result = await _companyExposureService.BulkCreateCompaniesWithExposureAsync(requests, cancellationToken);

            if (result.Success)
                return Ok(result.Data!, result.Message);
            else
                return BadRequest<BulkOperationResult>(result.Message);
        }

        /// <summary>
        /// Bulk update companies with exposure using Table Value Parameters
        /// </summary>
        [HttpPut("bulk-update")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), 200)]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkUpdateCompaniesWithExposure(
            [FromBody, Required] IEnumerable<UpdateCompanyExposureRequest> requests,
            CancellationToken cancellationToken = default)
        {
            if (!requests.Any())
            {
                return BadRequest<BulkOperationResult>("At least one company is required for bulk update");
            }

            if (requests.Count() > 1000)
            {
                return BadRequest<BulkOperationResult>("Maximum 1000 companies allowed per bulk operation");
            }

            var result = await _companyExposureService.BulkUpdateCompaniesWithExposureAsync(requests, cancellationToken);

            if (result.Success)
                return Ok(result.Data!, result.Message);
            else
                return BadRequest<BulkOperationResult>(result.Message);
        }

        /// <summary>
        /// Bulk upsert (insert or update) companies with exposure using Table Value Parameters
        /// This operation will insert new companies and update existing ones in a single call
        /// </summary>
        [HttpPost("bulk-upsert")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), 200)]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkUpsertCompaniesWithExposure(
            [FromBody, Required] IEnumerable<UpsertCompanyExposureRequest> requests,
            CancellationToken cancellationToken = default)
        {
            if (!requests.Any())
            {
                return BadRequest<BulkOperationResult>("At least one company is required for bulk upsert");
            }

            if (requests.Count() > 1000)
            {
                return BadRequest<BulkOperationResult>("Maximum 1000 companies allowed per bulk operation");
            }

            var result = await _companyExposureService.BulkUpsertCompaniesWithExposureAsync(requests, cancellationToken);

            if (result.Success)
                return Ok(result.Data!, result.Message);
            else
                return BadRequest<BulkOperationResult>(result.Message);
        }
    }
}