using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Common;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Exposure Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Stock Exposure information
/// Creation:    08/Oct/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
///
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

public class StockExposureService : IStockExposureService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateStockExposureRequest> _createValidator;
    private readonly IValidator<UpdateStockExposureRequest> _updateValidator;
    private readonly IValidator<DeleteStockExposureRequest> _deleteValidator;
    private readonly IValidator<AuthorizeStockExposureRequest> _authorizeValidator;
    private readonly IValidator<GetStockExposureWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StockExposureService> _logger;

    public StockExposureService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateStockExposureRequest> createValidator,
        IValidator<UpdateStockExposureRequest> updateValidator,
        IValidator<DeleteStockExposureRequest> deleteValidator,
        IValidator<AuthorizeStockExposureRequest> authorizeValidator,
        IValidator<GetStockExposureWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<StockExposureService> logger)
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

    public async Task<PagedResult<StockExposureDto>> GetStockExposureListAsync(int pageNumber = 1, int pageSize = 10,
        string? usrID = null, string? clntCode = null, string? stkCode = null, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged Stock Exposure list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                UsrID = usrID,
                ClntCode = clntCode,
                StkCode = stkCode,
                SearchText = searchTerm,
                SortColumn = "StkCode",
                SortDirection = "ASC",
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetStkCtrlList with parameters: PageNumber={PageNumber}, PageSize={PageSize}",
                pageNumber, pageSize);

            var result = await _repository.QueryPagedAsync<StockExposureDto>(
                sqlOrSp: "LB_SP_GetStkCtrlList",
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
            _logger.LogError(ex, "Invalid arguments for Stock Exposure list retrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Stock Exposure list");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Stock Exposure list");
            throw new DomainException($"The stock exposure list retrieval failed: {ex.Message}");
        }
    }

    public async Task<StockExposureDto?> GetStockExposureByKeyAsync(GetStockExposureByKeyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Stock Exposure by key: DataType={DataType}, CtrlType={CtrlType}, StkCode={StkCode}",
            request.DataType, request.CtrlType, request.StkCode);

        try
        {
            var parameters = new
            {
                PageNumber = 1,
                PageSize = 1,
                request.DataType,
                request.CtrlType,
                request.StkCode,
                request.CoCode,
                request.CoBrchCode,
                request.UsrID,
                request.ClntCode,
                request.ClntType,
                StatusCode = 0, // output param
                StatusMsg = "" // output param
            };

            var result = await _repository.QuerySingleAsync<StockExposureDto>(
                sqlOrSp: "LB_SP_GetStkCtrlByKey",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved stock exposure: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Stock Exposure retrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Stock Exposure by key");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Stock Exposure by key");
            throw new DomainException($"The stock exposure retrieval failed: {ex.Message}");
        }
    }

    public async Task<StockExposureDto> CreateStockExposureAsync(CreateStockExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Stock Exposure: DataType={DataType}, StkCode={StkCode}", request.DataType, request.StkCode);

        await ValidateCreateRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.INSERT,
                ["DataType"] = request.DataType,
                ["CtrlType"] = request.CtrlType,
                ["StkCode"] = request.StkCode,
                ["CoCode"] = request.CoCode ?? (object)DBNull.Value,
                ["CoBrchCode"] = request.CoBrchCode ?? (object)DBNull.Value,
                ["UsrID"] = request.UsrID ?? (object)DBNull.Value,
                ["ClntCode"] = request.ClntCode ?? (object)DBNull.Value,
                ["ClntType"] = request.ClntType ?? (object)DBNull.Value,
                ["XchgCode"] = request.XchgCode ?? (object)DBNull.Value,
                ["CtrlStatus"] = request.CtrlStatus,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.INSERT,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize,
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudStkCtrl with Action=1 (INSERT)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudStkCtrl",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Insert operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("Failed to create stock exposure - no rows affected");
            }

            var createdExposure = await GetStockExposureByKeyAsync(new GetStockExposureByKeyRequest
            {
                DataType = request.DataType,
                CtrlType = request.CtrlType,
                StkCode = request.StkCode,
                CoCode = request.CoCode,
                CoBrchCode = request.CoBrchCode,
                UsrID = request.UsrID,
                ClntCode = request.ClntCode,
                ClntType = request.ClntType
            }, cancellationToken);

            if (createdExposure == null)
            {
                throw new DomainException("Created stock exposure not found");
            }

            _logger.LogInformation("Successfully created Stock Exposure: DataType={DataType}, StkCode={StkCode}", request.DataType, request.StkCode);
            return createdExposure;
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
            _logger.LogError(ex, "An unexpected error occurred while creating Stock Exposure");
            throw new DomainException($"The stock exposure creation failed: {ex.Message}");
        }
    }

    public async Task<StockExposureDto> UpdateStockExposureAsync(UpdateStockExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Stock Exposure: DataType={DataType}, StkCode={StkCode}", request.DataType, request.StkCode);

        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
            var existingExposure = await GetStockExposureByKeyAsync(new GetStockExposureByKeyRequest
            {
                DataType = request.DataType,
                CtrlType = request.CtrlType,
                StkCode = request.StkCode,
                CoCode = request.CoCode,
                CoBrchCode = request.CoBrchCode,
                UsrID = request.UsrID,
                ClntCode = request.ClntCode,
                ClntType = request.ClntType
            }, cancellationToken);

            if (existingExposure == null)
            {
                throw new DomainException($"Stock exposure not found");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.UPDATE,
                ["DataType"] = request.DataType,
                ["CtrlType"] = request.CtrlType,
                ["StkCode"] = request.StkCode,
                ["CoCode"] = request.CoCode ?? (object)DBNull.Value,
                ["CoBrchCode"] = request.CoBrchCode ?? (object)DBNull.Value,
                ["UsrID"] = request.UsrID ?? (object)DBNull.Value,
                ["ClntCode"] = request.ClntCode ?? (object)DBNull.Value,
                ["ClntType"] = request.ClntType ?? (object)DBNull.Value,
                ["XchgCode"] = (object)DBNull.Value,
                ["CtrlStatus"] = request.CtrlStatus ?? (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.UPDATE,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize,
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudStkCtrl with Action=2 (UPDATE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudStkCtrl",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Update operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("The stock exposure update failed because no rows were affected");
            }

            var updatedExposure = await GetStockExposureByKeyAsync(new GetStockExposureByKeyRequest
            {
                DataType = request.DataType,
                CtrlType = request.CtrlType,
                StkCode = request.StkCode,
                CoCode = request.CoCode,
                CoBrchCode = request.CoBrchCode,
                UsrID = request.UsrID,
                ClntCode = request.ClntCode,
                ClntType = request.ClntType
            }, cancellationToken);

            if (updatedExposure == null)
            {
                throw new DomainException("Updated stock exposure not found");
            }

            _logger.LogInformation("Successfully updated Stock Exposure: DataType={DataType}, StkCode={StkCode}", request.DataType, request.StkCode);
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
            _logger.LogError(ex, "An unexpected error occurred while updating Stock Exposure");
            throw new DomainException($"The stock exposure update failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteStockExposureAsync(DeleteStockExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Stock Exposure: DataType={DataType}, StkCode={StkCode}", request.DataType, request.StkCode);

        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var existingExposure = await GetStockExposureByKeyAsync(new GetStockExposureByKeyRequest
            {
                DataType = request.DataType,
                CtrlType = request.CtrlType,
                StkCode = request.StkCode,
                CoCode = request.CoCode,
                CoBrchCode = request.CoBrchCode,
                UsrID = request.UsrID,
                ClntCode = request.ClntCode,
                ClntType = request.ClntType
            }, cancellationToken);

            if (existingExposure == null)
            {
                throw new DomainException($"Stock exposure not found");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.DELETE,
                ["DataType"] = request.DataType,
                ["CtrlType"] = request.CtrlType,
                ["StkCode"] = request.StkCode,
                ["CoCode"] = request.CoCode ?? (object)DBNull.Value,
                ["CoBrchCode"] = request.CoBrchCode ?? (object)DBNull.Value,
                ["UsrID"] = request.UsrID ?? (object)DBNull.Value,
                ["ClntCode"] = request.ClntCode ?? (object)DBNull.Value,
                ["ClntType"] = request.ClntType ?? (object)DBNull.Value,
                ["XchgCode"] = (object)DBNull.Value,
                ["CtrlStatus"] = (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.DELETE,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (object)DBNull.Value,
                ["AuthLevel"] = (object)DBNull.Value,
                ["IsDel"] = (object)DBNull.Value,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudStkCtrl with Action=3 (DELETE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudStkCtrl",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Delete operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            var success = rowsAffected > 0;

            if (success)
            {
                _logger.LogInformation("Successfully deleted Stock Exposure: DataType={DataType}, StkCode={StkCode}", request.DataType, request.StkCode);
            }
            else
            {
                _logger.LogWarning("The delete operation failed because no rows were affected");
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
            _logger.LogError(ex, "An unexpected error occurred while deleting Stock Exposure");
            throw new DomainException($"The stock exposure deletion failed: {ex.Message}");
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<StockExposureDto>> GetStockExposureUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? usrID = null,
       string? clntCode = null,
       string? stkCode = null,
       string? searchTerm = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       CancellationToken cancellationToken = default)
    {
        var request = new GetStockExposureWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            UsrID = usrID,
            ClntCode = clntCode,
            StkCode = stkCode,
            SearchTerm = searchTerm,
            IsAuth = isAuth
        };

        await ValidateWorkflowListRequestAsync(request, cancellationToken);

        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} Stock Exposure list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = pageSize,
                ["UsrID"] = usrID ?? (object)DBNull.Value,
                ["ClntCode"] = clntCode ?? (object)DBNull.Value,
                ["StkCode"] = stkCode ?? (object)DBNull.Value,
                ["SearchText"] = searchTerm ?? (object)DBNull.Value,
                ["SortColumn"] = "StkCode",
                ["SortDirection"] = "ASC",
                ["isAuth"] = (byte)isAuth,
                ["MakerId"] = _currentUserService.UserId,
                ["TotalCount"] = 0
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetStkCtrlListWF", pageNumber, pageSize, isAuth, _currentUserService.UserId);

            var result = await _repository.QueryPagedAsync<StockExposureDto>(
                sqlOrSp: "LB_SP_GetStkCtrlListWF",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting " + authAction + " stock exposure list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);
            throw;
        }
    }

    public async Task<bool> AuthorizeStockExposureAsync(AuthorizeStockExposureRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} Stock Exposure in workflow: DataType={DataType}, StkCode={StkCode}",
            request.DataType, request.StkCode, authAction);

        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var ipAddress = _currentUserService.GetClientIPAddress();

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = request.ActionType,
                ["DataType"] = request.DataType,
                ["CtrlType"] = request.CtrlType,
                ["StkCode"] = request.StkCode,
                ["CoCode"] = request.CoCode ?? (object)DBNull.Value,
                ["CoBrchCode"] = request.CoBrchCode ?? (object)DBNull.Value,
                ["UsrID"] = request.UsrID ?? (object)DBNull.Value,
                ["ClntCode"] = request.ClntCode ?? (object)DBNull.Value,
                ["ClntType"] = request.ClntType ?? (object)DBNull.Value,
                ["XchgCode"] = (object)DBNull.Value,
                ["CtrlStatus"] = (object)DBNull.Value,
                ["IPAddress"] = ipAddress,
                ["AuthID"] = _currentUserService.UserId,
                ["IsAuth"] = request.IsAuth,
                ["ActionType"] = request.ActionType,
                ["Remarks"] = !string.IsNullOrEmpty(request.Remarks) ? request.Remarks : (object)DBNull.Value,
                ["RowsAffected"] = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthStkCtrl",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized Stock Exposure: DataType={DataType}, StkCode={StkCode}, RowsAffected: {RowsAffected}",
                    request.DataType, request.StkCode, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("No rows affected during {authAction} authorization of Stock Exposure", authAction);
                throw new DomainException($"Failed to authorize: {authAction} stock exposure: No records were updated");
            }
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
            _logger.LogError(ex, "Error {authAction} authorizing Stock Exposure in workflow", authAction);
            throw new DomainException($"Failed to authorize: {authAction} stock exposure: {ex.Message}");
        }
    }
    #endregion

    #region Private Validation Methods

    private async Task ValidateCreateRequestAsync(CreateStockExposureRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Create Stock Exposure validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateUpdateRequestAsync(UpdateStockExposureRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Update Stock Exposure validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateDeleteRequestAsync(DeleteStockExposureRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Delete Stock Exposure validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeRequestAsync(AuthorizeStockExposureRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Authorization Stock Exposure validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetStockExposureWorkflowListRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Workflow Stock Exposure list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion
}
