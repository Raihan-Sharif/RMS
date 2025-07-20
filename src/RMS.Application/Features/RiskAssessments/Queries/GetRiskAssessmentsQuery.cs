using AutoMapper;
using MediatR;
using RMS.Application.Models.DTOs;
using RMS.Domain.Entities;
using RMS.Domain.Interfaces;
using RMS.Shared.Extensions;
using RMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Features.RiskAssessments.Queries
{
    public class GetRiskAssessmentsQuery : IRequest<PagedResult<RiskAssessmentDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Status { get; set; }
        public string? RiskCategory { get; set; }
    }

    public class GetRiskAssessmentsQueryHandler : IRequestHandler<GetRiskAssessmentsQuery, PagedResult<RiskAssessmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRiskAssessmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<RiskAssessmentDto>> Handle(GetRiskAssessmentsQuery request, CancellationToken cancellationToken)
        {
            var predicate = BuildPredicate(request);
            var pagedResult = await _unitOfWork.RiskAssessments.GetPagedAsync(
                request.PageNumber, request.PageSize, predicate, cancellationToken);

            return new PagedResult<RiskAssessmentDto>
            {
                Data = _mapper.Map<IEnumerable<RiskAssessmentDto>>(pagedResult.Data),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        private static Expression<Func<RiskAssessment, bool>>? BuildPredicate(GetRiskAssessmentsQuery request)
        {
            Expression<Func<RiskAssessment, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(request.Status))
            {
                predicate = x => x.Status == request.Status;
            }

            if (!string.IsNullOrEmpty(request.RiskCategory))
            {
                var categoryPredicate = (Expression<Func<RiskAssessment, bool>>)(x => x.RiskCategory == request.RiskCategory);
                predicate = predicate == null ? categoryPredicate : predicate.And(categoryPredicate);
            }

            return predicate;
        }
    }
}
