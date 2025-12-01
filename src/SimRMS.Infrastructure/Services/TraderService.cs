using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Validators;
using SimRMS.Application.Common;
using SimRMS.Application.Exceptions;
using SimRMS.Infrastructure.Interfaces.Common;
using SimRMS.Infrastructure.Common;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;
using System.Threading.Tasks;
using ValidationException = SimRMS.Application.Exceptions.ValidationException;
using LB.DAL.Core.Common;
using System.Data;

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
    public async Task<PagedResult<MstTraderDto>> GetMstTraderListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? searchColumn = null, string? xchgCode = null, string? sortDirection = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving paged Trader list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),

                // Sorting Parameters
                new LB_DALParam("SortColumn", "DlrCode"),
	            new LB_DALParam("SortDirection", sortDirection ?? (object)DBNull.Value), // Added null check for safety

                // Filter Parameters
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("SearchText", searchText ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("SearchColumn", searchColumn ?? (object)DBNull.Value), // Added null check for safety

                // Control Parameter
                new LB_DALParam("IsAuth", (byte)AuthTypeEnum.Approve) // Authorized records
            };

			var result = await _repository.QueryPagedAsync<MstTraderDto>(
                sqlOrSp: "LB_SP_GetTraderList",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Trader listretrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Trader list");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Trader list");
            throw new DomainException($"The trader list retrieval failed: {ex.Message}");
        }
    }
    public async Task<MstTraderDto> GetMstTraderByIdAsync(string xchgCode, string dlrCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(xchgCode))
            throw new ArgumentException("Exchange code cannot be null or empty", nameof(xchgCode));

        if (string.IsNullOrWhiteSpace(dlrCode))
            throw new ArgumentException("Dealer code cannot be null or empty", nameof(dlrCode));

        _logger.LogInformation("Retrieving Trader by ID: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        try
        {
			// Create parameters matching the stored procedure signature
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("DlrCode", dlrCode ?? (object)DBNull.Value),   // Added null check for safety

                // Output Parameters
                new LB_DALParam("StatusCode", 0, ParameterDirection.Output),
	            new LB_DALParam("StatusMsg", string.Empty, ParameterDirection.Output)
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
                _logger.LogWarning("The Trader was not found: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            }

            return trader; // This will be null if not found
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Trader retrieval: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Trader by ID: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Trader by ID: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"The trader retrieval failed: {ex.Message}");
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

        _logger.LogInformation("Creating Trader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);

        // Validate the request using FluentValidation
        await ValidateCreateRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (byte)ActionTypeEnum.INSERT),
	            new LB_DALParam("XchgCode", request.XchgCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("DlrCode", request.DlrCode ?? (object)DBNull.Value),   // Added null check for safety
                new LB_DALParam("XchgPrefix", request.XchgPrefix),  
                new LB_DALParam("BrokerCode", request.BrokerCode ?? (object)DBNull.Value),   // Added null check for safety

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Today),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.INSERT),
	            new LB_DALParam("IsDel", (byte)0), // Assuming 0 is DeleteStatusEnum.Active

                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Parameters for creating Trader: {@Parameters}", parameters);

            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudTrader",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully created Trader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
                return await GetMstTraderByIdAsync(request.XchgCode, request.DlrCode, cancellationToken);
            }
            else
            {
                _logger.LogWarning("No rows affected when creating Trader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
                throw new DomainException("Failed to create trader. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for creating Trader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while creating Trader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating Trader: {XchgCode}-{DlrCode}", request.XchgCode, request.DlrCode);
            throw new DomainException($"The trader creation failed: {ex.Message}");

        }
    }

    //update trader
    public async Task<MstTraderDto> UpdateMstTraderAsync(string xchgCode, string dlrCode, UpdateMstTraderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        //validate update request
        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (byte)ActionTypeEnum.UPDATE),
	            new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("DlrCode", dlrCode ?? (object)DBNull.Value),   // Added null check for safety
                new LB_DALParam("XchgPrefix", request.XchgPrefix),
                new LB_DALParam("BrokerCode", request.BrokerCode ?? (object)DBNull.Value), 
                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Today),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active), // Assuming DeleteStatusEnum.Active is 0

                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Parameters for updating Trader: {@Parameters}", parameters);
            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudTrader",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            if (rowsAffected > 0)
                {
                _logger.LogInformation("Successfully updated Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                return await GetMstTraderByIdAsync(xchgCode, dlrCode, cancellationToken);
            }
            else
            {
                _logger.LogWarning("No rows affected when updating Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                throw new DomainException("Failed to update trader. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for updating Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while updating Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"The trader update failed: {ex.Message}");
        }
    }

    //delete trader
    public async Task<bool> DeleteMstTraderAsync(string xchgCode, string dlrCode, DeleteMstTraderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        //validate delete request
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (byte)ActionTypeEnum.DELETE),
	            new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("DlrCode", dlrCode ?? (object)DBNull.Value),   // Added null check for safety

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Today),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.DELETE),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Deleted), // Marking the record as deleted

                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Parameters for deleting Trader: {@Parameters}", parameters);
            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudTrader",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            if (rowsAffected > 0)
                {
                _logger.LogInformation("Successfully deleted Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                return true;
            }
            else
            {
                _logger.LogWarning("No rows affected when deleting Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                throw new DomainException("Failed to delete trader. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for deleting Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while deleting Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"The trader deletion failed: {ex.Message}");
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
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),

                // Sorting Parameters
                new LB_DALParam("SortColumn", "XchgCode"),
	            new LB_DALParam("SortDirection", sortDirection ?? "ASC"), // Retaining null-coalescing from source

                // Filter Parameters (Already includes DBNull handling from the source Dictionary)
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
	            new LB_DALParam("SearchTerm", searchTerm ?? (object)DBNull.Value),

                // Control and Audit Parameters
                new LB_DALParam("isAuth", isAuth),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId())
            };

			_logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetTraderListWF", pageNumber, pageSize, isAuth, _currentUserService.GetCurrentUserId());

            var result = await _repository.QueryPagedAsync<MstTraderDto>(
                sqlOrSp: "LB_SP_GetTraderListWF",
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
            throw new DomainException($"The trader workflow list retrieval failed: {ex.Message}");
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
        _logger.LogInformation("Authorizing Trader in workflow: {XchgCode}-{DlrCode}", xchgCode, dlrCode);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Action and Key Parameters
                new LB_DALParam("Action", request.ActionType),
	            new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("DlrCode", dlrCode ?? (object)DBNull.Value),   // Added null check for safety
                new LB_DALParam("ActionType", request.ActionType),

                // Authorization/Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("AuthID", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("IsAuth", request.IsAuth),

                // Remarks (Handling empty string/null logic from source)
                new LB_DALParam("Remarks", !string.IsNullOrEmpty(request.Remarks) ? (object)request.Remarks : DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Calling LB_SP_AuthTrader with parameters: {@Parameters}", parameters);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthTrader",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully authorized Trader: {XchgCode}-{DlrCode}, RowsAffected: {RowsAffected}",
                    xchgCode, dlrCode, rowsAffected);
                return true;
            }
            else
            {
                _logger.LogWarning("No rows affected during authorization of Trader: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
                throw new DomainException($"The trader authorization failed: No records were updated");
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
            _logger.LogError(ex, "An error occurred while authorizing Trader in workflow: {XchgCode}-{DlrCode}", xchgCode, dlrCode);
            throw new DomainException($"The trader authorization failed: {ex.Message}");
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

            throw new ValidationException("The Trader creation validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("The Trader update validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("The Trader deletion validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("The Trader authorization validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("The Trader workflow list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion
}