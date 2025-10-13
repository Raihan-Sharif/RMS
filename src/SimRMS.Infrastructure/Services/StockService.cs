using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Common;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Stock information
/// Creation:    23/Sep/2025
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

public class StockService : IStockService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetStockByKeyRequest> _getByKeyValidator;
    private readonly IValidator<GetStockListRequest> _getListValidator;
    private readonly IValidator<CreateStockRequest> _createValidator;
    private readonly IValidator<UpdateStockRequest> _updateValidator;
    private readonly IValidator<DeleteStockRequest> _deleteValidator;
    private readonly IValidator<AuthorizeStockRequest> _authorizeValidator;
    private readonly IValidator<GetStockWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StockService> _logger;

    public StockService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<GetStockByKeyRequest> getByKeyValidator,
        IValidator<GetStockListRequest> getListValidator,
        IValidator<CreateStockRequest> createValidator,
        IValidator<UpdateStockRequest> updateValidator,
        IValidator<DeleteStockRequest> deleteValidator,
        IValidator<AuthorizeStockRequest> authorizeValidator,
        IValidator<GetStockWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<StockService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _getByKeyValidator = getByKeyValidator;
        _getListValidator = getListValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
        _workflowListValidator = workflowListValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Main CRUD Operations

    public async Task<PagedResult<StockDto>> GetStockListAsync(int pageNumber = 1, int pageSize = 10,
        string? xchgCode = null, string? stkCode = null, string? searchTerm = null,
        string sortColumn = "StkCode", string sortDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged Stock list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Create request model for validation
        var request = new GetStockListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            XchgCode = xchgCode,
            StkCode = stkCode,
            SearchText = searchTerm,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };

        // Validate the request
        await ValidateGetListRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                XchgCode = xchgCode,
                StkCode = stkCode,
                SearchText = searchTerm,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetMstStkList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchText={SearchText}",
                pageNumber, pageSize, searchTerm);

            var result = await _repository.QueryPagedAsync<StockDto>(
                sqlOrSp: "LB_SP_GetMstStkList",
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
            _logger.LogError(ex, "Invalid arguments for Stock list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting Stock list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting Stock list");
            throw new DomainException($"Failed to retrieve stock list: {ex.Message}");
        }
    }

    public async Task<StockDto?> GetStockByKeyAsync(string xchgCode, string stkCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Stock by key: {XchgCode}-{StkCode}", xchgCode, stkCode);

        // Create request model for validation
        var request = new GetStockByKeyRequest
        {
            XchgCode = xchgCode,
            StkCode = stkCode
        };

        // Validate the request
        await ValidateGetByKeyRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new
            {
                XchgCode = xchgCode,
                StkCode = stkCode,
                StatusCode = 0, // output param
                StatusMsg = "" // output param
            };

            var result = await _repository.QuerySingleAsync<StockDto>(
                sqlOrSp: "LB_SP_GetMstStkByKey",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved stock: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Stock retrieval: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting Stock by key: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting Stock by key: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"Failed to retrieve stock: {ex.Message}");
        }
    }

    public async Task<StockDto> CreateStockAsync(CreateStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new Stock: {XchgCode}-{StkCode}", request.XchgCode, request.StkCode);

        // Validate the request
        await ValidateCreateRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (int)ActionTypeEnum.INSERT,
                ["XchgCode"] = request.XchgCode,
                ["StkCode"] = request.StkCode,
                ["StkBrdCode"] = request.StkBrdCode ?? (object)DBNull.Value,
                ["StkSectCode"] = request.StkSectCode ?? (object)DBNull.Value,
                ["StkLName"] = request.StkLName,
                ["StkSName"] = request.StkSName,
                ["StkLastDonePrice"] = request.StkLastDonePrice ?? (object)DBNull.Value,
                ["StkClosePrice"] = request.StkClosePrice ?? (object)DBNull.Value,
                ["StkRefPrc"] = request.StkRefPrc ?? (object)DBNull.Value,
                ["StkUpperLmtPrice"] = request.StkUpperLmtPrice ?? (object)DBNull.Value,
                ["StkLowerLmtPrice"] = request.StkLowerLmtPrice ?? (object)DBNull.Value,
                ["StkIsSyariah"] = request.StkIsSyariah ?? (object)DBNull.Value,
                ["StkLot"] = request.StkLot,
                ["LastUpdateDate"] = request.LastUpdateDate ?? (object)DBNull.Value,
                ["ISIN"] = request.ISIN,
                ["Currency"] = request.Currency ?? (object)DBNull.Value,
                ["StkParValue"] = request.StkParValue ?? (object)DBNull.Value,
                ["StkVolumeTraded"] = request.StkVolumeTraded ?? (object)DBNull.Value,
                ["YearHigh"] = request.YearHigh ?? (object)DBNull.Value,
                ["YearLow"] = request.YearLow ?? (object)DBNull.Value,
                ["SecurityType"] = request.SecurityType ?? (object)DBNull.Value,
                ["ListingDate"] = request.ListingDate ?? (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = (object)DBNull.Value,
                ["TransDt"] = (object)DBNull.Value,
                ["ActionType"] = (byte)ActionTypeEnum.INSERT,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize, // Always start unauthorized
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0
                
            };

            _logger.LogDebug("Calling LB_SP_CrudMstStk for INSERT with XchgCode={XchgCode}, StkCode={StkCode}",
                request.XchgCode, request.StkCode);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var errorMessage = result.GetOutputValue<string>("ErrorMessage");

            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to create stock - no rows affected. Error Message: {errorMessage}");
            }

            _logger.LogDebug("Stock created successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}, Error Message: {errorMessage}",
                request.XchgCode, request.StkCode, rowsAffected, errorMessage);

            var createdStock = await GetStockByKeyAsync(request.XchgCode, request.StkCode, cancellationToken);
            if (createdStock == null)
            {
                throw new DomainException($"Created stock not found: {request.XchgCode}-{request.StkCode}");
            }

            return createdStock;
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
            _logger.LogError(ex, "Unexpected error creating Stock: {XchgCode}-{StkCode}", request.XchgCode, request.StkCode);
            throw new DomainException($"Failed to create stock: {ex.Message}");
        }
    }

    public async Task<StockDto> UpdateStockAsync(string xchgCode, string stkCode, UpdateStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);

        // Validate the request
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;
        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (int)ActionTypeEnum.UPDATE,
                ["XchgCode"] = xchgCode,
                ["StkCode"] = stkCode,
                ["StkBrdCode"] = request.StkBrdCode ?? (object)DBNull.Value,
                ["StkSectCode"] = request.StkSectCode ?? (object)DBNull.Value,
                ["StkLName"] = request.StkLName ?? (object)DBNull.Value,
                ["StkSName"] = request.StkSName ?? (object)DBNull.Value,
                ["StkLastDonePrice"] = request.StkLastDonePrice ?? (object)DBNull.Value,
                ["StkClosePrice"] = request.StkClosePrice ?? (object)DBNull.Value,
                ["StkRefPrc"] = request.StkRefPrc ?? (object)DBNull.Value,
                ["StkUpperLmtPrice"] = request.StkUpperLmtPrice ?? (object)DBNull.Value,
                ["StkLowerLmtPrice"] = request.StkLowerLmtPrice ?? (object)DBNull.Value,
                ["StkIsSyariah"] = request.StkIsSyariah ?? (object)DBNull.Value,
                ["StkLot"] = request.StkLot ?? (object)DBNull.Value,
                ["LastUpdateDate"] = (object)DBNull.Value,
                ["ISIN"] = request.ISIN ?? (object)DBNull.Value,
                ["Currency"] = request.Currency ?? (object)DBNull.Value,
                ["StkParValue"] = request.StkParValue ?? (object)DBNull.Value,
                ["StkVolumeTraded"] = request.StkVolumeTraded ?? (object)DBNull.Value,
                ["YearHigh"] = request.YearHigh ?? (object)DBNull.Value,
                ["YearLow"] = request.YearLow ?? (object)DBNull.Value,
                ["SecurityType"] = request.SecurityType ?? (object)DBNull.Value,
                ["ListingDate"] = request.ListingDate ?? (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = (object)DBNull.Value,
                ["TransDt"] = (object)DBNull.Value,
                ["ActionType"] = (byte)ActionTypeEnum.UPDATE,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize, // Reset to unauthorized for workflow
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0
            };

            _logger.LogDebug("Calling LB_SP_CrudMstStk for UPDATE with XchgCode={XchgCode}, StkCode={StkCode}",
                xchgCode, stkCode);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var errorMessage = result.GetOutputValue<string>("ErrorMessage");


            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to update stock - no rows affected or record not found. Error Message: {errorMessage}");
            }

            _logger.LogDebug("Stock updated successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}, Error Message: {errorMessage}",
                request.XchgCode, request.StkCode, rowsAffected, errorMessage);

            var updatedStock = await GetStockByKeyAsync(xchgCode, stkCode, cancellationToken);
            if (updatedStock == null)
            {
                throw new DomainException($"Updated stock not found: {xchgCode}-{stkCode}");
            }

            return updatedStock;
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
            _logger.LogError(ex, "Unexpected error updating Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"Failed to update stock: {ex.Message}");
        }
    }

    public async Task<bool> DeleteStockAsync(string xchgCode, string stkCode, DeleteStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);

        // Validate the request
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (int)ActionTypeEnum.DELETE,
                ["XchgCode"] = xchgCode,
                ["StkCode"] = stkCode,
                ["StkBrdCode"] = (object)DBNull.Value,
                ["StkSectCode"] = (object)DBNull.Value,
                ["StkLName"] = (object)DBNull.Value,
                ["StkSName"] = (object)DBNull.Value,
                ["StkLastDonePrice"] = (object)DBNull.Value,
                ["StkClosePrice"] = (object)DBNull.Value,
                ["StkRefPrc"] = (object)DBNull.Value,
                ["StkUpperLmtPrice"] = (object)DBNull.Value,
                ["StkLowerLmtPrice"] = (object)DBNull.Value,
                ["StkIsSyariah"] = (object)DBNull.Value,
                ["StkLot"] = (object)DBNull.Value,
                ["LastUpdateDate"] = (object)DBNull.Value,
                ["ISIN"] = (object)DBNull.Value,
                ["Currency"] = (object)DBNull.Value,
                ["StkParValue"] = (object)DBNull.Value,
                ["StkVolumeTraded"] = (object)DBNull.Value,
                ["YearHigh"] = (object)DBNull.Value,
                ["YearLow"] = (object)DBNull.Value,
                ["SecurityType"] = (object)DBNull.Value,
                ["ListingDate"] = (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = (object)DBNull.Value,
                ["TransDt"] = (object)DBNull.Value,
                ["ActionType"] = (byte)ActionTypeEnum.DELETE,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (object)DBNull.Value, // SP handles deletion logic
                ["AuthLevel"] = (object)DBNull.Value,
                ["IsDel"] = (object)DBNull.Value, // SP sets this to 1
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0
            };

            _logger.LogDebug("Calling LB_SP_CrudMstStk for DELETE with XchgCode={XchgCode}, StkCode={StkCode}",
                xchgCode, stkCode);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var errorMessage = result.GetOutputValue<string>("ErrorMessage");

            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to delete stock - no rows affected or record not found. Error Message: {errorMessage}");
            }

            _logger.LogDebug("Stock deleted successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}, Error Message: {errorMessage}",
                request.XchgCode, request.StkCode, rowsAffected, errorMessage);

            return true;
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
            _logger.LogError(ex, "Unexpected error deleting Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"Failed to delete stock: {ex.Message}");
        }
    }

    public async Task<bool> StockExistsAsync(string xchgCode, string stkCode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if Stock exists: {XchgCode}-{StkCode}", xchgCode, stkCode);

        try
        {
            var stock = await GetStockByKeyAsync(xchgCode, stkCode, cancellationToken);
            return stock != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Stock existence: {XchgCode}-{StkCode}", xchgCode, stkCode);
            return false;
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<StockDto>> GetStockUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? searchTerm = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       string sortColumn = "StkCode",
       string sortDirection = "ASC",
       CancellationToken cancellationToken = default)
    {
        // Create request model for validation
        var request = new GetStockWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchText = searchTerm,
            IsAuth = (byte)isAuth,
            MakerId = _currentUserService.UserId,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };

        // Validate the request using FluentValidation
        await ValidateWorkflowListRequestAsync(request, cancellationToken);

        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} Stock list for workflow - Page: {PageNumber}, Size: {PageSize}", authAction, pageNumber, pageSize);

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                XchgCode = (object)DBNull.Value,
                StkCode = (object)DBNull.Value,
                isAuth = (byte)isAuth,
                MakerId = _currentUserService.UserId,
                SearchText = searchTerm,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetMstStkListWF with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                pageNumber, pageSize, isAuth, _currentUserService.UserId);

            var result = await _repository.QueryPagedAsync<StockDto>(
                sqlOrSp: "LB_SP_GetMstStkListWF",
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
            _logger.LogError(ex, "Invalid arguments for workflow Stock list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting workflow Stock list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting {authAction} stock list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchText={SearchText}",
                authAction, pageNumber, pageSize, searchTerm);
            throw new DomainException($"Failed to retrieve workflow stock list: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeStockAsync(string xchgCode, string stkCode, AuthorizeStockRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} Stock in workflow: {XchgCode}-{StkCode}", authAction, xchgCode, stkCode);

        // Validate request
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (int)ActionTypeEnum.UPDATE, // Always 2 for AUTH operation as per SP
                ["XchgCode"] = xchgCode,
                ["StkCode"] = stkCode,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["AuthId"] = _currentUserService.UserId,
                ["AuthDt"] = (object)DBNull.Value,
                ["IsAuth"] = request.IsAuth,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["ActionType"] = (int)ActionTypeEnum.UPDATE,
                ["RowsAffected"] = 0
            };

            _logger.LogDebug("Calling LB_SP_AuthMstStk for {authAction} with XchgCode={XchgCode}, StkCode={StkCode}, IsAuth={IsAuth}",
                authAction, xchgCode, stkCode, request.IsAuth);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to {authAction.ToLower()} stock - no rows affected or record not found");
            }

            _logger.LogDebug("Stock {authAction} successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}",
                authAction, xchgCode, stkCode, rowsAffected);

            return true;
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
            _logger.LogError(ex, "Error {authAction} authorizing Stock in workflow: {XchgCode}-{StkCode}", authAction, xchgCode, stkCode);
            throw new DomainException($"Failed to {authAction.ToLower()} stock: {ex.Message}");
        }
    }
    #endregion

    #region Private Validation Methods

    private async Task ValidateGetByKeyRequestAsync(GetStockByKeyRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _getByKeyValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Get Stock by key validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateGetListRequestAsync(GetStockListRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _getListValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Get Stock list validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateCreateRequestAsync(CreateStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Create Stock validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateUpdateRequestAsync(UpdateStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Update Stock validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateDeleteRequestAsync(DeleteStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Delete Stock validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeRequestAsync(AuthorizeStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Authorization Stock validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetStockWorkflowListRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("Workflow Stock list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion
}