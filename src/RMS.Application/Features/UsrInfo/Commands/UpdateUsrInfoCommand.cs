using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Application.Models.DTOs;
using RMS.Application.Models.Requests;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Commands
{
    // Keep existing command class - NO CHANGES NEEDED
    public class UpdateUsrInfoCommand : IRequest<UsrInfoDto>
    {
        public string UsrId { get; set; } = null!;
        public UpdateUsrInfoRequest Request { get; set; } = new();
    }

    // UPDATE THIS HANDLER to use generic repository
    public class UpdateUsrInfoCommandHandler : IRequestHandler<UpdateUsrInfoCommand, UsrInfoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUsrInfoCommandHandler> _logger;

        public UpdateUsrInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateUsrInfoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UsrInfoDto> Handle(UpdateUsrInfoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating UsrInfo: {UsrId}", request.UsrId);

            try
            {
                // Use the new generic repository for UsrInfo
                var repository = _unitOfWork.Repository<RMS.Domain.Entities.UsrInfo>();
                var usrInfo = await repository.GetByIdAsync<string>(request.UsrId, cancellationToken);

                if (usrInfo == null)
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);

                // Map the update request to the existing entity
                _mapper.Map(request.Request, usrInfo);

                // Update using generic repository
                await repository.UpdateAsync(usrInfo, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var result = _mapper.Map<UsrInfoDto>(usrInfo);
                _logger.LogInformation("Successfully updated UsrInfo: {UsrId}", request.UsrId);

                return result;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Error updating UsrInfo: {UsrId}", request.UsrId);
                throw;
            }
        }
    }
}