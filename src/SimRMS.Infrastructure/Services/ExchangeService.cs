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
/// Title:       Exchange Service
/// Author:      Raihan
/// Purpose:     This service provides methods for managing Exchange information
/// Creation:    01/Dec/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// Raihan           01/Dec/2025   Initial creation of ExchangeService
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

/// <summary>
/// Exchange service with business logic and validation
/// </summary>
public class ExchangeService : IExchangeService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateExchangeRequest> _createValidator;
    private readonly IValidator<UpdateExchangeRequest> _updateValidator;
    private readonly IValidator<DeleteExchangeRequest> _deleteValidator;
    private readonly IValidator<GetExchangeWorkflowListRequest> _workflowListValidator;
    private readonly IValidator<AuthorizeExchangeRequest> _authorizeValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ExchangeService> _logger;

    public ExchangeService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateExchangeRequest> createValidator,
        IValidator<UpdateExchangeRequest> updateValidator,
        IValidator<DeleteExchangeRequest> deleteValidator,
        IValidator<GetExchangeWorkflowListRequest> workflowListValidator,
        IValidator<AuthorizeExchangeRequest> authorizeValidator,
        ICurrentUserService currentUserService,
        ILogger<ExchangeService> logger)
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
    #region Exchanges List Operations
    public async Task<PagedResult<ExchangeDto>> GetExchangeListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? searchColumn = null, string? xchgCode = null, string? sortDirection = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving paged Exchange list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

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
                new LB_DALParam("SortColumn", "BrokerCode"),
	            new LB_DALParam("SortDirection", sortDirection ?? (object)DBNull.Value),

                // Filter Parameters
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
                new LB_DALParam("SearchText", searchText ?? (object)DBNull.Value),
                new LB_DALParam("SearchColumn", searchColumn ?? (object)DBNull.Value),

                // Control Parameter
                new LB_DALParam("IsAuth", (byte)AuthTypeEnum.Approve)
            };

			var result = await _repository.QueryPagedAsync<ExchangeDto>(
                sqlOrSp: "LB_SP_GetExchangeList",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Exchange list retrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Exchange list");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Exchange list");
            throw new DomainException($"The exchange list retrieval failed: {ex.Message}");
        }
    }
    public async Task<ExchangeDto> GetExchangeByIdAsync(string xchgCode, int xchgPrefix, string brokerCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(xchgCode))
            throw new ArgumentException("Exchange code cannot be null or empty", nameof(xchgCode));

        if (string.IsNullOrWhiteSpace(brokerCode))
            throw new ArgumentException("Broker code cannot be null or empty", nameof(brokerCode));

        _logger.LogInformation("Retrieving Exchange by ID: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
                new LB_DALParam("XchgPrefix", xchgPrefix),
                new LB_DALParam("BrokerCode", brokerCode ?? (object)DBNull.Value),
                new LB_DALParam("StatusCode", 0, ParameterDirection.Output),
	            new LB_DALParam("StatusMsg", string.Empty, ParameterDirection.Output)
            };

			var exchange = await _repository.QuerySingleAsync<ExchangeDto>(
                sqlOrSp: "LB_SP_GetExchange_ByCode",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            if (exchange != null)
            {
                _logger.LogInformation("Successfully retrieved exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            }
            else
            {
                _logger.LogWarning("The Exchange was not found: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            }

            return exchange;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Exchange retrieval: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Exchange by ID: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Exchange by ID: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new DomainException($"The exchange retrieval failed: {ex.Message}");
        }
    }
    #endregion

    #region Exchanges CRUD Operations

    public async Task<ExchangeDto> CreateExchangeAsync(CreateExchangeRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "CreateExchangeRequest cannot be null");
        }

        _logger.LogInformation("Creating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", request.XchgCode, request.XchgPrefix, request.BrokerCode);

        await ValidateCreateRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                new LB_DALParam("Action", (byte)ActionTypeEnum.INSERT),
	            new LB_DALParam("XchgCode", request.XchgCode ?? (object)DBNull.Value),
                new LB_DALParam("XchgPrefix", request.XchgPrefix),  
                new LB_DALParam("BrokerCode", request.BrokerCode ?? (object)DBNull.Value),
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Today),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.INSERT),
	            new LB_DALParam("IsDel", (byte)0),
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Parameters for creating Exchange: {@Parameters}", parameters);

            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudExchange",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully created Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", request.XchgCode, request.XchgPrefix, request.BrokerCode);
                return await GetExchangeByIdAsync(request.XchgCode, request.XchgPrefix, request.BrokerCode, cancellationToken);
            }
            else
            {
                _logger.LogWarning("No rows affected when creating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", request.XchgCode, request.XchgPrefix, request.BrokerCode);
                throw new DomainException("Failed to create exchange. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for creating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", request.XchgCode, request.XchgPrefix, request.BrokerCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while creating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", request.XchgCode, request.XchgPrefix, request.BrokerCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", request.XchgCode, request.XchgPrefix, request.BrokerCode);
            throw new DomainException($"The exchange creation failed: {ex.Message}");
        }
    }

    public async Task<ExchangeDto> UpdateExchangeAsync(string xchgCode, int xchgPrefix, string brokerCode, UpdateExchangeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                new LB_DALParam("Action", (byte)ActionTypeEnum.UPDATE),
	            new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
                new LB_DALParam("XchgPrefix", xchgPrefix),
                new LB_DALParam("BrokerCode", brokerCode ?? (object)DBNull.Value), 
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Today),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Parameters for updating Exchange: {@Parameters}", parameters);
            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudExchange",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully updated Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
                return await GetExchangeByIdAsync(xchgCode, xchgPrefix, brokerCode, cancellationToken);
            }
            else
            {
                _logger.LogWarning("No rows affected when updating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
                throw new DomainException("Failed to update exchange. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for updating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while updating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new DomainException($"The exchange update failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteExchangeAsync(string xchgCode, int xchgPrefix, string brokerCode, DeleteExchangeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                new LB_DALParam("Action", (byte)ActionTypeEnum.DELETE),
	            new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
                new LB_DALParam("XchgPrefix", xchgPrefix),
                new LB_DALParam("BrokerCode", brokerCode ?? (object)DBNull.Value),
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Today),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.DELETE),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Deleted),
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Parameters for deleting Exchange: {@Parameters}", parameters);
            var rowsAffected = await _repository.ExecuteAsync(
               sqlOrSp: "LB_SP_CrudExchange",
               parameters: parameters,
               isStoredProcedure: true,
               cancellationToken: cancellationToken);
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully deleted Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
                return true;
            }
            else
            {
                _logger.LogWarning("No rows affected when deleting Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
                throw new DomainException("Failed to delete exchange. No rows affected.");
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for deleting Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while deleting Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new DomainException($"The exchange deletion failed: {ex.Message}");
        }
    }
    #endregion

    #region Workflow Methods
    public async Task<PagedResult<ExchangeDto>> GetExchangeUnAuthDeniedListAsync(
    int pageNumber = 1,
    int pageSize = 10,
    string? sortDirection = null,
    string? searchText = null,
    string? searchColumn = null,
    string? xchgCode = null,
    int isAuth = 0,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting exchange workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}",
            pageNumber, pageSize, isAuth);

        var request = new GetExchangeWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchText = searchText,
            SearchColumn = searchColumn,
            XchgCode = xchgCode,
            SortDirection = sortDirection,
            IsAuth = isAuth
        };
        
        await ValidateWorkflowListRequestAsync(request, cancellationToken);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),
                new LB_DALParam("SortColumn", "XchgCode"),
	            new LB_DALParam("SortDirection", sortDirection ?? "ASC"),
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
	            new LB_DALParam("SearchText", searchText ?? (object)DBNull.Value),
	            new LB_DALParam("SearchColumn", searchColumn ?? (object)DBNull.Value),
                new LB_DALParam("isAuth", isAuth),
	            new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId())
            };

			_logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetExchangeListWF", pageNumber, pageSize, isAuth, _currentUserService.GetCurrentUserId());

            var result = await _repository.QueryPagedAsync<ExchangeDto>(
                sqlOrSp: "LB_SP_GetExchangeListWF",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exchange workflow list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, XchgCode={XchgCode}, searchText={searchText}, IsAuth={IsAuth}",
                pageNumber, pageSize, xchgCode, searchText, isAuth);
            throw new DomainException($"The exchange workflow list retrieval failed: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeExchangeAsync(string xchgCode, int xchgPrefix, string brokerCode, AuthorizeExchangeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing Exchange in workflow: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);

        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                new LB_DALParam("Action", request.ActionType),
	            new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
                new LB_DALParam("XchgPrefix", xchgPrefix),
                new LB_DALParam("BrokerCode", brokerCode ?? (object)DBNull.Value),
                new LB_DALParam("ActionType", request.ActionType),
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("AuthID", _currentUserService.GetCurrentUserId()),
	            new LB_DALParam("IsAuth", request.IsAuth),
                new LB_DALParam("Remarks", !string.IsNullOrEmpty(request.Remarks) ? (object)request.Remarks : DBNull.Value),
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Calling LB_SP_AuthExchange with parameters: {@Parameters}", parameters);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthExchange",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully authorized Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}, RowsAffected: {RowsAffected}",
                    xchgCode, xchgPrefix, brokerCode, rowsAffected);
                return true;
            }
            else
            {
                _logger.LogWarning("No rows affected during authorization of Exchange: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
                throw new DomainException($"The exchange authorization failed: No records were updated");
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
            _logger.LogError(ex, "An error occurred while authorizing Exchange in workflow: {XchgCode}-{XchgPrefix}-{BrokerCode}", xchgCode, xchgPrefix, brokerCode);
            throw new DomainException($"The exchange authorization failed: {ex.Message}");
        }
    }

    #endregion

    #region Private Validation Methods
    private async Task ValidateCreateRequestAsync(CreateExchangeRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Exchange creation validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateUpdateRequestAsync(UpdateExchangeRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Exchange update validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateDeleteRequestAsync(DeleteExchangeRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Exchange deletion validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeRequestAsync(AuthorizeExchangeRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Exchange authorization validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetExchangeWorkflowListRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Exchange workflow list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion
}
