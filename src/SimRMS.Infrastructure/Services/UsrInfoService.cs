using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Common;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

namespace SimRMS.Infrastructure.Services;

/// <summary>
    /// Optimized UsrInfo service with single DB call pagination
    /// </summary>
    public class UsrInfoService : IUsrInfoService
    {
        private readonly IGenericRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UsrInfoRequest> _validator;
        private readonly ILogger<UsrInfoService> _logger;

        public UsrInfoService(
            IGenericRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<UsrInfoRequest> validator,
            ILogger<UsrInfoService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
        }

        public async Task<PagedResult<UsrInfoDto>> GetUsersAsync(int pageNumber = 1, int pageSize = 10, string? usrStatus = null, string? coCode = null, string? dlrCode = null, string? rmsType = null, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting paged users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            // Input validation
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Build base query WITHOUT ORDER BY (will be added in repository)
            var baseQuery = @"
                SELECT UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, UsrPassNo, 
                       UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                       UsrLastUpdatedDate, UsrCreationDate, UsrStatus, UsrQualify, UsrRegisterDate,
                       UsrTDRDate, UsrResignDate, ClntCode, UsrLicenseNo, UsrExpiryDate, RmsType,
                       UsrSuperiorID, UsrNotifierID 
                FROM UsrInfo 
                WHERE (@UsrStatus IS NULL OR UsrStatus = @UsrStatus)
                  AND (@CoCode IS NULL OR CoCode = @CoCode)
                  AND (@DlrCode IS NULL OR DlrCode = @DlrCode)
                  AND (@RmsType IS NULL OR RmsType = @RmsType)
                  AND (@SearchTerm IS NULL OR UsrID LIKE @SearchTerm OR UsrName LIKE @SearchTerm OR UsrEmail LIKE @SearchTerm)";

            var parameters = new
            {
                UsrStatus = usrStatus,
                CoCode = coCode,
                DlrCode = dlrCode,
                RmsType = rmsType,
                SearchTerm = string.IsNullOrEmpty(searchTerm) ? null : $"%{searchTerm}%"
            };

            // Single DB call with window function
            var result = await _repository.QueryPagedAsync<UsrInfoDto>(
                sqlOrSp: baseQuery, 
                pageNumber: pageNumber, 
                pageSize: pageSize, 
                parameters: parameters, 
                orderBy: "UsrID", // Order by column
                isStoredProcedure: false,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<UsrInfoDto?> GetUserByIdAsync(string usrId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(usrId));

            _logger.LogInformation("Getting user by ID: {UsrId}", usrId);

            var sql = @"SELECT UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, UsrPassNo, 
                       UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                       UsrLastUpdatedDate, UsrCreationDate, UsrStatus, UsrQualify, UsrRegisterDate,
                       UsrTDRDate, UsrResignDate, ClntCode, UsrLicenseNo, UsrExpiryDate, RmsType,
                       UsrSuperiorID, UsrNotifierID 
                       FROM UsrInfo WHERE UsrID = @UsrId";

            return await _repository.QuerySingleAsync<UsrInfoDto>(sql, new { UsrId = usrId }, cancellationToken: cancellationToken);
        }

        public async Task<UsrInfoDto> CreateUserAsync(UsrInfoRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _logger.LogInformation("Creating new user: {UsrId}", request.UsrId);

            // Validation
            await ValidateForCreateAsync(request, cancellationToken);

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Business validation
                await ValidateBusinessRulesAsync(request.UsrId!, request.UsrEmail, false, cancellationToken);

                // Insert user
                var sql = @"INSERT INTO UsrInfo (UsrID, DlrCode, CoCode, CoBrchCode, UsrName, UsrType, UsrNICNo, 
                           UsrPassNo, UsrGender, UsrDOB, UsrRace, UsrEmail, UsrAddr, UsrPhone, UsrMobile, UsrFax,
                           UsrStatus, UsrQualify, UsrRegisterDate, UsrTDRDate, UsrResignDate, ClntCode,
                           UsrLicenseNo, UsrExpiryDate, RmsType, UsrSuperiorID, UsrNotifierID, 
                           UsrCreationDate, UsrLastUpdatedDate)
                           VALUES (@UsrId, @DlrCode, @CoCode, @CoBrchCode, @UsrName, @UsrType, @UsrNicno,
                           @UsrPassNo, @UsrGender, @UsrDob, @UsrRace, @UsrEmail, @UsrAddr, @UsrPhone, @UsrMobile, @UsrFax,
                           @UsrStatus, @UsrQualify, @UsrRegisterDate, @UsrTdrdate, @UsrResignDate, @ClntCode,
                           @UsrLicenseNo, @UsrExpiryDate, @RmsType, @UsrSuperiorId, @UsrNotifierId,
                           @UsrCreationDate, @UsrLastUpdatedDate)";

                var parameters = CreateParametersFromRequest(request, true);
                var rowsAffected = await _repository.ExecuteAsync(sql, parameters, cancellationToken: cancellationToken);

                if (rowsAffected == 0)
                    throw new DomainException("Failed to create user");

                // Return created user
                var createdUser = await GetUserByIdAsync(request.UsrId!, cancellationToken);
                return createdUser ?? throw new DomainException("Failed to retrieve created user");
            }, cancellationToken);
        }

        public async Task<UsrInfoDto> UpdateUserAsync(string usrId, UsrInfoRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(usrId));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _logger.LogInformation("Updating user: {UsrId}", usrId);

            // Validation
            await ValidateForUpdateAsync(request, cancellationToken);

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Check if user exists
                var existingUser = await GetUserByIdAsync(usrId, cancellationToken);
                if (existingUser == null)
                    throw new NotFoundException("UsrInfo", usrId);

                // Business validation
                await ValidateBusinessRulesAsync(usrId, request.UsrEmail, true, cancellationToken);

                // Update user - only update non-null values
                var updateParts = new List<string>();
                var parameters = new Dictionary<string, object> { { "UsrId", usrId }, { "UsrLastUpdatedDate", DateTime.UtcNow } };

                if (!string.IsNullOrEmpty(request.DlrCode)) { updateParts.Add("DlrCode = @DlrCode"); parameters["DlrCode"] = request.DlrCode; }
                if (!string.IsNullOrEmpty(request.CoCode)) { updateParts.Add("CoCode = @CoCode"); parameters["CoCode"] = request.CoCode; }
                if (!string.IsNullOrEmpty(request.CoBrchCode)) { updateParts.Add("CoBrchCode = @CoBrchCode"); parameters["CoBrchCode"] = request.CoBrchCode; }
                if (!string.IsNullOrEmpty(request.UsrName)) { updateParts.Add("UsrName = @UsrName"); parameters["UsrName"] = request.UsrName; }
                if (request.UsrType.HasValue) { updateParts.Add("UsrType = @UsrType"); parameters["UsrType"] = request.UsrType; }
                if (!string.IsNullOrEmpty(request.UsrNicno)) { updateParts.Add("UsrNICNo = @UsrNicno"); parameters["UsrNicno"] = request.UsrNicno; }
                if (!string.IsNullOrEmpty(request.UsrPassNo)) { updateParts.Add("UsrPassNo = @UsrPassNo"); parameters["UsrPassNo"] = request.UsrPassNo; }
                if (!string.IsNullOrEmpty(request.UsrGender)) { updateParts.Add("UsrGender = @UsrGender"); parameters["UsrGender"] = request.UsrGender; }
                if (request.UsrDob.HasValue) { updateParts.Add("UsrDOB = @UsrDob"); parameters["UsrDob"] = request.UsrDob; }
                if (!string.IsNullOrEmpty(request.UsrRace)) { updateParts.Add("UsrRace = @UsrRace"); parameters["UsrRace"] = request.UsrRace; }
                if (!string.IsNullOrEmpty(request.UsrEmail)) { updateParts.Add("UsrEmail = @UsrEmail"); parameters["UsrEmail"] = request.UsrEmail; }
                if (!string.IsNullOrEmpty(request.UsrAddr)) { updateParts.Add("UsrAddr = @UsrAddr"); parameters["UsrAddr"] = request.UsrAddr; }
                if (!string.IsNullOrEmpty(request.UsrPhone)) { updateParts.Add("UsrPhone = @UsrPhone"); parameters["UsrPhone"] = request.UsrPhone; }
                if (!string.IsNullOrEmpty(request.UsrMobile)) { updateParts.Add("UsrMobile = @UsrMobile"); parameters["UsrMobile"] = request.UsrMobile; }
                if (!string.IsNullOrEmpty(request.UsrFax)) { updateParts.Add("UsrFax = @UsrFax"); parameters["UsrFax"] = request.UsrFax; }
                if (!string.IsNullOrEmpty(request.UsrStatus)) { updateParts.Add("UsrStatus = @UsrStatus"); parameters["UsrStatus"] = request.UsrStatus; }
                if (!string.IsNullOrEmpty(request.UsrQualify)) { updateParts.Add("UsrQualify = @UsrQualify"); parameters["UsrQualify"] = request.UsrQualify; }
                if (request.UsrRegisterDate.HasValue) { updateParts.Add("UsrRegisterDate = @UsrRegisterDate"); parameters["UsrRegisterDate"] = request.UsrRegisterDate; }
                if (request.UsrTdrdate.HasValue) { updateParts.Add("UsrTDRDate = @UsrTdrdate"); parameters["UsrTdrdate"] = request.UsrTdrdate; }
                if (request.UsrResignDate.HasValue) { updateParts.Add("UsrResignDate = @UsrResignDate"); parameters["UsrResignDate"] = request.UsrResignDate; }
                if (!string.IsNullOrEmpty(request.ClntCode)) { updateParts.Add("ClntCode = @ClntCode"); parameters["ClntCode"] = request.ClntCode; }
                if (!string.IsNullOrEmpty(request.UsrLicenseNo)) { updateParts.Add("UsrLicenseNo = @UsrLicenseNo"); parameters["UsrLicenseNo"] = request.UsrLicenseNo; }
                if (request.UsrExpiryDate.HasValue) { updateParts.Add("UsrExpiryDate = @UsrExpiryDate"); parameters["UsrExpiryDate"] = request.UsrExpiryDate; }
                if (!string.IsNullOrEmpty(request.RmsType)) { updateParts.Add("RmsType = @RmsType"); parameters["RmsType"] = request.RmsType; }
                if (request.UsrSuperiorId.HasValue) { updateParts.Add("UsrSuperiorID = @UsrSuperiorId"); parameters["UsrSuperiorId"] = request.UsrSuperiorId; }
                if (request.UsrNotifierId.HasValue) { updateParts.Add("UsrNotifierID = @UsrNotifierId"); parameters["UsrNotifierId"] = request.UsrNotifierId; }

                // Always update timestamp
                updateParts.Add("UsrLastUpdatedDate = @UsrLastUpdatedDate");

                if (updateParts.Count == 1) // Only timestamp
                {
                    throw new DomainException("No fields to update");
                }

                var sql = $"UPDATE UsrInfo SET {string.Join(", ", updateParts)} WHERE UsrID = @UsrId";
                var rowsAffected = await _repository.ExecuteAsync(sql, parameters, cancellationToken: cancellationToken);

                if (rowsAffected == 0)
                    throw new DomainException($"Failed to update user with ID '{usrId}'");

                // Return updated user
                var updatedUser = await GetUserByIdAsync(usrId, cancellationToken);
                return updatedUser ?? throw new DomainException("Failed to retrieve updated user");
            }, cancellationToken);
        }

        public async Task<bool> DeleteUserAsync(string usrId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(usrId));

            _logger.LogInformation("Deleting user: {UsrId}", usrId);

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Check if user exists
                var existingUser = await GetUserByIdAsync(usrId, cancellationToken);
                if (existingUser == null)
                    throw new NotFoundException("UsrInfo", usrId);

                var sql = "DELETE FROM UsrInfo WHERE UsrID = @UsrId";
                var rowsAffected = await _repository.ExecuteAsync(sql, new { UsrId = usrId }, cancellationToken: cancellationToken);

                return rowsAffected > 0;
            }, cancellationToken);
        }

        public async Task<bool> UserExistsAsync(string usrId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(usrId))
                return false;

            var sql = "SELECT COUNT(1) FROM UsrInfo WHERE UsrID = @UsrId";
            var count = await _repository.ExecuteScalarAsync<int>(sql, new { UsrId = usrId }, cancellationToken: cancellationToken);
            return count > 0;
        }

        public async Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting user statistics");

            var sql = @"SELECT COUNT(*) as TotalUsers,
                       SUM(CASE WHEN UsrStatus = 'A' THEN 1 ELSE 0 END) as ActiveUsers,
                       SUM(CASE WHEN UsrStatus = 'S' THEN 1 ELSE 0 END) as SuspendedUsers,
                       SUM(CASE WHEN UsrStatus = 'C' THEN 1 ELSE 0 END) as ClosedUsers
                       FROM UsrInfo";

            var stats = await _repository.QuerySingleAsync<UserStatisticsDto>(sql, cancellationToken: cancellationToken);
            return stats ?? new UserStatisticsDto();
        }

        #region Private Helper Methods

        private async Task ValidateForCreateAsync(UsrInfoRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, options => 
                options.IncludeRuleSets("Create"), cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage,
                    AttemptedValue = e.AttemptedValue?.ToString()
                }).ToList();
                
                throw new ValidationException("Create validation failed") { ValidationErrors = errors };
            }
        }

        private async Task ValidateForUpdateAsync(UsrInfoRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, options => 
                options.IncludeRuleSets("Update"), cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage,
                    AttemptedValue = e.AttemptedValue?.ToString()
                }).ToList();
                
                throw new ValidationException("Update validation failed") { ValidationErrors = errors };
            }
        }

        private async Task ValidateBusinessRulesAsync(string usrId, string? email, bool isUpdate, CancellationToken cancellationToken)
        {
            // Check for duplicate user ID (only for create)
            if (!isUpdate)
            {
                var userExists = await UserExistsAsync(usrId, cancellationToken);
                if (userExists)
                    throw new DomainException($"User with ID '{usrId}' already exists");
            }

            // Check for duplicate email
            if (!string.IsNullOrEmpty(email))
            {
                var sql = @"SELECT COUNT(1) FROM UsrInfo WHERE UsrEmail = @Email 
                           AND (@ExcludeUsrId IS NULL OR UsrID != @ExcludeUsrId)";
                var count = await _repository.ExecuteScalarAsync<int>(sql, new { Email = email, ExcludeUsrId = isUpdate ? usrId : null }, cancellationToken: cancellationToken);
                
                if (count > 0)
                    throw new DomainException($"User with email '{email}' already exists");
            }
        }

        private dynamic CreateParametersFromRequest(UsrInfoRequest request, bool isCreate)
        {
            var now = DateTime.UtcNow;
            
            return new
            {
                UsrId = request.UsrId,
                DlrCode = request.DlrCode,
                CoCode = request.CoCode,
                CoBrchCode = request.CoBrchCode,
                UsrName = request.UsrName,
                UsrType = request.UsrType,
                UsrNicno = request.UsrNicno,
                UsrPassNo = request.UsrPassNo,
                UsrGender = request.UsrGender,
                UsrDob = request.UsrDob,
                UsrRace = request.UsrRace,
                UsrEmail = request.UsrEmail,
                UsrAddr = request.UsrAddr,
                UsrPhone = request.UsrPhone,
                UsrMobile = request.UsrMobile,
                UsrFax = request.UsrFax,
                UsrStatus = request.UsrStatus ?? "A",
                UsrQualify = request.UsrQualify,
                UsrRegisterDate = request.UsrRegisterDate,
                UsrTdrdate = request.UsrTdrdate,
                UsrResignDate = request.UsrResignDate,
                ClntCode = request.ClntCode,
                UsrLicenseNo = request.UsrLicenseNo,
                UsrExpiryDate = request.UsrExpiryDate,
                RmsType = request.RmsType,
                UsrSuperiorId = request.UsrSuperiorId,
                UsrNotifierId = request.UsrNotifierId,
                UsrCreationDate = isCreate ? now : (DateTime?)null,
                UsrLastUpdatedDate = now
            };
        }

        #endregion
    }
