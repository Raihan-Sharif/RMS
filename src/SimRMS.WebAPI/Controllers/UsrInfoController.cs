using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SimRMS.Application.Features.UsrInfo.Commands;
using SimRMS.Application.Features.UsrInfo.Queries;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;
using SimRMS.Domain.Exceptions;

namespace SimRMS.WebAPI.Controllers
{
    /// <summary>
    /// User Information Management API
    /// Handles CRUD operations for user information from the database
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsrInfoController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsrInfoController> _logger;

        public UsrInfoController(
            IMediator mediator,
            IConfigurationService configurationService,
            ILogger<UsrInfoController> logger)
            : base(configurationService)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of user information with optional filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="usrStatus">Filter by user status (A=Active, S=Suspend, C=Close)</param>
        /// <param name="coCode">Filter by company code</param>
        /// <param name="dlrCode">Filter by dealer code</param>
        /// <param name="rmsType">Filter by RMS type</param>
        /// <param name="searchTerm">Search in UsrId, UsrName, or UsrEmail</param>
        /// <returns>Paginated list of user information</returns>
        [HttpGet]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UsrInfoDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        public async Task<ActionResult<ApiResponse<PagedResult<UsrInfoDto>>>> GetUsrInfos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? usrStatus = null,
            [FromQuery] string? coCode = null,
            [FromQuery] string? dlrCode = null,
            [FromQuery] string? rmsType = null,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("Getting UsrInfo list - Page: {PageNumber}, Size: {PageSize}, Status: {UsrStatus}, CoCode: {CoCode}, DlrCode: {DlrCode}",
                    pageNumber, pageSize, usrStatus, coCode, dlrCode);

                var result = await _mediator.Send(new GetUsrInfosQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    UsrStatus = usrStatus,
                    CoCode = coCode,
                    DlrCode = dlrCode,
                    RmsType = rmsType,
                    SearchTerm = searchTerm
                });

                return Ok(result, $"Retrieved {result.Data.Count()} records out of {result.TotalCount} total");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UsrInfo list");
                return BadRequest<PagedResult<UsrInfoDto>>("Error retrieving user information list");
            }
        }

        /// <summary>
        /// Get user information by ID
        /// </summary>
        /// <param name="usrId">User ID</param>
        /// <returns>User information details</returns>
        [HttpGet("{usrId}")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<UsrInfoDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> GetUsrInfo(string usrId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usrId))
                {
                    return BadRequest<UsrInfoDto>("User ID is required");
                }

                _logger.LogInformation("Getting UsrInfo by ID: {UsrId}", usrId);

                var result = await _mediator.Send(new GetUsrInfoByIdQuery { UsrId = usrId });
                return Ok(result, "User information retrieved successfully");
            }
            catch (NotFoundException)
            {
                return NotFound<UsrInfoDto>($"User with ID '{usrId}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UsrInfo by ID: {UsrId}", usrId);
                return BadRequest<UsrInfoDto>("Error retrieving user information");
            }
        }

        /// <summary>
        /// Create new user information
        /// </summary>
        /// <param name="request">User information creation request</param>
        /// <returns>Created user information</returns>
        [HttpPost]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<UsrInfoDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> CreateUsrInfo([FromBody] CreateUsrInfoRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest<UsrInfoDto>("Request body is required");
                }

                _logger.LogInformation("Creating new UsrInfo: {UsrId}", request.UsrId);

                var result = await _mediator.Send(new CreateUsrInfoCommand { Request = request });
                return Ok(result, "User information created successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest<UsrInfoDto>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating UsrInfo: {UsrId}", request?.UsrId);
                return BadRequest<UsrInfoDto>("Error creating user information");
            }
        }

        /// <summary>
        /// Update existing user information
        /// </summary>
        /// <param name="usrId">User ID to update</param>
        /// <param name="request">User information update request</param>
        /// <returns>Updated user information</returns>
        [HttpPut("{usrId}")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<UsrInfoDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> UpdateUsrInfo(string usrId, [FromBody] UpdateUsrInfoRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usrId))
                {
                    return BadRequest<UsrInfoDto>("User ID is required");
                }

                if (request == null)
                {
                    return BadRequest<UsrInfoDto>("Request body is required");
                }

                _logger.LogInformation("Updating UsrInfo: {UsrId}", usrId);

                var result = await _mediator.Send(new UpdateUsrInfoCommand { UsrId = usrId, Request = request });
                return Ok(result, "User information updated successfully");
            }
            catch (NotFoundException)
            {
                return NotFound<UsrInfoDto>($"User with ID '{usrId}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating UsrInfo: {UsrId}", usrId);
                return BadRequest<UsrInfoDto>("Error updating user information");
            }
        }

        /// <summary>
        /// Delete user information
        /// </summary>
        /// <param name="usrId">User ID to delete</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{usrId}")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUsrInfo(string usrId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usrId))
                {
                    return BadRequest<bool>("User ID is required");
                }

                _logger.LogInformation("Deleting UsrInfo: {UsrId}", usrId);

                var result = await _mediator.Send(new DeleteUsrInfoCommand { UsrId = usrId });
                return Ok(result, "User information deleted successfully");
            }
            catch (NotFoundException)
            {
                return NotFound<bool>($"User with ID '{usrId}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting UsrInfo: {UsrId}", usrId);
                return BadRequest<bool>("Error deleting user information");
            }
        }

        /// <summary>
        /// Check if user exists (HEAD request for existence check)
        /// </summary>
        /// <param name="usrId">User ID to check</param>
        /// <returns>200 if exists, 404 if not found</returns>
        [HttpHead("{usrId}")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> CheckUsrInfoExists(string usrId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usrId))
                {
                    return BadRequest();
                }

                var query = new GetUsrInfoByIdQuery { UsrId = usrId };
                await _mediator.Send(query);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get user information statistics
        /// </summary>
        /// <returns>User information statistics</returns>
        [HttpGet("statistics")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<ActionResult<ApiResponse<object>>> GetUsrInfoStatistics()
        {
            try
            {
                _logger.LogInformation("Getting UsrInfo statistics");

                // Get all users and calculate statistics
                var allUsersQuery = new GetUsrInfosQuery { PageNumber = 1, PageSize = int.MaxValue };
                var allUsers = await _mediator.Send(allUsersQuery);

                var statistics = new
                {
                    TotalUsers = allUsers.TotalCount,
                    ActiveUsers = allUsers.Data.Count(u => u.UsrStatus == "A"),
                    SuspendedUsers = allUsers.Data.Count(u => u.UsrStatus == "S"),
                    ClosedUsers = allUsers.Data.Count(u => u.UsrStatus == "C"),
                    RmsTypes = allUsers.Data.GroupBy(u => u.RmsType)
                        .Select(g => new { RmsType = g.Key, Count = g.Count() })
                        .ToList(),
                    CompanyCodes = allUsers.Data.Where(u => !string.IsNullOrEmpty(u.CoCode))
                        .GroupBy(u => u.CoCode)
                        .Select(g => new { CoCode = g.Key, Count = g.Count() })
                        .ToList()
                };

                // return Ok(statistics, "Statistics retrieved successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UsrInfo statistics");
                return BadRequest<object>("Error retrieving statistics");
            }
        }
    }
}

