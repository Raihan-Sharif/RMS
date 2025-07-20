using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Features.Users.Queries;
using RMS.Application.Interfaces;
using RMS.Application.Models.DTOs;
using RMS.Shared.Constants;
using RMS.Shared.Models;
using MediatR;

namespace RMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator, IConfigurationService configurationService)
            : base(configurationService)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ViewUsers")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "ViewUsers")]
        public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            return Ok(result);
        }
    }
}
