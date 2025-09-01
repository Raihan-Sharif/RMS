using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Validators;
using SimRMS.Domain.Common;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Infrastructure.Common;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;
using System.Threading.Tasks;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Trader Service
/// Author:      Asif Zaman
/// Purpose:     This service provides methods for managing Market Stock Trader information
/// Creation:    20/Aug/2025
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
/// Trader service with business logic and validation
/// </summary>
public class TraderService : ITraderService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateMstTraderRequest> _createValidator;
    private readonly IValidator<UpdateMstTraderRequest> _updateValidator;
    private readonly IValidator<DeleteMstTraderRequest> _deleteValidator;
    private readonly IValidator<GetTraderWorkflowListRequest> _workflowListValidator;
    private readonly IValidator<AuthorizeMstTraderRequest> _authorizeValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TraderService> _logger;

    public TraderService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateMstTraderRequest> createValidator,
        IValidator<UpdateMstTraderRequest> updateValidator,
        IValidator<DeleteMstTraderRequest> deleteValidator,
        IValidator<GetTraderWorkflowListRequest> workflowListValidator,
        IValidator<AuthorizeMstTraderRequest> authorizeValidator,
        ICurrentUserService currentUserService,
        ILogger<TraderService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _workflowListValidator = workflowListValidator;
        _authorizeValidator = authorizeValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }
    #region Traders List Operations
    public async Task<PagedResult<MstTraderDto>> GetMstTraderListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? xchgCode = null, string? sortDirection = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged MstTrader list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = "XchgCode",
                SortDirection = sortDirection,
                XchgCode = xchgCode,
                SearchTerm = searchTerm,
                IsAuth = (byte) AuthTypeEnum.Approve, // Authorized records
                TotalCount = 0 // OUTPUT parameter
            };

            var result = await _repository.QueryPagedAsync<MstTraderDto>(
                sqlOrSp: "LB_SP_GetTraderList",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for MstTrader list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting MstTrader list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting MstTrader list");
            throw new DomainException($"Failed to retrieve trader list: {ex.Message}");
        }
    }
    public async Task<MstTraderDto> GetMstTraderByIdAsync(string xchgCode, string dlrCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(xchgCode))
            throw new ArgumentException("Exchange code cannot be null or empty", nameof(xchgCode));

        if (string.IsNullOrWhiteSpace(dlrCode))
            throw new ArgumentException("Dealer code cannot be null or empty", nameof(dlrCode));

        _logger.LogInformation("Getting MstTrader by ID: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        try
        {
            // Create parameters matching the stored procedure signature
            var parameters = new
            {
                XchgCode = xchgCode,
                DlrCode = dlrCode,
                StatusCode = 0,     // OUTPUT parameter
                StatusMsg = ""      // OUTPUT parameter
            };

            // Call the stored procedure to get the trader data
            var trader = await _repository.QuerySingleAsync<MstTraderDto>(
                sqlOrSp: "LB_SP_GetTrader_ByTraderCode",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            if (trader != null)
            {
                _logger.LogInformation("Successfully retrieved trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            }
            else
            {
                _logger.LogWarning("Trader not found: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            }

            return trader; // This will be null if not found
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for MstTrader retrieval: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting MstTrader by ID: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting MstTrader by ID: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"Failed to retrieve trader: {ex.Message}");
        }
    }
    #endregion

    #region Traders CRUD Operations

    //create trader
    public async Task<MstTraderDto> CreateMstTraderAsync(CreateMstTraderRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "CreateMstTraderRequest cannot be null");
        }

        _logger.LogInformation("Creating new MstTrader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);

        // Validate the request using FluentValidation
        await ValidateCreateRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new
            {
                Action = (byte)ActionTypeEnum.INSERT, // INSERT
                XchgCode = request.XchgCode,
                DlrCode = request.DlrCode,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.GetCurrentUserId(),
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.INSERT,
                IsDel = (byte)0,
                Remarks = request.Remarks,
                RowsAffected = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Parameters for creating MstTrader: {@Parameters}", parameters);

            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudTrader",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully created MstTrader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
                return await GetMstTraderByIdAsync(request.XchgCode, request.DlrCode, cancellationToken);
            }
            else
            {
                _logger.LogWarning("No rows affected when creating MstTrader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
                throw new DomainException("Failed to create trader. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for creating MstTrader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error creating MstTrader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating MstTrader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
            throw new DomainException($"Failed to create trader: {ex.Message}");

        }
    }

    //update trader
    public async Task<MstTraderDto> UpdateMstTraderAsync(string xchgCode, string dlrCode, UpdateMstTraderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        //validate update request
        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new
            {
                Action = (byte)ActionTypeEnum.UPDATE, // UPDATE
                XchgCode = xchgCode,
                DlrCode = dlrCode,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.GetCurrentUserId(),
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.UPDATE,
                IsDel = (byte) DeleteStatusEnum.Active, // default to active 0
                Remarks = request.Remarks,
                RowsAffected = 0 // OUTPUT parameter
                };

            _logger.LogDebug("Parameters for updating MstTrader: {@Parameters}", parameters);
            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudTrader",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            if (rowsAffected > 0)
                {
                _logger.LogInformation("Successfully updated MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                return await GetMstTraderByIdAsync(xchgCode, dlrCode, cancellationToken);
            }
            else
            {
                _logger.LogWarning("No rows affected when updating MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                throw new DomainException("Failed to update trader. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for updating MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error updating MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"Failed to update trader: {ex.Message}");
        }
    }

    //delete trader
    public async Task<bool> DeleteMstTraderAsync(string xchgCode, string dlrCode, DeleteMstTraderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        //validate delete request
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new
            {
                Action = (byte)ActionTypeEnum.DELETE, // DELETE
                XchgCode = xchgCode,
                DlrCode = dlrCode,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.GetCurrentUserId(),
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.DELETE,
                IsDel = (byte) DeleteStatusEnum.Deleted, // mark as deleted 1
                Remarks = request.Remarks,
                RowsAffected = 0 // OUTPUT parameter
                };
            _logger.LogDebug("Parameters for deleting MstTrader: {@Parameters}", parameters);
            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudTrader",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            if (rowsAffected > 0)
                {
                _logger.LogInformation("Successfully deleted MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                return true;
            }
            else
            {
                _logger.LogWarning("No rows affected when deleting MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                throw new DomainException("Failed to delete trader. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for deleting MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error deleting MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"Failed to delete trader: {ex.Message}");
        }
    }
    #endregion

    #region Workflow Methods
    // Get unauthorized or denied traders for workflow
    public async Task<PagedResult<MstTraderDto>> GetTraderUnAuthDeniedListAsync(
    int pageNumber = 1,
    int pageSize = 10,
    string? sortDirection = null,
    string? searchTerm = null,
    string? xchgCode = null,
    int isAuth = 0,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting trader workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}",
            pageNumber, pageSize, isAuth);

        // Create request
        var request = new GetTraderWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            XchgCode = xchgCode,
            SortDirection = sortDirection,
            IsAuth = isAuth
        };
        
        //Validate the request
        await ValidateWorkflowListRequestAsync(request, cancellationToken);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = pageSize,
                ["SortColumn"] = "XchgCode",
                ["SortDirection"] = sortDirection ?? "ASC",
                ["XchgCode"] = xchgCode ?? (object)DBNull.Value,
                ["SearchTerm"] = searchTerm ?? (object)DBNull.Value,
                ["isAuth"] = isAuth,
                ["MakerId"] = _currentUserService.GetCurrentUserId(),
                ["TotalCount"] = 0
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetTraderListWF", pageNumber, pageSize, isAuth, _currentUserService.GetCurrentUserId());

            var result = await _repository.QueryPagedAsync<MstTraderDto>(
                sqlOrSp: "LB_SP_GetTraderListWF",
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
            _logger.LogError(ex, "Error getting trader workflow list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, XchgCode={XchgCode}, SearchTerm={SearchTerm}, IsAuth={IsAuth}",
                pageNumber, pageSize, xchgCode, searchTerm, isAuth);
            throw new DomainException($"Failed to retrieve trader workflow list: {ex.Message}");
        }
    }


    /// <summary>
    /// Authorize trader in workflow
    /// </summary>
    /// <param name="xchgCode">Exchange code</param>
    /// <param name="dlrCode">Dealer code</param>
    /// <param name="request">Authorization request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    public async Task<bool> AuthorizeTraderAsync(string xchgCode, string dlrCode, AuthorizeMstTraderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing MstTrader in workflow: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = request.ActionType,
                ["XchgCode"] = xchgCode,
                ["DlrCode"] = dlrCode,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["AuthID"] = _currentUserService.GetCurrentUserId(),
                ["IsAuth"] = request.IsAuth,
                ["ActionType"] = request.ActionType,
                ["Remarks"] = !string.IsNullOrEmpty(request.Remarks) ? request.Remarks : DBNull.Value,
                ["RowsAffected"] = 0 // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_AuthTrader with parameters: {@Parameters}", parameters);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthTrader",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully authorized MstTrader: {XchgCode}-{DlrCode}, RowsAffected: {RowsAffected}",
                    xchgCode, dlrCode, rowsAffected);
                return true;
            }
            else
            {
                _logger.LogWarning("No rows affected during authorization of MstTrader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                throw new DomainException($"Failed to authorize trader: No records were updated");
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
            _logger.LogError(ex, "Error authorizing MstTrader in workflow: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"Failed to authorize trader: {ex.Message}");
        }
    }



    #endregion

    #region Private Validation Methods
    private async Task ValidateCreateRequestAsync(CreateMstTraderRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateUpdateRequestAsync(UpdateMstTraderRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateDeleteRequestAsync(DeleteMstTraderRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateAuthorizeRequestAsync(AuthorizeMstTraderRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateWorkflowListRequestAsync(GetTraderWorkflowListRequest request, CancellationToken cancellationToken)
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