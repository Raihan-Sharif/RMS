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
/// Title:       Client Exposure Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage Client Exposure Operations
/// Creation:    17/Sep/2025
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
/// Client Exposure controller for managing client exposure information
/// </summary>
[Route("api/v{version:apiVersion}/client-exposure")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ClientExposureController : BaseController
{
    private readonly IClientExposureService _clientExposureService;
    private readonly ILogger<ClientExposureController> _logger;

    public ClientExposureController(
        IClientExposureService clientExposureService,
        IConfigurationService configurationService,
        ILogger<ClientExposureController> logger)
        : base(configurationService)
    {
        _clientExposureService = clientExposureService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Client Exposures with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="clntCode">Client code filter</param>
    /// <param name="coBrchCode">Company branch code filter</param>
    /// <param name="searchTerm">Search term for client name or email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of client exposures</returns>
    [HttpGet("exposure-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientExposureDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientExposureDto>>>> GetClientExposureList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? clntCode = null,
        [FromQuery] string? coBrchCode = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting ClientExposure list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _clientExposureService.GetClientExposureListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            clntCode: clntCode,
            coBrchCode: coBrchCode,
            searchTerm: searchTerm,
            cancellationToken: cancellationToken);

        return Ok(result, "Client Exposures retrieved successfully");
    }

    /// <summary>
    /// Get Client Exposure by Client Code and Branch Code
    /// </summary>
    /// <param name="clntCode">Client Code</param>
    /// <param name="coBrchCode">Company Branch Code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Client exposure information</returns>
    [HttpGet("exposure-by-id/{clntCode}/{coBrchCode}")]
    [ProducesResponseType(typeof(ApiResponse<ClientExposureDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ClientExposureDto>>> GetClientExposureById(
        [FromRoute, Required, MaxLength(50)] string clntCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting ClientExposure by ID: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);

        var exposure = await _clientExposureService.GetClientExposureByIdAsync(clntCode, coBrchCode, cancellationToken);

        if (exposure == null)
        {
            return NotFound<ClientExposureDto>($"Client Exposure with Client Code '{clntCode}' and Branch Code '{coBrchCode}' not found");
        }

        return Ok(exposure, "Client Exposure retrieved successfully");
    }

    /// <summary>
    /// Create new Client Exposure
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created client exposure information</returns>
    [HttpPost("create-exposure")]
    [ProducesResponseType(typeof(ApiResponse<ClientExposureDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<ClientExposureDto>>> CreateClientExposure(
        [FromBody, Required] CreateClientExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating ClientExposure: {ClntCode}, {CoBrchCode}", request.ClntCode, request.CoBrchCode);

        var createdExposure = await _clientExposureService.CreateClientExposureAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetClientExposureById),
            new { clntCode = createdExposure.ClntCode, coBrchCode = createdExposure.CoBrchCode },
            new ApiResponse<ClientExposureDto>
            {
                Success = true,
                Data = createdExposure,
                Message = "Client Exposure created successfully"
            });
    }

    /// <summary>
    /// Update Client Exposure information
    /// </summary>
    /// <param name="clntCode">Client Code</param>
    /// <param name="coBrchCode">Company Branch Code</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated client exposure information</returns>
    [HttpPut("update-exposure/{clntCode}/{coBrchCode}")]
    [ProducesResponseType(typeof(ApiResponse<ClientExposureDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ClientExposureDto>>> UpdateClientExposure(
        [FromRoute, Required, MaxLength(50)] string clntCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        [FromBody, Required] UpdateClientExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating ClientExposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);

        request.ClntCode = clntCode;
        request.CoBrchCode = coBrchCode;

        var updatedExposure = await _clientExposureService.UpdateClientExposureAsync(clntCode, coBrchCode, request, cancellationToken);

        return Ok(updatedExposure, "Client Exposure updated successfully");
    }

    /// <summary>
    /// Delete Client Exposure (soft delete)
    /// </summary>
    /// <param name="clntCode">Client Code</param>
    /// <param name="coBrchCode">Company Branch Code</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete-exposure/{clntCode}/{coBrchCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteClientExposure(
        [FromRoute, Required, MaxLength(50)] string clntCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        [FromBody] DeleteClientExposureRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting ClientExposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);

        request ??= new DeleteClientExposureRequest { ClntCode = clntCode, CoBrchCode = coBrchCode };
        request.ClntCode = clntCode;
        request.CoBrchCode = coBrchCode;

        var result = await _clientExposureService.DeleteClientExposureAsync(clntCode, coBrchCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>("Failed to delete Client Exposure");
        }

        return Ok(new object(), "Client Exposure deleted successfully");
    }

    /// <summary>
    /// Check if Client Exposure exists
    /// </summary>
    /// <param name="clntCode">Client Code</param>
    /// <param name="coBrchCode">Company Branch Code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating existence</returns>
    [HttpHead("exposure-exists/{clntCode}/{coBrchCode}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult> CheckClientExposureExists(
        [FromRoute, Required, MaxLength(50)] string clntCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        CancellationToken cancellationToken = default)
    {
        var exists = await _clientExposureService.ClientExposureExistsAsync(clntCode, coBrchCode, cancellationToken);

        return exists ? Ok() : NotFound();
    }

    #region Work Flow

    /// <summary>
    /// Get paginated list of unauthorized Client Exposures for workflow
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="clntCode">Client code filter</param>
    /// <param name="coBrchCode">Company branch code filter</param>
    /// <param name="searchTerm">Search term for client name or email</param>
    /// <param name="isAuth">Authorization status (0: Unauthorized, 2: Denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unauthorized client exposures</returns>
    [HttpGet("wf/unauth-denied-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientExposureDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientExposureDto>>>> GetClientExposureUnAuthDeniedListWF(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? clntCode = null,
        [FromQuery] string? coBrchCode = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery, Range(0, 2)] int isAuth = 0,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} ClientExposure list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        var result = await _clientExposureService.GetClientExposureUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            clntCode: clntCode,
            coBrchCode: coBrchCode,
            searchTerm: searchTerm,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, $"Client Exposures {authAction} data retrieved successfully");
    }

    /// <summary>
    /// Authorize Client Exposure in workflow
    /// </summary>
    /// <param name="clntCode">Client Code</param>
    /// <param name="coBrchCode">Company Branch Code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    [HttpPost("wf/authorize/{clntCode}/{coBrchCode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<object>>> AuthorizeClientExposureWF(
        [FromRoute, Required, MaxLength(50)] string clntCode,
        [FromRoute, Required, MaxLength(6)] string coBrchCode,
        [FromBody, Required] AuthorizeClientExposureRequest request,
        CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing ClientExposure in workflow: {ClntCode}, {CoBrchCode} Auth Action: {authAction}", clntCode, coBrchCode, authAction);

        request.ClntCode = clntCode;
        request.CoBrchCode = coBrchCode;

        var result = await _clientExposureService.AuthorizeClientExposureAsync(clntCode, coBrchCode, request, cancellationToken);

        if (!result)
        {
            return BadRequest<object>($"Failed to {authAction} Client Exposure");
        }

        return Ok(new object(), $"Client Exposure {authAction} successfully");
    }

    #endregion
}