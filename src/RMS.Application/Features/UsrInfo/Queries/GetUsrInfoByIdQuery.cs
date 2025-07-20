using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Application.Models.DTOs;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Queries
{
    // Keep existing query class - NO CHANGES NEEDED
    public class GetUsrInfoByIdQuery : IRequest<UsrInfoDto>
    {
        public string UsrId { get; set; } = null!;
    }

    // UPDATE THIS HANDLER to use generic repository
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

                // Use the new generic repository for UsrInfo
                var repository = _unitOfWork.Repository<RMS.Domain.Entities.UsrInfo>();
                var usrInfo = await repository.GetByIdAsync<string>(request.UsrId.Trim(), cancellationToken);

                if (usrInfo == null)
                {
                    _logger.LogWarning("UsrInfo not found for UsrId: {UsrId}", request.UsrId);
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);
                }

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