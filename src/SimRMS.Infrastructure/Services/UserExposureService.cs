using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Infrastructure.Interfaces.Common;
using SimRMS.Shared.Models;
using SimRMS.Application.Exceptions;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.Application.Common;
using ValidationException = SimRMS.Application.Exceptions.ValidationException;
using LB.DAL.Core.Common;
using System.Data;

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
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),

                // Filter Parameter (assuming it can be null/optional)
                new LB_DALParam("SearchText", searchTerm ?? (object)DBNull.Value),

                // Sorting Parameters
                new LB_DALParam("SortColumn", "UsrID"),
	            new LB_DALParam("SortDirection", "ASC")
            };

			_logger.LogDebug("Calling LB_SP_GetUserExpsList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);

            var result = await _repository.QueryPagedAsync<UserExposureDto>(
                sqlOrSp: "LB_SP_GetUserExpsList",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for User Exposure list retrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving User Exposure list");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving User Exposure list");
            throw new DomainException($"The user exposure list retrieval failed: {ex.Message}");
        }
    }

    public async Task<UserExposureDto?> GetUserExposureByIdAsync(string usrId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting UserExposure by ID: {UsrId}", usrId);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameter
                new LB_DALParam("UsrID", usrId ?? (object)DBNull.Value), // Added null check for safety

                // Output Parameters
                new LB_DALParam("StatusCode", 0, ParameterDirection.Output),
	            new LB_DALParam("StatusMsg", string.Empty, ParameterDirection.Output)
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
            _logger.LogError(ex, "Invalid arguments for User Exposure retrieval: {UsrId}", usrId);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving User Exposure by ID: {UsrId}", usrId);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving User Exposure by ID: {UsrId}", usrId);
            throw new DomainException($"The user exposure retrieval failed: {ex.Message}");
        }
    }

    public async Task<UserExposureDto> CreateUserExposureAsync(CreateUserExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating User Exposure for user: {UsrId}", request.UsrId);

        // Validate the request
        await ValidateCreateRequestAsync(request, cancellationToken);

        // Apply business logic: buy/sell vs total exposure
        ApplyBusinessLogicRules(request);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (byte)ActionTypeEnum.INSERT),
	            new LB_DALParam("UsrID", request.UsrId),

                // Data Parameters (Expense Limits)
                new LB_DALParam("UsrExpsCheckBuy", request.UsrExpsCheckBuy),
	            new LB_DALParam("UsrExpsBuyAmt", request.UsrExpsBuyAmt),
                new LB_DALParam("UsrExpsCheckSell", request.UsrExpsCheckSell),
	            new LB_DALParam("UsrExpsSellAmt", request.UsrExpsSellAmt), 
                new LB_DALParam("UsrExpsCheckTotal", request.UsrExpsCheckTotal),
	            new LB_DALParam("UsrExpsTotalAmt", request.UsrExpsTotalAmt),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Now.Date),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.INSERT),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Authorization/Workflow Parameters (Initial State)
                new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize), // Always start unauthorized
                new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
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
            _logger.LogError(ex, "An unexpected error occurred while creating User Exposure for user: {UsrId}", request.UsrId);
            throw new DomainException($"The user exposure creation failed: {ex.Message}");
        }
    }

    public async Task<UserExposureDto> UpdateUserExposureAsync(string usrId, UpdateUserExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating User Exposure: {UsrId}", usrId);

        // Use existing validation method
        await ValidateUpdateRequestAsync(request, cancellationToken);

        // Apply business logic: buy/sell vs total exposure
        ApplyBusinessLogicRules(request);

        try
        {
            var existingExposure = await GetUserExposureByIdAsync(usrId, cancellationToken);
            if (existingExposure == null)
            {
                throw new DomainException($"The User exposure was not found: {usrId}");
            }

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (byte)ActionTypeEnum.UPDATE),
	            new LB_DALParam("UsrID", usrId),

                // Data Parameters (Pending Expense Limits)
                new LB_DALParam("PendingUsrExpsCheckBuy", request.PendingUsrExpsCheckBuy ?? (object)DBNull.Value),
	            new LB_DALParam("PendingUsrExpsBuyAmt", request.PendingUsrExpsBuyAmt ?? (object)DBNull.Value),
	            new LB_DALParam("PendingUsrExpsCheckSell", request.PendingUsrExpsCheckSell ?? (object)DBNull.Value),
	            new LB_DALParam("PendingUsrExpsSellAmt", request.PendingUsrExpsSellAmt ?? (object)DBNull.Value),
	            new LB_DALParam("PendingUsrExpsCheckTotal", request.PendingUsrExpsCheckTotal ?? (object)DBNull.Value),
	            new LB_DALParam("PendingUsrExpsTotalAmt", request.PendingUsrExpsTotalAmt ?? (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Now.Date),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Authorization/Workflow Parameters (Resetting for a new workflow)
                new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize), // Resetting status
                new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
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
                throw new DomainException("The user exposure update failed because no rows were affected");
            }

            var updatedExposure = await GetUserExposureByIdAsync(usrId, cancellationToken);

            if (updatedExposure == null)
            {
                throw new DomainException($"The Updated user exposure was not found: {usrId}");
            }

            _logger.LogInformation("Successfully updated User Exposure: {UsrId}", usrId);
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
            _logger.LogError(ex, "An unexpected error occurred while updating User Exposure: {UsrId}", usrId);
            throw new DomainException($"The user exposure update failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteUserExposureAsync(string usrId, DeleteUserExposureRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting User Exposure: {UsrId}", usrId);

        // Use existing validation method
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var existingExposure = await GetUserExposureByIdAsync(usrId, cancellationToken);
            if (existingExposure == null)
            {
                throw new DomainException($"The User exposure was not found: {usrId}");
            }

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (byte)ActionTypeEnum.DELETE),
	            new LB_DALParam("UsrID", usrId),

                // Data Parameters (All explicitly set to DBNull for DELETE)
                new LB_DALParam("UsrExpsCheckBuy", (object)DBNull.Value),
	            new LB_DALParam("UsrExpsBuyAmt", (object)DBNull.Value),
	            new LB_DALParam("UsrExpsCheckSell", (object)DBNull.Value),
	            new LB_DALParam("UsrExpsSellAmt", (object)DBNull.Value),
	            new LB_DALParam("UsrExpsCheckTotal", (object)DBNull.Value),
	            new LB_DALParam("UsrExpsTotalAmt", (object)DBNull.Value),
	            new LB_DALParam("UsrExpsWithShrLimit", (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", DateTime.Now),
	            new LB_DALParam("TransDt", DateTime.Now.Date),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.DELETE),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Authorization/Workflow Parameters (Explicitly DBNull for DELETE)
                new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (object)DBNull.Value),
	            new LB_DALParam("AuthLevel", (object)DBNull.Value),
	            new LB_DALParam("IsDel", (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
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
                _logger.LogInformation("Successfully deleted User Exposure: {UsrId}", usrId);
            }
            else
            {
                _logger.LogWarning("The delete operation failed because no rows were affected: {UsrId}", usrId);
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
            _logger.LogError(ex, "An unexpected error occurred while deleting User Exposure: {UsrId}", usrId);
            throw new DomainException($"The user exposure deletion failed: {ex.Message}");
        }
    }

    public async Task<bool> UserExposureExistsAsync(string usrId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if User Exposure exists: {UsrId}", usrId);

        try
        {
            var exposure = await GetUserExposureByIdAsync(usrId, cancellationToken);
            return exposure != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking User Exposure existence: {UsrId}", usrId);
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

        _logger.LogInformation("Getting {authAction} User Exposure list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),

                // Filter and Sorting Parameters
                new LB_DALParam("SearchText", searchTerm ?? (object)DBNull.Value),
	            new LB_DALParam("SortColumn", "UsrID"),
	            new LB_DALParam("SortDirection", "ASC"),

                // Control and Audit Parameters
                new LB_DALParam("isAuth", (byte)isAuth),
	            new LB_DALParam("MakerId", _currentUserService.UserId)
            };

			_logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetUserExpsListWF", pageNumber, pageSize, isAuth, _currentUserService.UserId);

            var result = await _repository.QueryPagedAsync<UserExposureDto>(
                sqlOrSp: "LB_SP_GetUserExpsListWF",
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

        _logger.LogInformation("Authorizing {authAction} User Exposure in workflow: {UsrId}", usrId, authAction);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var ipAddress = _currentUserService.GetClientIPAddress();

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Action and Key Parameters
                new LB_DALParam("Action", request.ActionType),
	            new LB_DALParam("UsrID", usrId ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("ActionType", request.ActionType), 

                // Authorization/Audit Parameters
                new LB_DALParam("IPAddress", ipAddress ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("AuthID", _currentUserService.UserId),
	            new LB_DALParam("IsAuth", request.IsAuth),

                // Remarks (Handling empty string/null logic from source)
                new LB_DALParam("Remarks", !string.IsNullOrEmpty(request.Remarks) ? (object)request.Remarks : (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthUsrExpsInfo",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized User Exposure: {UsrId}, RowsAffected: {RowsAffected}",
                    usrId, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("No rows affected during {authAction} authorization of User Exposure: {UsrId}", usrId, authAction);
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
            _logger.LogError(ex, "Error {authAction} authorizing User Exposure in workflow: {UsrId}", usrId, authAction);
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

            throw new ValidationException("Create User Exposure validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Update User Exposure validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Delete User Exposure validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Authorization User Exposure validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Workflow User Exposure list validation failed") { ValidationErrors = errors };
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
        // If total is checked or has value but buy/sell are also enabled, throw exception
        if (request.UsrExpsCheckTotal || request.UsrExpsTotalAmt > 0)
        {
            if (request.UsrExpsCheckBuy || request.UsrExpsBuyAmt > 0 || request.UsrExpsCheckSell || request.UsrExpsSellAmt > 0)
            {
                throw new DomainException("To enable Total exposure (check or set amount), please first disable Buy and Sell exposure options (uncheck and set amounts to 0).");
            }
            
            // Previously this was setting values automatically - now commented out
            // request.UsrExpsCheckBuy = false;
            // request.UsrExpsBuyAmt = 0.00m;
            // request.UsrExpsCheckSell = false;
            // request.UsrExpsSellAmt = 0.00m;
        }
        
        // If buy or sell are checked or have values but total is also enabled, throw exception
        if (request.UsrExpsCheckBuy || request.UsrExpsBuyAmt > 0 || request.UsrExpsCheckSell || request.UsrExpsSellAmt > 0)
        {
            if (request.UsrExpsCheckTotal || request.UsrExpsTotalAmt > 0)
            {
                throw new DomainException("To enable Buy/Sell exposure (check or set amounts), please first disable Total exposure option (uncheck and set amount to 0).");
            }
            
            // Previously this was setting values automatically - now commented out
            // request.UsrExpsCheckTotal = false;
            // request.UsrExpsTotalAmt = 0.00m;
        }
    }

    /// <summary>
    /// Applies business logic rules for buy/sell vs total exposure for updates
    /// Rule: If buy and sell are checked, then total should be unchecked and amount 0
    ///       If total is checked, then buy and sell should be unchecked and amounts 0
    /// </summary>
    private static void ApplyBusinessLogicRules(UpdateUserExposureRequest request)
    {
        // If total is being checked or amount set but buy/sell are still enabled, throw exception
        if ((request.PendingUsrExpsCheckTotal.HasValue && request.PendingUsrExpsCheckTotal.Value) || 
            (request.PendingUsrExpsTotalAmt.HasValue && request.PendingUsrExpsTotalAmt.Value > 0))
        {
            if ((request.PendingUsrExpsCheckBuy.HasValue && request.PendingUsrExpsCheckBuy.Value) || 
                (request.PendingUsrExpsBuyAmt.HasValue && request.PendingUsrExpsBuyAmt.Value > 0) ||
                (request.PendingUsrExpsCheckSell.HasValue && request.PendingUsrExpsCheckSell.Value) || 
                (request.PendingUsrExpsSellAmt.HasValue && request.PendingUsrExpsSellAmt.Value > 0))
            {
                throw new DomainException("To enable Total exposure (check or set amount), please first disable Buy and Sell exposure options (uncheck and set amounts to 0).");
            }
            
            // Previously this was setting values automatically - now commented out
            // request.UsrExpsCheckBuy = false;
            // request.UsrExpsBuyAmt = 0.00m;
            // request.UsrExpsCheckSell = false;
            // request.UsrExpsSellAmt = 0.00m;
        }
        
        // If buy or sell are being checked or amounts set but total is still enabled, throw exception
        if ((request.PendingUsrExpsCheckBuy.HasValue && request.PendingUsrExpsCheckBuy.Value) || 
            (request.PendingUsrExpsBuyAmt.HasValue && request.PendingUsrExpsBuyAmt.Value > 0) ||
            (request.PendingUsrExpsCheckSell.HasValue && request.PendingUsrExpsCheckSell.Value) || 
            (request.PendingUsrExpsSellAmt.HasValue && request.PendingUsrExpsSellAmt.Value > 0))
        {
            if ((request.PendingUsrExpsCheckTotal.HasValue && request.PendingUsrExpsCheckTotal.Value) || 
                (request.PendingUsrExpsTotalAmt.HasValue && request.PendingUsrExpsTotalAmt.Value > 0))
            {
                throw new DomainException("To enable Buy/Sell exposure (check or set amounts), please first disable Total exposure option (uncheck and set amount to 0).");
            }
            
            // Previously this was setting values automatically - now commented out
            // request.UsrExpsCheckTotal = false;
            // request.UsrExpsTotalAmt = 0.00m;
        }
    }

    #endregion
}