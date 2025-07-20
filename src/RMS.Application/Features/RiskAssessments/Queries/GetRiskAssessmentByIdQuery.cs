using AutoMapper;
using RMS.Application.Models.DTOs;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;
using MediatR;

namespace RMS.Application.Features.RiskAssessments.Queries
{
    public class GetRiskAssessmentByIdQuery : IRequest<RiskAssessmentDto>
    {
        public Guid Id { get; set; }
    }

    public class GetRiskAssessmentByIdQueryHandler : IRequestHandler<GetRiskAssessmentByIdQuery, RiskAssessmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRiskAssessmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RiskAssessmentDto> Handle(GetRiskAssessmentByIdQuery request, CancellationToken cancellationToken)
        {
            var riskAssessment = await _unitOfWork.RiskAssessments.GetByIdAsync(request.Id, cancellationToken);

            if (riskAssessment == null)
                throw new NotFoundException(nameof(RiskAssessment), request.Id);

            return _mapper.Map<RiskAssessmentDto>(riskAssessment);
        }
    }
}
