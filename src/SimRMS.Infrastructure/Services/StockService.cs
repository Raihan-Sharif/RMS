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
            SearchTerm = searchTerm,
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

            _logger.LogDebug("Calling LB_SP_GetMstStkList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
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
            // TODO: Implement when LB_SP_CrudMstStk stored procedure is available
            throw new NotImplementedException("Create Stock operation requires LB_SP_CrudMstStk stored procedure to be implemented");

            // Implementation placeholder for when SP is available:
            /*
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.INSERT,
                ["XchgCode"] = request.XchgCode,
                ["StkBrdCode"] = request.StkBrdCode,
                ["StkSectCode"] = request.StkSectCode,
                ["StkCode"] = request.StkCode,
                ["StkLName"] = request.StkLName,
                ["StkSName"] = request.StkSName,
                ["ISIN"] = request.ISIN ?? (object)DBNull.Value,
                ["Currency"] = request.Currency ?? (object)DBNull.Value,
                ["SecurityType"] = request.SecurityType ?? (object)DBNull.Value,
                ["StkIsSyariah"] = request.StkIsSyariah ?? (object)DBNull.Value,
                ["StkLot"] = request.StkLot ?? (object)DBNull.Value,
                ["StkParValue"] = request.StkParValue ?? (object)DBNull.Value,
                ["StkLastDonePrice"] = request.StkLastDonePrice ?? (object)DBNull.Value,
                ["StkClosePrice"] = request.StkClosePrice ?? (object)DBNull.Value,
                ["StkRefPrc"] = request.StkRefPrc ?? (object)DBNull.Value,
                ["StkUpperLmtPrice"] = request.StkUpperLmtPrice ?? (object)DBNull.Value,
                ["StkLowerLmtPrice"] = request.StkLowerLmtPrice ?? (object)DBNull.Value,
                ["YearHigh"] = request.YearHigh ?? (object)DBNull.Value,
                ["YearLow"] = request.YearLow ?? (object)DBNull.Value,
                ["StkVolumeTraded"] = request.StkVolumeTraded ?? (object)DBNull.Value,
                ["ListingDate"] = request.ListingDate ?? (object)DBNull.Value,
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
                ["RowsAffected"] = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected <= 0)
            {
                throw new DomainException("Failed to create stock - no rows affected");
            }

            var createdStock = await GetStockByKeyAsync(request.XchgCode, request.StkCode, cancellationToken);
            if (createdStock == null)
            {
                throw new DomainException($"Created stock not found: {request.XchgCode}-{request.StkCode}");
            }

            return createdStock;
            */
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
            // TODO: Implement when LB_SP_CrudMstStk stored procedure is available
            throw new NotImplementedException("Update Stock operation requires LB_SP_CrudMstStk stored procedure to be implemented");
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
            // TODO: Implement when LB_SP_CrudMstStk stored procedure is available
            throw new NotImplementedException("Delete Stock operation requires LB_SP_CrudMstStk stored procedure to be implemented");
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
            SearchTerm = searchTerm,
            IsAuth = isAuth,
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

        _logger.LogInformation("Getting {authAction} Stock list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        try
        {
            // TODO: Implement when LB_SP_GetMstStkListWF stored procedure is available
            throw new NotImplementedException("Workflow Stock list operation requires LB_SP_GetMstStkListWF stored procedure to be implemented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting " + authAction + " stock list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);
            throw;
        }
    }

    public async Task<bool> AuthorizeStockAsync(string xchgCode, string stkCode, AuthorizeStockRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} Stock in workflow: {XchgCode}-{StkCode}", xchgCode, stkCode, authAction);

        // Validate request
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            // TODO: Implement when LB_SP_AuthMstStk stored procedure is available
            throw new NotImplementedException("Authorize Stock operation requires LB_SP_AuthMstStk stored procedure to be implemented");
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
            _logger.LogError(ex, "Error {authAction} authorizing Stock in workflow: {XchgCode}-{StkCode}", xchgCode, stkCode, authAction);
            throw new DomainException($"Failed to authorize: {authAction} stock: {ex.Message}");
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

            throw new ValidationException("Get by key validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Get list validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Create validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Update validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Delete validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Authorization validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Workflow list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion
}