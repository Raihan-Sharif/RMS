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
/// Title:       Stock Exposure Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage Stock Exposure Operations (Stock Trading Control/Suspension)
/// Creation:    08/Oct/2025
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
/// Stock Exposure controller for managing stock trading control/suspension
/// </summary>
[Route("api/v{version:apiVersion}/stock-exposure")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class StockExposureController : BaseController
{
    private readonly IStockExposureService _stockExposureService;
    private readonly ILogger<StockExposureController> _logger;

    public StockExposureController(
        IStockExposureService stockExposureService,
        IConfigurationService configurationService,
        ILogger<StockExposureController> logger)
        : base(configurationService)
    {
        _stockExposureService = stockExposureService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Stock Exposures with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="usrID">User ID filter</param>
    /// <param name="clntCode">Client code filter</param>
    /// <param name="stkCode">Stock code filter</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of stock exposures</returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockExposureDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockExposureDto>>>> GetStockExposureList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? usrID = null,
        [FromQuery] string? clntCode = null,
        [FromQuery] string? stkCode = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting StockExposure list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _stockExposureService.GetStockExposureListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            usrID: usrID,
            clntCode: clntCode,
            stkCode: stkCode,
            searchTerm: searchTerm,
            cancellationToken: cancellationToken);

        return Ok(result, "Stock Exposures retrieved successfully");
    }

    /// <summary>
    /// Get Stock Exposure by composite key
    /// </summary>
    /// <param name="request">Request containing composite key fields</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stock exposure information</returns>
    [HttpPost("by-key")]
    [ProducesResponseType(typeof(ApiResponse<StockExposureDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<StockExposureDto>>> GetStockExposureByKey(
        [FromBody, Required] GetStockExposureByKeyRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting StockExposure by key: DataType={DataType}, StkCode={StkCode}",
            request.DataType, request.StkCode);

        var exposure = await _stockExposureService.GetStockExposureByKeyAsync(request, cancellationToken);

        if (exposure == null)
        {
            return NotFound<StockExposureDto>("Stock Exposure not found");
        }

        return Ok(exposure, "Stock Exposure retrieved successfully");
    }

    /// <summary>
    /// Create new Stock Exposure
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created stock exposure information</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<StockExposureDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<StockExposureDto>>> CreateStockExposure(
        [FromBody, Required] CreateStockExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating StockExposure: DataType={DataType}, StkCode={StkCode}",
            request.DataType, request.StkCode);

        var createdExposure = await _stockExposureService.CreateStockExposureAsync(request, cancellationToken);

        return Created($"api/v1/stock-exposure/by-key",
            ApiResponse<StockExposureDto>.SuccessResult(createdExposure, "Stock Exposure created successfully"));
    }

    /// <summary>
    /// Update Stock Exposure information
    /// </summary>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated stock exposure information</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<StockExposureDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<StockExposureDto>>> UpdateStockExposure(
        [FromBody, Required] UpdateStockExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating StockExposure: DataType={DataType}, StkCode={StkCode}",
            request.DataType, request.StkCode);

        var updatedExposure = await _stockExposureService.UpdateStockExposureAsync(request, cancellationToken);

        return Ok(updatedExposure, "Stock Exposure updated successfully");
    }

    /// <summary>
    /// Delete Stock Exposure (soft delete)
    /// </summary>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStockExposure(
        [FromBody, Required] DeleteStockExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting StockExposure: DataType={DataType}, StkCode={StkCode}",
            request.DataType, request.StkCode);

        var result = await _stockExposureService.DeleteStockExposureAsync(request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete Stock Exposure");
        }

        return Ok(new object(), "Stock Exposure deleted successfully");
    }

    #region Work Flow

    /// <summary>
    /// Get paginated list of unauthorized/denied Stock Exposures for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="usrID">User ID filter</param>
    /// <param name="clntCode">Client code filter</param>
    /// <param name="stkCode">Stock code filter</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="isAuth">Authorization status (0: Unauthorized, 2: Denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized/denied stock exposures</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StockExposureDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockExposureDto>>>> GetStockExposureUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? usrID = null,
        [FromQuery] string? clntCode = null,
        [FromQuery] string? stkCode = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery, Range(0, 2)] int isAuth = 0,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} StockExposure list for workflow - Page: {PageNumber}, Size: {PageSize}",
            pageNumber, pageSize, authAction);

        var result = await _stockExposureService.GetStockExposureUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            usrID: usrID,
            clntCode: clntCode,
            stkCode: stkCode,
            searchTerm: searchTerm,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"Stock Exposures {authAction} data retrieved successfully");
    }

    /// <summary>
    /// Authorize Stock Exposure in workflow
    /// </summary>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeStockExposureWF(
        [FromBody, Required] AuthorizeStockExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing StockExposure in workflow: DataType={DataType}, StkCode={StkCode} Auth Action: {authAction}",
            request.DataType, request.StkCode, authAction);

        var result = await _stockExposureService.AuthorizeStockExposureAsync(request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {authAction} Stock Exposure");
        }

        return Ok(new object(), $"Stock Exposure {authAction} successfully");
    }

    #endregion
}
