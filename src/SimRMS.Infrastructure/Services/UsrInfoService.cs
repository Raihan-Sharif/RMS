using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Mappings;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Validators; // ✅ ADDED: For ValidationContextManager extensions
using SimRMS.Domain.Common;
using SimRMS.Domain.Entities;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Domain.Interfaces.Repo;
using SimRMS.Shared.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

namespace SimRMS.Infrastructure.Services
{
    /// <summary>
    /// Business logic service implementation for UsrInfo operations
    /// Implementation belongs in Infrastructure layer per Clean Architecture
    /// </summary>
    public class UsrInfoService : IUsrInfoService
    {
        private readonly IUsrInfoRepository _usrInfoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UsrInfoRequest> _requestValidator;
        private readonly ILogger<UsrInfoService> _logger;

        public UsrInfoService(
            IUsrInfoRepository usrInfoRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<UsrInfoRequest> requestValidator,
            ILogger<UsrInfoService> logger)
        {
            _usrInfoRepository = usrInfoRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _requestValidator = requestValidator;
            _logger = logger;
        }

        public async Task<PagedResult<UsrInfoDto>> GetUsersAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? usrStatus = null,
            string? coCode = null,
            string? dlrCode = null,
            string? rmsType = null,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting paged users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            // Input validation
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            try
            {
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                var pagedResult = await _usrInfoRepository.GetUsersPagedAsync(
                    pageNumber, pageSize, usrStatus, coCode, dlrCode, rmsType, searchTerm, cancellationToken);

                var mappedData = pagedResult.Data.ToDtos<UsrInfoDto>(_mapper);

                return new PagedResult<UsrInfoDto>
                {
                    Data = mappedData,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged users");
                throw;
            }
        }

        public async Task<UsrInfoDto> GetUserByIdAsync(string usrId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(usrId));

            _logger.LogInformation("Getting user by ID: {UsrId}", usrId);

            try
            {
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                var user = await _usrInfoRepository.GetByUserIdAsync(usrId.Trim(), cancellationToken);
                if (user == null)
                    throw new NotFoundException(nameof(UsrInfo), usrId);

                return user.ToDto<UsrInfoDto>(_mapper);
            }
            catch (Exception ex) when (!(ex is NotFoundException || ex is ArgumentException))
            {
                _logger.LogError(ex, "Error getting user by ID: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<UsrInfoDto> CreateUserAsync(UsrInfoRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _logger.LogInformation("Creating new user: {UsrId}", request.UsrId);

            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    // ✅ FIXED: Use CREATE validation context
                    await ValidateRequestForCreateAsync(request, cancellationToken);

                    // Business validation
                    await ValidateUserBusinessRulesAsync(request.UsrId!, request.UsrEmail, false, cancellationToken);

                    // Map and create using extension method
                    var usrInfo = request.ToEntityForCreate(_mapper);
                    var createdUser = await _usrInfoRepository.CreateUserAsync(usrInfo, cancellationToken);

                    _logger.LogInformation("Successfully created user: {UsrId}", request.UsrId);
                    return createdUser.ToDto<UsrInfoDto>(_mapper);
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {UsrId}", request.UsrId);
                throw;
            }
        }

        public async Task<UsrInfoDto> UpdateUserAsync(string usrId, UsrInfoRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(usrId));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _logger.LogInformation("Updating user: {UsrId}", usrId);

            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    // Get existing user first
                    var existingUser = await _usrInfoRepository.GetByUserIdAsync(usrId, cancellationToken);
                    if (existingUser == null)
                        throw new NotFoundException(nameof(UsrInfo), usrId);

                    // Clear UsrId from request (should come from route, not body)
                    request.UsrId = null;

                    // ✅ FIXED: Use UPDATE validation context
                    await ValidateRequestForUpdateAsync(request, cancellationToken);

                    // Business validation
                    await ValidateUserBusinessRulesAsync(usrId, request.UsrEmail, true, cancellationToken);

                    // Update entity using extension method
                    existingUser.UpdateFromRequest(request, _mapper);

                    // Update in repository
                    var updatedUser = await _usrInfoRepository.UpdateUserAsync(existingUser, cancellationToken);

                    _logger.LogInformation("Successfully updated user: {UsrId}", usrId);
                    return updatedUser.ToDto<UsrInfoDto>(_mapper);
                }, cancellationToken);
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Error updating user: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string usrId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(usrId));

            _logger.LogInformation("Deleting user: {UsrId}", usrId);

            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    // Check if user exists
                    var existingUser = await _usrInfoRepository.GetByUserIdAsync(usrId, cancellationToken);
                    if (existingUser == null)
                        throw new NotFoundException(nameof(UsrInfo), usrId);

                    // Additional business rules for deletion can be added here
                    // e.g., check if user has active trades, etc.

                    var deleted = await _usrInfoRepository.DeleteUserAsync(usrId, cancellationToken);
                    if (!deleted)
                        throw new DomainException($"Failed to delete user with ID '{usrId}'");

                    _logger.LogInformation("Successfully deleted user: {UsrId}", usrId);
                    return true;
                }, cancellationToken);
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Error deleting user: {UsrId}", usrId);
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string usrId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                return false;

            try
            {
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);
                var user = await _usrInfoRepository.GetByUserIdAsync(usrId.Trim(), cancellationToken);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists: {UsrId}", usrId);
                return false;
            }
        }

        public async Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting user statistics");

            try
            {
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                var basicStats = await _usrInfoRepository.GetUserStatisticsAsync(cancellationToken);

                return new UserStatisticsDto
                {
                    TotalUsers = Convert.ToInt32(basicStats["TotalUsers"]),
                    ActiveUsers = Convert.ToInt32(basicStats["ActiveUsers"]),
                    SuspendedUsers = Convert.ToInt32(basicStats["SuspendedUsers"]),
                    ClosedUsers = Convert.ToInt32(basicStats["ClosedUsers"]),
                    RmsTypes = new List<RmsTypeStatistic>(), // Can be enhanced later
                    CompanyCodes = new List<CompanyCodeStatistic>() // Can be enhanced later
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                throw;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// ✅ FIXED: Validate request for CREATE operation using working context
        /// </summary>
        private async Task ValidateRequestForCreateAsync(UsrInfoRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _requestValidator.ValidateForCreateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException("Create validation failed")
                {
                    ValidationErrors = validationResult.Errors.Select(e => new ValidationErrorDetail
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage,
                        AttemptedValue = e.AttemptedValue?.ToString()
                    }).ToList()
                };
            }
        }

        /// <summary>
        /// ✅ FIXED: Validate request for UPDATE operation using working context
        /// </summary>
        private async Task ValidateRequestForUpdateAsync(UsrInfoRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _requestValidator.ValidateForUpdateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException("Update validation failed")
                {
                    ValidationErrors = validationResult.Errors.Select(e => new ValidationErrorDetail
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage,
                        AttemptedValue = e.AttemptedValue?.ToString()
                    }).ToList()
                };
            }
        }

        private async Task ValidateUserBusinessRulesAsync(string? usrId, string? email, bool isUpdate = false, CancellationToken cancellationToken = default)
        {
            // Check for duplicate user ID (only for create)
            if (!isUpdate && !string.IsNullOrEmpty(usrId))
            {
                var userExists = await UserExistsAsync(usrId, cancellationToken);
                if (userExists)
                    throw new DomainException($"User with ID '{usrId}' already exists");
            }

            // Check for duplicate email
            if (!string.IsNullOrEmpty(email))
            {
                var emailExists = await _usrInfoRepository.ExistsByEmailAsync(email, isUpdate ? usrId : null, cancellationToken);
                if (emailExists)
                    throw new DomainException($"User with email '{email}' already exists");
            }

            // Add more business rules as needed
        }

        #endregion
    }
}