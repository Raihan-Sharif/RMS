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
/// Title:       Client Exposure Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Client Exposure information
/// Creation:    17/Sep/2025
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

public class ClientExposureService : IClientExposureService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateClientExposureRequest> _updateValidator;
    private readonly IValidator<DeleteClientExposureRequest> _deleteValidator;
    private readonly IValidator<AuthorizeClientExposureRequest> _authorizeValidator;
    private readonly IValidator<GetClientExposureWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ClientExposureService> _logger;

    public ClientExposureService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateClientExposureRequest> updateValidator,
        IValidator<DeleteClientExposureRequest> deleteValidator,
        IValidator<AuthorizeClientExposureRequest> authorizeValidator,
        IValidator<GetClientExposureWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<ClientExposureService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
        _workflowListValidator = workflowListValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Main CRUD Operations

    public async Task<PagedResult<ClientExposureDto>> GetClientExposureListAsync(int pageNumber = 1, int pageSize = 10,
        string? clntCode = null, string? coBrchCode = null, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged Client Exposure list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                ClntCode = clntCode,
                CoBrchCode = coBrchCode,
                SearchText = searchTerm,
                SortColumn = "ClntCode",
                SortDirection = "ASC",
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetMstClntExpsList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, ClntCode={ClntCode}, CoBrchCode={CoBrchCode}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, clntCode, coBrchCode, searchTerm);

            var result = await _repository.QueryPagedAsync<ClientExposureDto>(
                sqlOrSp: "LB_SP_GetMstClntExpsList",
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
            _logger.LogError(ex, "Invalid arguments for Client Exposure list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting Client Exposure list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting Client Exposure list");
            throw new DomainException($"Failed to retrieve client exposure list: {ex.Message}");
        }
    }

    public async Task<ClientExposureDto?> GetClientExposureByIdAsync(string clntCode, string coBrchCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client Exposure by ID: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);

        try
        {
            var parameters = new
            {
                ClntCode = clntCode,
                CoBrchCode = coBrchCode,
                StatusCode = 0, // output param
                StatusMsg = "" // output param
            };

            var result = await _repository.QuerySingleAsync<ClientExposureDto>(
                sqlOrSp: "LB_SP_GetMstClntExpsByClientID",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved client exposure: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client Exposure retrieval: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting Client Exposure by ID: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting Client Exposure by ID: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
            throw new DomainException($"Failed to retrieve client exposure: {ex.Message}");
        }
    }

    public async Task<ClientExposureDto> UpdateClientExposureAsync(string clntCode, string coBrchCode, UpdateClientExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Client Exposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);

        // Use existing validation method
        await ValidateUpdateRequestAsync(request, cancellationToken);

        // Apply business logic: validate amounts and dates
        ApplyBusinessLogicRules(request);

        try
        {
            var existingExposure = await GetClientExposureByIdAsync(clntCode, coBrchCode, cancellationToken);
            if (existingExposure == null)
            {
                throw new DomainException($"Client exposure not found: {clntCode}, {coBrchCode}");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.UPDATE,
                ["ClntCode"] = clntCode,
                ["CoBrchCode"] = coBrchCode,
                ["PendingClntExpsBuyAmtTopUp"] = request.ClntExpsBuyAmtTopUp ?? (object)DBNull.Value,
                //["ClntExpsBuyAmtTopUpExpiry"] = request.ClntExpsBuyAmtTopUpExpiry ?? (object)DBNull.Value,
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
                //["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudMstClntExps with Action=2 (UPDATE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstClntExps",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Update operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("Failed to update client exposure - no rows affected");
            }

            var updatedExposure = await GetClientExposureByIdAsync(clntCode, coBrchCode, cancellationToken);

            if (updatedExposure == null)
            {
                throw new DomainException($"Updated client exposure not found: {clntCode}, {coBrchCode}");
            }

            _logger.LogInformation("Successfully updated Client Exposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
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
            _logger.LogError(ex, "Unexpected error updating Client Exposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
            throw new DomainException($"Failed to update client exposure: {ex.Message}");
        }
    }

    public async Task<bool> DeleteClientExposureAsync(string clntCode, string coBrchCode, DeleteClientExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Client Exposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);

        // Use existing validation method
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var existingExposure = await GetClientExposureByIdAsync(clntCode, coBrchCode, cancellationToken);
            if (existingExposure == null)
            {
                throw new DomainException($"Client exposure not found: {clntCode}, {coBrchCode}");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.DELETE,
                ["ClntCode"] = clntCode,
                ["CoBrchCode"] = coBrchCode,
                ["PendingClntExpsBuyAmtTopUp"] = (object)DBNull.Value,

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
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudMstClntExps with Action=3 (DELETE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstClntExps",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Delete operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            var success = rowsAffected > 0;

            if (success)
            {
                _logger.LogInformation("Successfully deleted Client Exposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
            }
            else
            {
                _logger.LogWarning("Delete operation failed - no rows affected: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
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
            _logger.LogError(ex, "Unexpected error deleting Client Exposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
            throw new DomainException($"Failed to delete client exposure: {ex.Message}");
        }
    }

    public async Task<bool> ClientExposureExistsAsync(string clntCode, string coBrchCode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if Client Exposure exists: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);

        try
        {
            var exposure = await GetClientExposureByIdAsync(clntCode, coBrchCode, cancellationToken);
            return exposure != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Client Exposure existence: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode);
            return false;
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<ClientExposureDto>> GetClientExposureUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? clntCode = null,
       string? coBrchCode = null,
       string? searchTerm = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       CancellationToken cancellationToken = default)
    {
        // Create request model for validation
        var request = new GetClientExposureWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            ClntCode = clntCode,
            CoBrchCode = coBrchCode,
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

        _logger.LogInformation("Getting {authAction} Client Exposure list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = pageSize,
                ["CoBrchCode"] = coBrchCode ?? (object)DBNull.Value,
                ["ClntCode"] = clntCode ?? (object)DBNull.Value,
                ["SearchText"] = searchTerm ?? (object)DBNull.Value,
                ["SortColumn"] = "ClntCode",
                ["SortDirection"] = "ASC",
                ["isAuth"] = (byte)isAuth, // 0: Unauthorized or 2: Denied records
                ["MakerId"] = _currentUserService.UserId,
                ["TotalCount"] = 0 // OUTPUT parameter - will be populated by SP
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetClientMstExposureListWF", pageNumber, pageSize, isAuth, _currentUserService.UserId);

            var result = await _repository.QueryPagedAsync<ClientExposureDto>(
                sqlOrSp: "LB_SP_GetClientMstExposureListWF",
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
                    "LB_SP_GetClientMstExposureListWF");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting " + authAction + " client exposure list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);
            throw;
        }
    }

    public async Task<bool> AuthorizeClientExposureAsync(string clntCode, string coBrchCode, AuthorizeClientExposureRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} Client Exposure in workflow: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode, authAction);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var ipAddress = _currentUserService.GetClientIPAddress();

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = request.ActionType,
                ["ClntCode"] = clntCode,
                ["CoBrchCode"] = coBrchCode,
                ["IPAddress"] = ipAddress,
                ["AuthID"] = _currentUserService.UserId,
                ["IsAuth"] = request.IsAuth,
                ["ActionType"] = request.ActionType,
                ["Remarks"] = !string.IsNullOrEmpty(request.Remarks) ? request.Remarks : (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthMstClntExps",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized Client Exposure: {ClntCode}, {CoBrchCode}, RowsAffected: {RowsAffected}",
                    clntCode, coBrchCode, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("No rows affected during {authAction} authorization of Client Exposure: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode, authAction);
                throw new DomainException($"Failed to authorize: {authAction} client exposure: No records were updated");
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
            _logger.LogError(ex, "Error {authAction} authorizing Client Exposure in workflow: {ClntCode}, {CoBrchCode}", clntCode, coBrchCode, authAction);
            throw new DomainException($"Failed to authorize: {authAction} client exposure: {ex.Message}");
        }
    }
    #endregion

    #region Private Validation Methods

    private async Task ValidateUpdateRequestAsync(UpdateClientExposureRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Update Client Exposure validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateDeleteRequestAsync(DeleteClientExposureRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Delete Client Exposure validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeRequestAsync(AuthorizeClientExposureRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Authorization Client Exposure validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetClientExposureWorkflowListRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Workflow Client Exposure list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion

    #region Private Business Logic Methods

    /// <summary>
    /// Applies business logic rules for client exposure updates
    /// </summary>
    private static void ApplyBusinessLogicRules(UpdateClientExposureRequest request)
    {
        // Business rule: All amounts must be >= 0 (already validated in FluentValidation but double-check)
        if ((request.ClntExpsBuyAmtTopUp.HasValue && request.ClntExpsBuyAmtTopUp.Value < 1))
        {
            throw new DomainException("All exposure Top Up amounts must be greater than 0");
        }
    }

    #endregion
}