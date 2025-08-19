using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Common;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using SimRMS.Shared.Constants;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company Branch Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This service provides methods for managing Market Stock Company Branch information
/// Creation:    13/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

/// <summary>
/// Broker Branch service with business logic and validation
/// </summary>
public class BrokerBranchService : IBrokerBranchService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateMstCoBrchRequest> _createValidator;
    private readonly IValidator<UpdateMstCoBrchRequest> _updateValidator;
    private readonly IValidator<DeleteMstCoBrchRequest> _deleteValidator;
    private readonly IValidator<AuthorizeMstCoBrchRequest> _authorizeValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<BrokerBranchService> _logger;

    public BrokerBranchService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateMstCoBrchRequest> createValidator,
        IValidator<UpdateMstCoBrchRequest> updateValidator,
        IValidator<DeleteMstCoBrchRequest> deleteValidator,
        IValidator<AuthorizeMstCoBrchRequest> authorizeValidator,
        ICurrentUserService currentUserService,
        ILogger<BrokerBranchService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PagedResult<MstCoBrchDto>> GetMstCoBrchListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? coCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged MstCoBrch list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            // Call stored procedure directly with OUTPUT parameter
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = "CoCode",
                SortDirection = "ASC",
                CoCode = coCode,
                SearchTerm = searchTerm,
                IsAuth = 1, // Approved: 1 , Denied: 2
                TotalCount = 0 // OUTPUT parameter
            };

            // Get the data from SP using QueryPagedAsync which should handle the OUTPUT parameter
            var result = await _repository.QueryPagedAsync<MstCoBrchDto>(
                sqlOrSp: "LB_SP_GetBrokerBranchList",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for MstCoBrch list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting MstCoBrch list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting MstCoBrch list");
            throw new DomainException($"Failed to retrieve broker branch list: {ex.Message}");
        }
    }

    public async Task<MstCoBrchDto?> GetMstCoBrchByIdAsync(string coCode, string coBrchCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(coCode))
            throw new ArgumentException("Company code cannot be null or empty", nameof(coCode));

        if (string.IsNullOrWhiteSpace(coBrchCode))
            throw new ArgumentException("Branch code cannot be null or empty", nameof(coBrchCode));

        _logger.LogInformation("Getting MstCoBrch by ID: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        var parameters = new 
        { 
            CoCode = coCode, 
            CoBrchCode = coBrchCode,
            StatusCode = 0, // OUTPUT parameter
            StatusMsg = "" // OUTPUT parameter
        };
        
        try
        {
            return await _repository.QuerySingleAsync<MstCoBrchDto>(
                sqlOrSp: "LB_SP_GetMstCoBrch_ByBranchCode", 
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for MstCoBrch retrieval: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting MstCoBrch by ID: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting MstCoBrch by ID: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to retrieve broker branch: {ex.Message}");
        }
    }

    public async Task<MstCoBrchDto> CreateMstCoBrchAsync(CreateMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Creating MstCoBrch: {CoCode}-{CoBrchCode}", request.CoCode, request.CoBrchCode);

        await ValidateCreateRequestAsync(request, cancellationToken);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await MstCoBrchExistsAsync(request.CoCode, request.CoBrchCode, cancellationToken))
                throw new DomainException($"Branch with code '{request.CoCode}-{request.CoBrchCode}' already exists");

            await ValidateBusinessRulesAsync(request, cancellationToken);

            var parameters = new
            {
                Action = 1,
                CoCode = request.CoCode,
                CoBrchCode = request.CoBrchCode,
                CoBrchDesc = request.CoBrchDesc,
                CoBrchAddr = request.CoBrchAddr,
                CoBrchPhone = request.CoBrchPhone,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.GetCurrentUserId(),
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.INSERT,
                IsDel = (byte)0,
                Remarks = request.Remarks,
                RowsAffected = 0 // OUTPUT parameter
            };

            var rowsAffected = await _repository.ExecuteAsync(
                sqlOrSp: "LB_SP_CrudMstCoBrch",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            if (rowsAffected == 0)
                throw new DomainException($"Failed to create MstCoBrch with code '{request.CoCode}-{request.CoBrchCode}'");

            var createdBranch = await GetMstCoBrchByIdAsync(request.CoCode, request.CoBrchCode, cancellationToken);
            return createdBranch ?? throw new DomainException("Failed to retrieve created MstCoBrch");
        }, cancellationToken);
    }

    public async Task<MstCoBrchDto> UpdateMstCoBrchAsync(string coCode, string coBrchCode, UpdateMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(coCode))
            throw new ArgumentException("Company code cannot be null or empty", nameof(coCode));

        if (string.IsNullOrWhiteSpace(coBrchCode))
            throw new ArgumentException("Branch code cannot be null or empty", nameof(coBrchCode));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Updating MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        await ValidateUpdateRequestAsync(request, cancellationToken);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existingBranch = await GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);
            if (existingBranch == null)
                throw new NotFoundException("MstCoBrch", $"{coCode}-{coBrchCode}");

            await ValidateBusinessRulesForUpdateAsync(coCode, coBrchCode, request, cancellationToken);

            var parameters = new
            {
                Action = 2,
                CoCode = coCode,
                CoBrchCode = coBrchCode,
                CoBrchDesc = request.CoBrchDesc,
                CoBrchAddr = request.CoBrchAddr,
                CoBrchPhone = request.CoBrchPhone,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.GetCurrentUserId(),
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.UPDATE,
                IsDel = (byte)0,
                Remarks = request.Remarks,
                RowsAffected = 0 // OUTPUT parameter

            };

            var rowsAffected = await _repository.ExecuteAsync(
                sqlOrSp: "LB_SP_CrudMstCoBrch",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            if (rowsAffected == 0)
                throw new DomainException($"Failed to update MstCoBrch with code '{coCode}-{coBrchCode}'");

            var updatedBranch = await GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);
            return updatedBranch ?? throw new DomainException("Failed to retrieve updated MstCoBrch");
        }, cancellationToken);
    }

    public async Task<bool> DeleteMstCoBrchAsync(string coCode, string coBrchCode, DeleteMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(coCode))
            throw new ArgumentException("Company code cannot be null or empty", nameof(coCode));

        if (string.IsNullOrWhiteSpace(coBrchCode))
            throw new ArgumentException("Branch code cannot be null or empty", nameof(coBrchCode));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Deleting MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        await ValidateDeleteRequestAsync(request, cancellationToken);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existingBranch = await GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);
            if (existingBranch == null)
                throw new NotFoundException("MstCoBrch", $"{coCode}-{coBrchCode}");

            var parameters = new
            {
                Action = 3,
                CoCode = coCode,
                CoBrchCode = coBrchCode,
                CoBrchDesc = (string?)null,
                CoBrchAddr = (string?)null,
                CoBrchPhone = (string?)null,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.GetCurrentUserId(),
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.DELETE,
                IsDel = (byte)1,
                Remarks = request.Remarks,
                RowsAffected = 0 // OUTPUT parameter

            };

            var rowsAffected = await _repository.ExecuteAsync(
                sqlOrSp: "LB_SP_CrudMstCoBrch",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return rowsAffected > 0;
        }, cancellationToken);
    }

    public async Task<bool> MstCoBrchExistsAsync(string coCode, string coBrchCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(coCode) || string.IsNullOrWhiteSpace(coBrchCode))
            return false;

        try
        {
            var sql = "SELECT COUNT(1) FROM MstCoBrch WHERE CoCode = @CoCode AND CoBrchCode = @CoBrchCode AND IsDel = 0";
            var count = await _repository.ExecuteScalarAsync<int>(sql, new { CoCode = coCode, CoBrchCode = coBrchCode }, cancellationToken: cancellationToken);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking MstCoBrch existence: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to check broker branch existence: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private async Task ValidateCreateRequestAsync(CreateMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

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

    private async Task ValidateUpdateRequestAsync(UpdateMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

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

    private async Task ValidateDeleteRequestAsync(DeleteMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _deleteValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            
            throw new ValidationException("Delete validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateBusinessRulesAsync(CreateMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var sql = @"SELECT COUNT(1) FROM MstCo WHERE CoCode = @CoCode AND IsDel = 0";
            var companyExists = await _repository.ExecuteScalarAsync<int>(
                sql, 
                new { CoCode = request.CoCode }, 
                cancellationToken: cancellationToken);
            
            if (companyExists == 0)
                throw new DomainException($"Company with code '{request.CoCode}' does not exist");

            if (!string.IsNullOrEmpty(request.CoBrchDesc))
            {
                var sql2 = @"SELECT COUNT(1) FROM MstCoBrch 
                            WHERE CoBrchDesc = @CoBrchDesc 
                            AND CoCode = @CoCode 
                            AND IsDel = 0";
                var count = await _repository.ExecuteScalarAsync<int>(
                    sql2, 
                    new { CoBrchDesc = request.CoBrchDesc, CoCode = request.CoCode }, 
                    cancellationToken: cancellationToken);
                
                if (count > 0)
                    throw new DomainException($"Branch with description '{request.CoBrchDesc}' already exists for company '{request.CoCode}'");
            }
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating business rules for branch creation: {CoCode}", request.CoCode);
            throw new DomainException($"Failed to validate business rules: {ex.Message}");
        }
    }

    private async Task ValidateBusinessRulesForUpdateAsync(string coCode, string coBrchCode, UpdateMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(request.CoBrchDesc))
            {
                var sql = @"SELECT COUNT(1) FROM MstCoBrch 
                           WHERE CoBrchDesc = @CoBrchDesc 
                           AND CoCode = @CoCode 
                           AND CoBrchCode != @CoBrchCode 
                           AND IsDel = 0";
                var count = await _repository.ExecuteScalarAsync<int>(
                    sql, 
                    new { CoBrchDesc = request.CoBrchDesc, CoCode = coCode, CoBrchCode = coBrchCode }, 
                    cancellationToken: cancellationToken);
                
                if (count > 0)
                    throw new DomainException($"Branch with description '{request.CoBrchDesc}' already exists for company '{coCode}'");
            }
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating business rules for branch update: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to validate business rules: {ex.Message}");
        }
    }

    #endregion

    #region WF / Work Flow

    public async Task<PagedResult<MstCoBrchDto>> GetUnauthorizedListAsync(
    int pageNumber = 1,
    int pageSize = 10,
    string? searchTerm = null,
    string? coCode = null,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting unauthorized MstCoBrch list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            // FIXED: Proper parameter setup for OUTPUT parameter handling
            var parameters = new Dictionary<string, object>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = pageSize,
                ["SortColumn"] = "CoCode",
                ["SortDirection"] = "ASC",
                ["CoCode"] = coCode ?? (object)DBNull.Value,
                ["SearchTerm"] = searchTerm ?? (object)DBNull.Value,
                ["isAuth"] = (byte)0, // Unauthorized records
                ["MakerId"] = _currentUserService.UserId,
                ["TotalCount"] = 0 // OUTPUT parameter - will be populated by SP
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetBrokerBranchListWF", pageNumber, pageSize, 0, _currentUserService.UserId);

            // FIXED: The GenericRepository now properly handles OUTPUT parameters in a single call
            var result = await _repository.QueryPagedAsync<MstCoBrchDto>(
                sqlOrSp: "LB_SP_GetBrokerBranchListWF",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            if (result.TotalCount == 0)
            {
                _logger.LogWarning("SP {StoredProcedure} returned TotalCount=0. Check OUTPUT parameter handling or SP logic.",
                    "LB_SP_GetBrokerBranchListWF");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unauthorized broker branch list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, CoCode={CoCode}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, coCode, searchTerm);
            throw;
        }
    }






    public async Task<bool> AuthorizeBranchAsync(string coCode, string coBrchCode, AuthorizeMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing MstCoBrch in workflow: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        // Validate request
        var validationResult = await _authorizeValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed for authorization request: {Errors}", errors);
            throw new ValidationException($"Validation failed: {errors}");
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var ipAddress = _currentUserService.GetClientIPAddress();
            
            var parameters = new
            {
                Action = request.ActionType, // Default is 2 for authorization
                CoCode = coCode,
                CoBrchCode = coBrchCode,
                IPAddress = ipAddress,
                AuthID = _currentUserService.UserId,
                IsAuth = request.IsAuth,
                ActionType = request.ActionType,
                RowsAffected = 0 // OUTPUT parameter
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthMstCoBrch",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized MstCoBrch: {CoCode}-{CoBrchCode}, RowsAffected: {RowsAffected}", 
                    coCode, coBrchCode, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("No rows affected during authorization of MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
                throw new DomainException($"Failed to authorize branch: No records were updated");
            }
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing MstCoBrch in workflow: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to authorize branch: {ex.Message}");
        }
    }

    #endregion
}