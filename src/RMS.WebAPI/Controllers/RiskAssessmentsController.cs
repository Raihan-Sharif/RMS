using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Features.RiskAssessments.Queries;
using RMS.Application.Interfaces;
using RMS.Application.Models.DTOs;
using RMS.Shared.Constants;
using RMS.Shared.Models;
using MediatR;

namespace RMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RiskAssessmentsController : BaseController
    {
        private readonly IMediator _mediator;

        public RiskAssessmentsController(IMediator mediator, IConfigurationService configurationService)
            : base(configurationService)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "ViewRisks")]
        public async Task<ActionResult<ApiResponse<PagedResult<RiskAssessmentDto>>>> GetRiskAssessments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? riskCategory = null)
        {
            var result = await _mediator.Send(new GetRiskAssessmentsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = status,
                RiskCategory = riskCategory
            });
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "ViewRisks")]
        public async Task<ActionResult<ApiResponse<RiskAssessmentDto>>> GetRiskAssessment(Guid id)
        {
            var result = await _mediator.Send(new GetRiskAssessmentByIdQuery { Id = id });
            return Ok(result);
        }
    }
}
