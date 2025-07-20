using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Application.Interfaces;
using RMS.Application.Models.DTOs;
using RMS.Application.Models.Requests;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Commands
{
    // Keep existing command class - NO CHANGES NEEDED
    public class CreateUsrInfoCommand : IRequest<UsrInfoDto>
    {
        public CreateUsrInfoRequest Request { get; set; } = new();
    }

    // UPDATE THIS HANDLER to use generic repository
    public class CreateUsrInfoCommandHandler : IRequestHandler<CreateUsrInfoCommand, UsrInfoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CreateUsrInfoCommandHandler> _logger;

        public CreateUsrInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService,
            ILogger<CreateUsrInfoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<UsrInfoDto> Handle(CreateUsrInfoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new UsrInfo: {UsrId}", request.Request.UsrId);

            try
            {
                // Use the new generic repository for UsrInfo
                var repository = _unitOfWork.Repository<RMS.Domain.Entities.UsrInfo>();

                // Check if user already exists
                var existingUser = await repository.GetByIdAsync<string>(request.Request.UsrId, cancellationToken);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"User with ID '{request.Request.UsrId}' already exists");
                }

                var usrInfo = _mapper.Map<RMS.Domain.Entities.UsrInfo>(request.Request);

                // Add the entity using generic repository
                await repository.AddAsync(usrInfo, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var result = _mapper.Map<UsrInfoDto>(usrInfo);
                _logger.LogInformation("Successfully created UsrInfo: {UsrId}", request.Request.UsrId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating UsrInfo: {UsrId}", request.Request.UsrId);
                throw;
            }
        }
    }
}