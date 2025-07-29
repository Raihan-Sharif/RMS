using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Application.Features.UsrInfo
{
    #region Create Command
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
    #endregion

    #region Update Command
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
    #endregion

    #region Delete Command
    public class DeleteUsrInfoCommand : IRequest<bool>
    {
        public string UsrId { get; set; } = null!;
    }

    public class DeleteUsrInfoCommandHandler : IRequestHandler<DeleteUsrInfoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUsrInfoCommandHandler> _logger;

        public DeleteUsrInfoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteUsrInfoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUsrInfoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting UsrInfo: {UsrId}", request.UsrId);

            try
            {
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Business validation - Check if user exists
                var existingUser = await _unitOfWork.UsrInfoRepository.GetByUserIdAsync(request.UsrId, cancellationToken);
                if (existingUser == null)
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);

                // Use transaction for data integrity
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Delete through domain repository
                    var deleted = await _unitOfWork.UsrInfoRepository.DeleteUserAsync(request.UsrId, cancellationToken);

                    if (!deleted)
                    {
                        throw new DomainException($"Failed to delete user with ID '{request.UsrId}'");
                    }

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    _logger.LogInformation("Successfully deleted UsrInfo: {UsrId}", request.UsrId);
                    return true;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Error deleting UsrInfo: {UsrId}", request.UsrId);
                throw;
            }
        }
    }

    #endregion
}
