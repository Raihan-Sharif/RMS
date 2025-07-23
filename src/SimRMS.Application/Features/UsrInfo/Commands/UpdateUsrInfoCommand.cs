using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Interfaces;
using SimRMS.Domain.Entities;
using SimRMS.Domain.Exceptions;

namespace SimRMS.Application.Features.UsrInfo.Commands
{
    // UPDATE COMMAND
    public class UpdateUsrInfoCommand : IRequest<UsrInfoDto>
    {
        public string UsrId { get; set; } = null!;
        public UpdateUsrInfoRequest Request { get; set; } = new();
    }

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
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Business validation - Get existing user
                var existingUser = await _unitOfWork.UsrInfoRepository.GetByUserIdAsync(request.UsrId, cancellationToken);
                if (existingUser == null)
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);

                // Business validation - Check for duplicate email if provided and different from current
                if (!string.IsNullOrEmpty(request.Request.UsrEmail) &&
                    request.Request.UsrEmail != existingUser.UsrEmail)
                {
                    var emailExists = await _unitOfWork.UsrInfoRepository.ExistsByEmailAsync(request.Request.UsrEmail, request.UsrId, cancellationToken);
                    if (emailExists)
                    {
                        throw new DomainException($"Another user with email '{request.Request.UsrEmail}' already exists");
                    }
                }

                // Map update request to existing entity (only non-null values)
                _mapper.Map(request.Request, existingUser);

                // Use transaction for data integrity
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Update through domain repository
                    var updatedUser = await _unitOfWork.UsrInfoRepository.UpdateUserAsync(existingUser, cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    // Map to DTO for response
                    var result = _mapper.Map<UsrInfoDto>(updatedUser);
                    _logger.LogInformation("Successfully updated UsrInfo: {UsrId}", request.UsrId);

                    return result;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Error updating UsrInfo: {UsrId}", request.UsrId);
                throw;
            }
        }
    }
}