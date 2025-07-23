using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Interfaces;
using SimRMS.Domain.Entities;
using SimRMS.Domain.Exceptions;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Features.UsrInfo.Queries
{
    // GET BY ID QUERY
    public class GetUsrInfoByIdQuery : IRequest<UsrInfoDto>
    {
        public string UsrId { get; set; } = null!;
    }

    public class GetUsrInfoByIdQueryHandler : IRequestHandler<GetUsrInfoByIdQuery, UsrInfoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsrInfoByIdQueryHandler> _logger;

        public GetUsrInfoByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetUsrInfoByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UsrInfoDto> Handle(GetUsrInfoByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting UsrInfo by UsrId: {UsrId}", request.UsrId);

            try
            {
                if (string.IsNullOrWhiteSpace(request.UsrId))
                {
                    _logger.LogWarning("GetUsrInfoByIdQuery called with null or empty UsrId");
                    throw new ArgumentException("UsrId cannot be null or empty", nameof(request.UsrId));
                }

                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Get user through domain repository
                var usrInfo = await _unitOfWork.UsrInfoRepository.GetByUserIdAsync(request.UsrId.Trim(), cancellationToken);

                if (usrInfo == null)
                {
                    _logger.LogWarning("UsrInfo not found for UsrId: {UsrId}", request.UsrId);
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);
                }

                // Map to DTO for response
                var result = _mapper.Map<UsrInfoDto>(usrInfo);
                _logger.LogInformation("Successfully retrieved UsrInfo for UsrId: {UsrId}", request.UsrId);

                return result;
            }
            catch (Exception ex) when (!(ex is NotFoundException || ex is ArgumentException))
            {
                _logger.LogError(ex, "Error getting UsrInfo by UsrId: {UsrId}", request.UsrId);
                throw;
            }
        }
    }
}