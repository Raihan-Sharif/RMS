using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using SimRMS.Domain.Exceptions;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.Domain.Common;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Exposure Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing User Exposure information
/// Creation:    04/Sep/2025
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

public class UserExposureService : IUserExposureService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateUserExposureRequest> _createValidator;
    private readonly IValidator<UpdateUserExposureRequest> _updateValidator;
    private readonly IValidator<DeleteUserExposureRequest> _deleteValidator;
    private readonly IValidator<AuthorizeUserExposureRequest> _authorizeValidator;
    private readonly IValidator<GetUserExposureWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UserExposureService> _logger;

    public UserExposureService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateUserExposureRequest> createValidator,
        IValidator<UpdateUserExposureRequest> updateValidator,
        IValidator<DeleteUserExposureRequest> deleteValidator,
        IValidator<AuthorizeUserExposureRequest> authorizeValidator,
        IValidator<GetUserExposureWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<UserExposureService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
        _workflowListValidator = workflowListValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Main CRUD Operations

    public async Task<PagedResult<UserExposureDto>> GetUserExposureListAsync(int pageNumber = 1, int pageSize = 10,
        string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged UserExposure list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchText = searchTerm,
                SortColumn = "UsrID",
                SortDirection = "ASC",
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetUserExpsList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);

            var result = await _repository.QueryPagedAsync<UserExposureDto>(
                sqlOrSp: "LB_SP_GetUserExpsList",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for UserExposure list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting UserExposure list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting UserExposure list");
            throw new DomainException($"Failed to retrieve user exposure list: {ex.Message}");
        }
    }

    public async Task<UserExposureDto?> GetUserExposureByIdAsync(string usrId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting UserExposure by ID: {UsrId}", usrId);

        try
        {
            var parameters = new
            {
                UsrID = usrId,
                StatusCode = 0, // output param
                StatusMsg = "" // output param
            };

            var result = await _repository.QuerySingleAsync<UserExposureDto>(
                sqlOrSp: "LB_SP_GetMstUserExpsByUsrID",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved user exposure: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for UserExposure retrieval: {UsrId}", usrId);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting UserExposure by ID: {UsrId}", usrId);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting UserExposure by ID: {UsrId}", usrId);
            throw new DomainException($"Failed to retrieve user exposure: {ex.Message}");
        }
    }

    public async Task<UserExposureDto> CreateUserExposureAsync(CreateUserExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new UserExposure for user: {UsrId}", request.UsrId);

        // Validate the request
        await ValidateCreateRequestAsync(request, cancellationToken);

        // Apply business logic: buy/sell vs total exposure
        ApplyBusinessLogicRules(request);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.INSERT,
                ["UsrID"] = request.UsrId,
                ["UsrExpsCheckBuy"] = request.UsrExpsCheckBuy,
                ["UsrExpsBuyAmt"] = request.UsrExpsBuyAmt,
                ["UsrExpsCheckSell"] = request.UsrExpsCheckSell,
                ["UsrExpsSellAmt"] = request.UsrExpsSellAmt,
                ["UsrExpsCheckTotal"] = request.UsrExpsCheckTotal,
                ["UsrExpsTotalAmt"] = request.UsrExpsTotalAmt,
                ["UsrExpsWithShrLimit"] = request.UsrExpsWithShrLimit,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.INSERT,
                ["AuthId"] = (object)DBNull.Value, // NULL for workflow
                ["AuthDt"] = (object)DBNull.Value, // NULL for workflow
                ["AuthTransDt"] = (object)DBNull.Value, // NULL for workflow
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize, // Always start unauthorized
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudMstUsrExpsInfo with Action=1 (INSERT) for user: {UsrId}", request.UsrId);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstUsrExpsInfo",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Create operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("Failed to create user exposure - no rows affected");
            }

            // Return manually constructed DTO since unauthorized records aren't returned by GetById SP
            UserExposureDto newUserExposure = new UserExposureDto
            {
                UsrId = request.UsrId,
                UsrExpsCheckBuy = request.UsrExpsCheckBuy,
                UsrExpsBuyAmt = request.UsrExpsBuyAmt,
                UsrExpsCheckSell = request.UsrExpsCheckSell,
                UsrExpsSellAmt = request.UsrExpsSellAmt,
                UsrExpsCheckTotal = request.UsrExpsCheckTotal,
                UsrExpsTotalAmt = request.UsrExpsTotalAmt,
                UsrExpsWithShrLimit = request.UsrExpsWithShrLimit,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date,
                ActionType = (byte)ActionTypeEnum.INSERT,
                IsAuth = (byte)AuthTypeEnum.UnAuthorize, // Initially unauthorized based on workflow
                AuthLevel = (byte)AuthLevelEnum.Level1,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks,
                
            };

            return newUserExposure;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating UserExposure for user: {UsrId}", request.UsrId);
            throw new DomainException($"Failed to create user exposure: {ex.Message}");
        }
    }

    public async Task<UserExposureDto> UpdateUserExposureAsync(string usrId, UpdateUserExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating UserExposure: {UsrId}", usrId);

        // Use existing validation method
        await ValidateUpdateRequestAsync(request, cancellationToken);

        // Apply business logic: buy/sell vs total exposure
        ApplyBusinessLogicRules(request);

        try
        {
            var existingExposure = await GetUserExposureByIdAsync(usrId, cancellationToken);
            if (existingExposure == null)
            {
                throw new DomainException($"User exposure not found: {usrId}");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.UPDATE,
                ["UsrID"] = usrId,
                ["UsrExpsCheckBuy"] = request.UsrExpsCheckBuy ?? (object)DBNull.Value,
                ["UsrExpsBuyAmt"] = request.UsrExpsBuyAmt ?? (object)DBNull.Value,
                ["UsrExpsCheckSell"] = request.UsrExpsCheckSell ?? (object)DBNull.Value,
                ["UsrExpsSellAmt"] = request.UsrExpsSellAmt ?? (object)DBNull.Value,
                ["UsrExpsCheckTotal"] = request.UsrExpsCheckTotal ?? (object)DBNull.Value,
                ["UsrExpsTotalAmt"] = request.UsrExpsTotalAmt ?? (object)DBNull.Value,
                ["UsrExpsWithShrLimit"] = request.UsrExpsWithShrLimit ?? (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.UPDATE,
                ["AuthId"] = (object)DBNull.Value, // Reset for workflow
                ["AuthDt"] = (object)DBNull.Value, // Reset for workflow
                ["AuthTransDt"] = (object)DBNull.Value, // Reset for workflow
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize, // Reset to unauthorized for workflow
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudMstUsrExpsInfo with Action=2 (UPDATE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstUsrExpsInfo",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Update operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("Failed to update user exposure - no rows affected");
            }

            var updatedExposure = await GetUserExposureByIdAsync(usrId, cancellationToken);

            if (updatedExposure == null)
            {
                throw new DomainException($"Updated user exposure not found: {usrId}");
            }

            _logger.LogInformation("Successfully updated UserExposure: {UsrId}", usrId);
            return updatedExposure;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating UserExposure: {UsrId}", usrId);
            throw new DomainException($"Failed to update user exposure: {ex.Message}");
        }
    }

    public async Task<bool> DeleteUserExposureAsync(string usrId, DeleteUserExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting UserExposure: {UsrId}", usrId);

        // Use existing validation method
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var existingExposure = await GetUserExposureByIdAsync(usrId, cancellationToken);
            if (existingExposure == null)
            {
                throw new DomainException($"User exposure not found: {usrId}");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.DELETE,
                ["UsrID"] = usrId,
                ["UsrExpsCheckBuy"] = (object)DBNull.Value,
                ["UsrExpsBuyAmt"] = (object)DBNull.Value,
                ["UsrExpsCheckSell"] = (object)DBNull.Value,
                ["UsrExpsSellAmt"] = (object)DBNull.Value,
                ["UsrExpsCheckTotal"] = (object)DBNull.Value,
                ["UsrExpsTotalAmt"] = (object)DBNull.Value,
                ["UsrExpsWithShrLimit"] = (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.DELETE,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (object)DBNull.Value, // SP handles deletion logic
                ["AuthLevel"] = (object)DBNull.Value,
                ["IsDel"] = (object)DBNull.Value, // SP sets this to 1
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudMstUsrExpsInfo with Action=3 (DELETE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstUsrExpsInfo",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Delete operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            var success = rowsAffected > 0;

            if (success)
            {
                _logger.LogInformation("Successfully deleted UserExposure: {UsrId}", usrId);
            }
            else
            {
                _logger.LogWarning("Delete operation failed - no rows affected: {UsrId}", usrId);
            }

            return success;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting UserExposure: {UsrId}", usrId);
            throw new DomainException($"Failed to delete user exposure: {ex.Message}");
        }
    }

    public async Task<bool> UserExposureExistsAsync(string usrId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if UserExposure exists: {UsrId}", usrId);

        try
        {
            var exposure = await GetUserExposureByIdAsync(usrId, cancellationToken);
            return exposure != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking UserExposure existence: {UsrId}", usrId);
            return false;
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<UserExposureDto>> GetUserExposureUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? searchTerm = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       CancellationToken cancellationToken = default)
    {
        // Create request model for validation
        var request = new GetUserExposureWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            IsAuth = isAuth
        };

        // Validate the request using FluentValidation
        await ValidateWorkflowListRequestAsync(request, cancellationToken);

        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} UserExposure list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = pageSize,
                ["SearchText"] = searchTerm ?? (object)DBNull.Value,
                ["SortColumn"] = "UsrID",
                ["SortDirection"] = "ASC",
                ["isAuth"] = (byte)isAuth, // 0: Unauthorized or 2: Denied records
                ["MakerId"] = _currentUserService.UserId,
                ["TotalCount"] = 0 // OUTPUT parameter - will be populated by SP
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetUserExpsListWF", pageNumber, pageSize, isAuth, _currentUserService.UserId);

            var result = await _repository.QueryPagedAsync<UserExposureDto>(
                sqlOrSp: "LB_SP_GetUserExpsListWF",
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
                    "LB_SP_GetUserExpsListWF");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting " + authAction + " user exposure list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);
            throw;
        }
    }

    public async Task<bool> AuthorizeUserExposureAsync(string usrId, AuthorizeUserExposureRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} UserExposure in workflow: {UsrId}", usrId, authAction);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var ipAddress = _currentUserService.GetClientIPAddress();

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = request.ActionType,
                ["UsrID"] = usrId,
                ["IPAddress"] = ipAddress,
                ["AuthID"] = _currentUserService.UserId,
                ["IsAuth"] = request.IsAuth,
                ["ActionType"] = request.ActionType,
                ["Remarks"] = !string.IsNullOrEmpty(request.Remarks) ? request.Remarks : (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };
            
            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthUsrExpsInfo",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized UserExposure: {UsrId}, RowsAffected: {RowsAffected}",
                    usrId, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("No rows affected during {authAction} authorization of UserExposure: {UsrId}", usrId, authAction);
                throw new DomainException($"Failed to authorize: {authAction} user exposure: No records were updated");
            }
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error {authAction} authorizing UserExposure in workflow: {UsrId}", usrId, authAction);
            throw new DomainException($"Failed to authorize: {authAction} user exposure: {ex.Message}");
        }
    }
    #endregion

    #region Private Validation Methods

    private async Task ValidateCreateRequestAsync(CreateUserExposureRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateUpdateRequestAsync(UpdateUserExposureRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateDeleteRequestAsync(DeleteUserExposureRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateAuthorizeRequestAsync(AuthorizeUserExposureRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _authorizeValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Authorization validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetUserExposureWorkflowListRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _workflowListValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Workflow list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion

    #region Private Business Logic Methods

    /// <summary>
    /// Applies business logic rules for buy/sell vs total exposure
    /// Rule: If buy and sell are checked, then total should be unchecked and amount 0
    ///       If total is checked, then buy and sell should be unchecked and amounts 0
    /// </summary>
    private static void ApplyBusinessLogicRules(CreateUserExposureRequest request)
    {
        // If both buy and sell are checked
        if (request.UsrExpsCheckBuy && request.UsrExpsCheckSell)
        {
            // Force total to be unchecked and amount to 0
            request.UsrExpsCheckTotal = false;
            request.UsrExpsTotalAmt = 0.00m;
        }
        // If total is checked
        else if (request.UsrExpsCheckTotal)
        {
            // Force buy and sell to be unchecked and amounts to 0
            request.UsrExpsCheckBuy = false;
            request.UsrExpsBuyAmt = 0.00m;
            request.UsrExpsCheckSell = false;
            request.UsrExpsSellAmt = 0.00m;
        }
    }

    /// <summary>
    /// Applies business logic rules for buy/sell vs total exposure for updates
    /// Rule: If buy and sell are checked, then total should be unchecked and amount 0
    ///       If total is checked, then buy and sell should be unchecked and amounts 0
    /// </summary>
    private static void ApplyBusinessLogicRules(UpdateUserExposureRequest request)
    {
        // If total is being checked, reset buy/sell
        if (request.UsrExpsCheckTotal.HasValue && request.UsrExpsCheckTotal.Value)
        {
            request.UsrExpsCheckBuy = false;
            request.UsrExpsBuyAmt = 0.00m;
            request.UsrExpsCheckSell = false;
            request.UsrExpsSellAmt = 0.00m;
        }
        // If both buy and sell are being set to true, reset total
        else if ((request.UsrExpsCheckBuy.HasValue && request.UsrExpsCheckBuy.Value) && 
                 (request.UsrExpsCheckSell.HasValue && request.UsrExpsCheckSell.Value))
        {
            request.UsrExpsCheckTotal = false;
            request.UsrExpsTotalAmt = 0.00m;
        }
    }

    #endregion
}