using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using RMS.Application.Features.UsrInfo.Commands;
using RMS.Application.Features.UsrInfo.Queries;
using RMS.Application.Interfaces;
using RMS.Application.Models.DTOs;
using RMS.Application.Models.Requests;
using RMS.Shared.Models;

namespace RMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UsrInfoController : BaseController
    {
        private readonly IMediator _mediator;

        public UsrInfoController(IMediator mediator, IConfigurationService configurationService)
            : base(configurationService)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "ViewUsers")]
        public async Task<ActionResult<ApiResponse<PagedResult<UsrInfoDto>>>> GetUsrInfos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? usrStatus = null,
            [FromQuery] string? coCode = null,
            [FromQuery] string? dlrCode = null)
        {
            var result = await _mediator.Send(new GetUsrInfosQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                UsrStatus = usrStatus,
                CoCode = coCode,
                DlrCode = dlrCode
            });
            return Ok(result);
        }

        [HttpGet("{usrId}")]
        [Authorize(Policy = "ViewUsers")]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> GetUsrInfo(string usrId)
        {
            var result = await _mediator.Send(new GetUsrInfoByIdQuery { UsrId = usrId });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ManageUsers")]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> CreateUsrInfo([FromBody] CreateUsrInfoRequest request)
        {
            var result = await _mediator.Send(new CreateUsrInfoCommand { Request = request });
            return Ok(result, "User info created successfully");
        }

        [HttpPut("{usrId}")]
        [Authorize(Policy = "ManageUsers")]
        public async Task<ActionResult<ApiResponse<UsrInfoDto>>> UpdateUsrInfo(string usrId, [FromBody] UpdateUsrInfoRequest request)
        {
            var result = await _mediator.Send(new UpdateUsrInfoCommand { UsrId = usrId, Request = request });
            return Ok(result, "User info updated successfully");
        }

        [HttpDelete("{usrId}")]
        [Authorize(Policy = "ManageUsers")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUsrInfo(string usrId)
        {
            var result = await _mediator.Send(new DeleteUsrInfoCommand { UsrId = usrId });
            return Ok(result, "User info deleted successfully");
        }
    }
}
