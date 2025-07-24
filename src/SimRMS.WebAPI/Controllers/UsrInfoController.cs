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
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UsrInfoDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsrInfoDto>>>> GetUsrInfos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? usrStatus = null,
            [FromQuery] string? coCode = null,
            [FromQuery] string? dlrCode = null,
            [FromQuery] string? rmsType = null,
            [FromQuery] string? searchTerm = null)
        {

            _logger.LogInformation("Getting UsrInfo list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

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

            // Use the new paged result method
            return Ok(result, $"Retrieved {result.Data.Count()} records out of {result.TotalCount} total");

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
            if (string.IsNullOrWhiteSpace(usrId))
            {
                return BadRequest<UsrInfoDto>("User ID is required");
            }

            _logger.LogInformation("Getting UsrInfo by ID: {UsrId}", usrId);

            var result = await _mediator.Send(new GetUsrInfoByIdQuery { UsrId = usrId });
            return Ok(result, "User information retrieved successfully");

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

            if (request == null)
            {
                return BadRequest<UsrInfoDto>("Request body is required");
            }

            _logger.LogInformation("Creating new UsrInfo: {UsrId}", request.UsrId);

            var result = await _mediator.Send(new CreateUsrInfoCommand { Request = request });
            return Ok(result, "User information created successfully");

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

            if (string.IsNullOrWhiteSpace(usrId))
            {
                return BadRequest<bool>("User ID is required");
            }

            _logger.LogInformation("Deleting UsrInfo: {UsrId}", usrId);

            var result = await _mediator.Send(new DeleteUsrInfoCommand { UsrId = usrId });
            return Ok(result, "User information deleted successfully");


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

            if (string.IsNullOrWhiteSpace(usrId))
            {
                return BadRequest();
            }

            var query = new GetUsrInfoByIdQuery { UsrId = usrId };
           var user = await _mediator.Send(query);

            return Ok();

        }

        /// <summary>
        /// Get user information statistics
        /// </summary>
        /// <returns>User information statistics</returns>
        [HttpGet("statistics")]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(ApiResponse<UserStatisticsDto>), 200)]
        public async Task<ActionResult<ApiResponse<UserStatisticsDto>>> GetUsrInfoStatistics()
        {
            _logger.LogInformation("Getting UsrInfo statistics");

            // Use the dedicated statistics query instead of loading all users
            var statistics = await _mediator.Send(new GetUsrInfoStatisticsQuery());

            return Ok(statistics, "Statistics retrieved successfully");
        }
    }
}

