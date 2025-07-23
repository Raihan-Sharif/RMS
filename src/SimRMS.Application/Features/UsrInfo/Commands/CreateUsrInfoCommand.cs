using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Interfaces;
using SimRMS.Domain.Entities;
using SimRMS.Domain.Exceptions;

namespace SimRMS.Application.Features.UsrInfo.Commands
{
    public class CreateUsrInfoCommand : IRequest<UsrInfoDto>
    {
        public CreateUsrInfoRequest Request { get; set; } = new();
    }

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
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Business validation - Check if user already exists
                var existingUser = await _unitOfWork.UsrInfoRepository.GetByUserIdAsync(request.Request.UsrId, cancellationToken);
                if (existingUser != null)
                {
                    throw new DomainException($"User with ID '{request.Request.UsrId}' already exists");
                }

                // Business validation - Check for duplicate email if provided
                if (!string.IsNullOrEmpty(request.Request.UsrEmail))
                {
                    var emailExists = await _unitOfWork.UsrInfoRepository.ExistsByEmailAsync(request.Request.UsrEmail, null, cancellationToken);
                    if (emailExists)
                    {
                        throw new DomainException($"User with email '{request.Request.UsrEmail}' already exists");
                    }
                }

                // Map request to domain entity
                var usrInfo = _mapper.Map<SimRMS.Domain.Entities.UsrInfo>(request.Request);

                // Use transaction for data integrity
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Create user through domain repository
                    var createdUser = await _unitOfWork.UsrInfoRepository.CreateUserAsync(usrInfo, cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    // Map to DTO for response
                    var result = _mapper.Map<UsrInfoDto>(createdUser);
                    _logger.LogInformation("Successfully created UsrInfo: {UsrId}", request.Request.UsrId);

                    return result;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating UsrInfo: {UsrId}", request.Request.UsrId);
                throw;
            }
        }
    }
}