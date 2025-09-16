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
/// Title:       Client Controller
/// Author:      Raihan Sharif
/// Purpose:     Manage Client Operations
/// Creation:    16/Sep/2025
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
/// Client controller for managing client information
/// </summary>
[Route("api/v{version:apiVersion}/client")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ClientController : BaseController
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientController> _logger;

    public ClientController(
        IClientService clientService,
        IConfigurationService configurationService,
        ILogger<ClientController> logger)
        : base(configurationService)
    {
        _clientService = clientService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of Clients with optional search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for client name or GCIF</param>
    /// <param name="gcif">Filter by specific GCIF</param>
    /// <param name="clntName">Filter by client name</param>
    /// <param name="clntCode">Filter by client code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of clients</returns>
    [HttpGet("client-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientDto>>>> GetClientList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? gcif = null,
        [FromQuery] string? clntName = null,
        [FromQuery] string? clntCode = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _clientService.GetClientListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            gcif: gcif,
            clntName: clntName,
            clntCode: clntCode,
            cancellationToken: cancellationToken);

        return Ok(result, "Clients retrieved successfully");
    }

    /// <summary>
    /// Get Client by GCIF
    /// </summary>
    /// <param name="gcif">Client GCIF</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Client information</returns>
    [HttpGet("client-by-id/{gcif}")]
    [ProducesResponseType(typeof(ApiResponse<ClientDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ClientDto>>> GetClientById(
        [FromRoute, Required, MaxLength(20)] string gcif,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client by GCIF: {GCIF}", gcif);

        var client = await _clientService.GetClientByIdAsync(gcif, cancellationToken);

        if (client == null)
        {
            _logger.LogWarning("Client not found with GCIF: {GCIF}", gcif);
            return NotFound($"Client with GCIF '{gcif}' not found");
        }

        return Ok(client, "Client retrieved successfully");
    }

    /// <summary>
    /// Create a new Client
    /// </summary>
    /// <param name="request">Client creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created client information</returns>
    [HttpPost("create-client")]
    [ProducesResponseType(typeof(ApiResponse<ClientDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<ClientDto>>> CreateClient(
        [FromBody] CreateClientRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new Client with GCIF: {GCIF}", request.GCIF);

        var result = await _clientService.CreateClientAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetClientById),
            new { gcif = result.GCIF },
            ApiResponse<ClientDto>.SuccessResult(result, "Client created successfully"));
    }

    /// <summary>
    /// Update an existing Client
    /// </summary>
    /// <param name="gcif">Client GCIF</param>
    /// <param name="request">Client update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated client information</returns>
    [HttpPut("update-client/{gcif}")]
    [ProducesResponseType(typeof(ApiResponse<ClientDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<ClientDto>>> UpdateClient(
        [FromRoute, Required, MaxLength(20)] string gcif,
        [FromBody] UpdateClientRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Client with GCIF: {GCIF}", gcif);

        // Set the GCIF from route parameter
        request.GCIF = gcif;

        var result = await _clientService.UpdateClientAsync(gcif, request, cancellationToken);

        return Ok(result, "Client updated successfully");
    }

    /// <summary>
    /// Delete a Client (soft delete)
    /// </summary>
    /// <param name="gcif">Client GCIF</param>
    /// <param name="request">Client deletion request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("delete-client/{gcif}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteClient(
        [FromRoute, Required, MaxLength(20)] string gcif,
        [FromBody] DeleteClientRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Client with GCIF: {GCIF}", gcif);

        // Set the GCIF from route parameter
        request.GCIF = gcif;

        var result = await _clientService.DeleteClientAsync(gcif, request, cancellationToken);

        return Ok(result, "Client deleted successfully");
    }

    /// <summary>
    /// Check if Client exists by GCIF
    /// </summary>
    /// <param name="gcif">Client GCIF</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Existence status</returns>
    [HttpGet("client-exists/{gcif}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<bool>>> ClientExists(
        [FromRoute, Required, MaxLength(20)] string gcif,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking if Client exists with GCIF: {GCIF}", gcif);

        var exists = await _clientService.ClientExistsAsync(gcif, cancellationToken);

        return Ok(exists, $"Client existence check completed for GCIF: {gcif}");
    }

    #region Workflow Operations

    /// <summary>
    /// Get paginated list of unauthorized/denied Clients for workflow approval
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for client name or GCIF</param>
    /// <param name="gcif">Filter by specific GCIF</param>
    /// <param name="clntName">Filter by client name</param>
    /// <param name="clntCode">Filter by client code</param>
    /// <param name="isAuth">Authorization status (0: Unauthorized, 2: Denied)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of clients awaiting authorization</returns>
    [HttpGet("workflow-list")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClientDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClientDto>>>> GetClientWorkflowList(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? gcif = null,
        [FromQuery] string? clntName = null,
        [FromQuery] string? clntCode = null,
        [FromQuery] int isAuth = (int)AuthTypeEnum.UnAuthorize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}",
            pageNumber, pageSize, isAuth);

        var result = await _clientService.GetClientUnAuthDeniedListAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            searchTerm: searchTerm,
            gcif: gcif,
            clntName: clntName,
            clntCode: clntCode,
            isAuth: isAuth,
            cancellationToken: cancellationToken);

        return Ok(result, "Client workflow list retrieved successfully");
    }

    /// <summary>
    /// Authorize or deny a Client in the workflow
    /// </summary>
    /// <param name="gcif">Client GCIF</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization status</returns>
    [HttpPut("authorize-client/{gcif}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<bool>>> AuthorizeClient(
        [FromRoute, Required, MaxLength(20)] string gcif,
        [FromBody] AuthorizeClientRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing Client with GCIF: {GCIF}, IsAuth: {IsAuth}", gcif, request.IsAuth);

        // Set the GCIF from route parameter
        request.GCIF = gcif;

        var result = await _clientService.AuthorizeClientAsync(gcif, request, cancellationToken);

        var action = request.IsAuth == (byte)AuthTypeEnum.Approve ? "approved" : "denied";
        return Ok(result, $"Client {action} successfully");
    }

    #endregion
}