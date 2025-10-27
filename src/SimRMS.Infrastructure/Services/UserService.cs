using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Common;
using SimRMS.Application.Exceptions;
using SimRMS.Infrastructure.Interfaces.Common;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;
using ValidationException = SimRMS.Application.Exceptions.ValidationException;
using LB.DAL.Core.Common;
using System.Data;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Service
/// Author:      Asif Zaman
/// Purpose:     User Service manages business logics and validations for User operations
/// Creation:    03/Sep/2025
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
/// User Service with business logic and validations
/// </summary>
public class UserService : IUserService
{
	private readonly IGenericRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IValidator<CreateUserRequest> _createValidator;
	private readonly IValidator<UpdateUserRequest> _updateValidator;
	private readonly IValidator<DeleteUserRequest> _deleteValidator;
	private readonly IValidator<GetUserWorkflowListRequest> _workflowListValidator;
	private readonly IValidator<AuthorizeUserRequest> _authorizeValidator;
	private readonly ICurrentUserService _currentUserService;
	private readonly ILogger<UserService> _logger;

	public UserService(
		IGenericRepository repository,
		IUnitOfWork unitOfWork,
		IValidator<CreateUserRequest> createValidator,
		IValidator<UpdateUserRequest> updateValidator,
		IValidator<DeleteUserRequest> deleteValidator,
		IValidator<GetUserWorkflowListRequest> workflowListValidator,
		IValidator<AuthorizeUserRequest> authorizeValidator,
		ICurrentUserService currentUserService,
		ILogger<UserService> logger)
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

	#region User List operations
	public async Task<PagedResult<UserDto>> GetUserListAsync(
		int pageNumber = 1,
		int pageSize = 10,
		string? searchText = null,
		string? usrStatus = null,
		string? dlrCode = null,
		string? coCode = null,
		string? coBrchCode = null,
		string? sortColumn = "UsrID",
		string? sortDirection = "ASC",
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Retrieving paged User list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

		if (pageNumber < 1) pageNumber = 1;
		if (pageSize < 1 || pageSize > 100) pageSize = 10;

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Pagination Parameters
                new LB_DALParam("PageNumber", pageNumber),
				new LB_DALParam("PageSize", pageSize),

                // Filter Parameters (assuming these can be null/optional)
                new LB_DALParam("SearchText", searchText ?? (object)DBNull.Value),
				new LB_DALParam("UsrStatus", usrStatus ?? (object)DBNull.Value),
				new LB_DALParam("DlrCode", dlrCode ?? (object)DBNull.Value),
				new LB_DALParam("CoCode", coCode ?? (object)DBNull.Value),
				new LB_DALParam("CoBrchCode", coBrchCode ?? (object)DBNull.Value),

                // Sorting Parameters (retaining null-coalescing logic from source)
                new LB_DALParam("SortColumn", sortColumn ?? "UsrID"),
				new LB_DALParam("SortDirection", sortDirection ?? "ASC")
			};

			var result = await _repository.QueryPagedAsync<UserDto>(
				sqlOrSp: "LB_SP_GetUserList",
				parameters: parameters,
				isStoredProcedure: true,
				cancellationToken: cancellationToken);

			return result;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for User listretrieval");
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while retrieving User list");
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while retrieving User list");
			throw new DomainException($"The user list retrieval failed: {ex.Message}");
		}
	}

	public async Task<UserDetailDto?> GetUserByIdAsync(string usrId, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(usrId))
			throw new ArgumentException("User ID c cannot be null or empty", nameof(usrId));

		_logger.LogInformation("Retrieving User by ID: {UsrId}", usrId);

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

			var user = await _repository.QuerySingleAsync<UserDetailDto>(
				sqlOrSp: "LB_SP_GetUserByUsrID",
				parameters: parameters,
				isStoredProcedure: true,
				cancellationToken: cancellationToken);

			if (user != null && !string.IsNullOrEmpty(user.UsrID))
			{
				_logger.LogInformation("Successfully retrieved user: {UsrId}", usrId);
				return user;
			}
			else
			{
				_logger.LogWarning("The User was not found: {UsrId}", usrId);
				return null;
			}
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Userretrieval: {UsrId}", usrId);
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while retrieving User by ID: {UsrId}", usrId);
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while retrieving User by ID: {UsrId}", usrId);
			throw new DomainException($"The user retrieval failed: {ex.Message}");
		}
	}
	#endregion

	#region User CRUD Operations
	public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
	{
		if (request == null)
		{
			throw new ArgumentNullException(nameof(request), "CreateUserRequest cannot be null");
		}

		_logger.LogInformation("Creating User: {UsrID}", request.UsrID);

		// Validate the request
		await ValidateCreateRequestAsync(request, cancellationToken);

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.INSERT),
				new LB_DALParam("UsrID", request.UsrID),

                // Data Parameters (User Details)
                new LB_DALParam("DlrCode", request.DlrCode ?? (object)DBNull.Value),
				new LB_DALParam("CoCode", request.CoCode ?? (object)DBNull.Value),
				new LB_DALParam("CoBrchCode", request.CoBrchCode ?? (object)DBNull.Value),
				new LB_DALParam("UsrName", request.UsrName),
				new LB_DALParam("UsrType", request.UsrType),
				new LB_DALParam("UsrGender", request.UsrGender ?? (object)DBNull.Value),
				new LB_DALParam("UsrDOB", request.UsrDOB ?? (object)DBNull.Value),
				new LB_DALParam("UsrEmail", request.UsrEmail ?? (object)DBNull.Value),
				new LB_DALParam("UsrAddr", request.UsrAddr ?? (object)DBNull.Value),
				new LB_DALParam("UsrMobile", request.UsrMobile ?? (object)DBNull.Value),
				new LB_DALParam("UsrStatus", request.UsrStatus),
				new LB_DALParam("UsrRegisterDate", request.UsrRegisterDate),
				new LB_DALParam("UsrExpiryDate", request.UsrExpiryDate ?? (object)DBNull.Value),
				new LB_DALParam("CSEDlrCode", request.CSEDlrCode ?? (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
				new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
				new LB_DALParam("ActionDt", DateTime.Now),
				new LB_DALParam("TransDt", DateTime.Today),
				new LB_DALParam("ActionType", (byte)ActionTypeEnum.INSERT),
				new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
				new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Authorization/Workflow Parameters (Initial State)
                new LB_DALParam("AuthId", (object)DBNull.Value), // (int?)null
                new LB_DALParam("AuthDt", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("AuthTransDt", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize), // Always start unauthorized
                new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
			};
			_logger.LogDebug("Parameters for creating User: {@Parameters}", parameters);

			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_CrudUsrInfo",
				parameters,
				cancellationToken: cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");

			if (rowsAffected > 0)
			{
				_logger.LogInformation("Successfully created User: {UsrID}", request.UsrID);
				return await GetUserByIdAsync(request.UsrID, cancellationToken) ?? throw new DomainException($"The Created user was not found: {request.UsrID}");
			}
			else
			{
				_logger.LogWarning("No rows affected when creating User: {UsrID}", request.UsrID);
				throw new DomainException("Failed to create user. No rows affected.");
			}
		}
		catch (ValidationException)
		{
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while creating User: {UsrID}", request.UsrID);
			throw new DomainException($"The user creation failed: {ex.Message}");
		}
	}

	public async Task<UserDto> UpdateUserAsync(string usrId, UpdateUserRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Updating User: {UsrID}", usrId);

		// Validate update request
		await ValidateUpdateRequestAsync(request, cancellationToken);

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE),
				new LB_DALParam("UsrID", usrId), // Key for update

                // Data Parameters (User Details)
                new LB_DALParam("DlrCode", request.DlrCode ?? (object)DBNull.Value),
				new LB_DALParam("CoCode", request.CoCode ?? (object)DBNull.Value),
				new LB_DALParam("CoBrchCode", request.CoBrchCode ?? (object)DBNull.Value),
				new LB_DALParam("UsrName", request.UsrName ?? (object)DBNull.Value),
				new LB_DALParam("UsrType", request.UsrType ?? (object)DBNull.Value),
				new LB_DALParam("UsrGender", request.UsrGender ?? (object)DBNull.Value),
				new LB_DALParam("UsrDOB", request.UsrDOB ?? (object)DBNull.Value),
				new LB_DALParam("UsrEmail", request.UsrEmail ?? (object)DBNull.Value),
				new LB_DALParam("UsrAddr", request.UsrAddr ?? (object)DBNull.Value),
				new LB_DALParam("UsrMobile", request.UsrMobile ?? (object)DBNull.Value),
				new LB_DALParam("UsrStatus", request.UsrStatus),
				new LB_DALParam("UsrRegisterDate", request.UsrRegisterDate ?? (object)DBNull.Value),
				new LB_DALParam("UsrExpiryDate", request.UsrExpiryDate ?? (object)DBNull.Value),
				new LB_DALParam("CSEDlrCode", request.CSEDlrCode ?? (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
				new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
				new LB_DALParam("ActionDt", DateTime.Now),
				new LB_DALParam("TransDt", DateTime.Today),
				new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),
				new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active), // Assuming the update does not change deletion status
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Authorization/Workflow Parameters (Resetting to unauthorized for workflow)
                new LB_DALParam("AuthId", (object)DBNull.Value), // (int?)null
                new LB_DALParam("AuthDt", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("AuthTransDt", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize),
				new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
			};

			_logger.LogDebug("Parameters for updating User: {@Parameters}", parameters);

			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_CrudUsrInfo",
				parameters,
				cancellationToken: cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");

			if (rowsAffected > 0)
			{
				_logger.LogInformation("Successfully updated User: {UsrID}", usrId);
				return await GetUserByIdAsync(usrId, cancellationToken) ?? throw new DomainException($"The Updated user was not found: {usrId}");
			}
			else
			{
				_logger.LogWarning("No rows affected when updating User: {UsrID}", usrId);
				throw new DomainException("Failed to update user. No rows affected.");
			}
		}
		catch (ValidationException)
		{
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while updating User: {UsrID}", usrId);
			throw new DomainException($"The user update failed: {ex.Message}");
		}
	}

	public async Task<bool> DeleteUserAsync(string usrId, DeleteUserRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Deleting User: {UsrID}", usrId);

		// Validate delete request
		await ValidateDeleteRequestAsync(request, cancellationToken);

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.DELETE),
				new LB_DALParam("UsrID", usrId), // The key for deletion

                // Data Parameters 
                new LB_DALParam("DlrCode", (object)DBNull.Value), // (string?)null
                new LB_DALParam("CoCode", (object)DBNull.Value), // (string?)null
                new LB_DALParam("CoBrchCode", (object)DBNull.Value), // (string?)null
                new LB_DALParam("UsrName", (object)DBNull.Value), // (string?)null
                new LB_DALParam("UsrType", (object)DBNull.Value), // (int?)null
                new LB_DALParam("UsrGender", (object)DBNull.Value), // (string?)null
                new LB_DALParam("UsrDOB", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("UsrEmail", (object)DBNull.Value), // (string?)null
                new LB_DALParam("UsrAddr", (object)DBNull.Value), // (string?)null
                new LB_DALParam("UsrMobile", (object)DBNull.Value), // (string?)null
                new LB_DALParam("UsrStatus", (object)DBNull.Value), // (string?)null
                new LB_DALParam("UsrRegisterDate", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("UsrExpiryDate", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("CSEDlrCode", (object)DBNull.Value), // (string?)null

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
				new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId()),
				new LB_DALParam("ActionDt", DateTime.Now),
				new LB_DALParam("TransDt", DateTime.Today),
				new LB_DALParam("ActionType", (byte)ActionTypeEnum.DELETE),
				new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Deleted), // Marking the record as deleted

                // Authorization/Workflow Parameters
                new LB_DALParam("AuthId", (object)DBNull.Value), // (int?)null
                new LB_DALParam("AuthDt", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("AuthTransDt", (object)DBNull.Value), // (DateTime?)null
                new LB_DALParam("IsAuth", (object)DBNull.Value), // (byte?)null
                new LB_DALParam("AuthLevel", (object)DBNull.Value), // (byte?)null

                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? "Soft deleted"), // Using the provided null-coalescing logic

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
			};

			_logger.LogDebug("Parameters for deleting User: {@Parameters}", parameters);

			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_CrudUsrInfo",
				parameters,
				cancellationToken: cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");

			if (rowsAffected > 0)
			{
				_logger.LogInformation("Successfully deleted User: {UsrID}", usrId);
				return true;
			}
			else
			{
				_logger.LogWarning("No rows affected when deleting User: {UsrID}", usrId);
				throw new DomainException("Failed to delete user. No rows affected.");
			}
		}
		catch (ValidationException)
		{
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while deleting User: {UsrID}", usrId);
			throw new DomainException($"The user deletion failed: {ex.Message}");
		}
	}

	public async Task<bool> UserExistsAsync(string usrId, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("Checking if User exists with UsrID: {UsrID}", usrId);

		try
		{
			var sql = @"
                SELECT ui.UsrID, ui.UsrName
                FROM UsrInfo ui
                WHERE ui.UsrID = @usrID";

			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
				new LB_DALParam("UsrID", usrId),
			};


			var result = await _repository.ExecuteScalarAsync<string>(sql, parameters, false, cancellationToken);
			return !string.IsNullOrEmpty(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking User existence: {UsrID}", usrId);
			throw new DomainException($"Failed to check user existence: {ex.Message}");
		}
	}
	#endregion

	#region Workflow Methods
	/// <summary>
	/// Get unauthorized or denied users for workflow
	/// </summary>
	public async Task<PagedResult<UserDto>> GetUserUnAuthDeniedListAsync(
		int pageNumber = 1,
		int pageSize = 10,
		string? sortColumn = "UsrID",
		string? sortDirection = "ASC",
		string? searchText = null,
		string? usrStatus = null,
		string? dlrCode = null,
		string? coCode = null,
		string? coBrchCode = null,
		int isAuth = 0,
		CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Getting user workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}",
			pageNumber, pageSize, isAuth);

		// Create request for validation
		var request = new GetUserWorkflowListRequest
		{
			PageNumber = pageNumber,
			PageSize = pageSize,
			SearchText = searchText,
			UsrStatus = usrStatus,
			DlrCode = dlrCode,
			CoCode = coCode,
			CoBrchCode = coBrchCode,
			SortColumn = sortColumn,
			SortDirection = sortDirection,
			IsAuth = isAuth
		};

		// Validate the request
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

				// Filter Parameters 
				new LB_DALParam("SearchText", searchText ?? (object)DBNull.Value),
				new LB_DALParam("UsrStatus", usrStatus ?? (object)DBNull.Value),
				new LB_DALParam("DlrCode", dlrCode ?? (object)DBNull.Value),
				new LB_DALParam("CoCode", coCode ?? (object)DBNull.Value),
				new LB_DALParam("CoBrchCode", coBrchCode ?? (object)DBNull.Value),

				// Sorting Parameters
				new LB_DALParam("SortColumn", sortColumn ?? "UsrID"),
				new LB_DALParam("SortDirection", sortDirection ?? "ASC"),

				// Control and Audit Parameters
				new LB_DALParam("isAuth", isAuth),
				new LB_DALParam("MakerId", _currentUserService.GetCurrentUserId())
			};

			_logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
				"LB_SP_GetUserListWF", pageNumber, pageSize, isAuth, _currentUserService.GetCurrentUserId());

			var result = await _repository.QueryPagedAsync<UserDto>(
				sqlOrSp: "LB_SP_GetUserListWF",
				parameters: parameters,
				isStoredProcedure: true,
				cancellationToken: cancellationToken);

			_logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
				result.TotalCount, result.Data.Count());

			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting user workflow list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, UsrStatus={UsrStatus}, SearchText={SearchText}, IsAuth={IsAuth}",
				pageNumber, pageSize, usrStatus, searchText, isAuth);
			throw new DomainException($"The user workflow list retrieval failed: {ex.Message}");
		}
	}


	/// <summary>
	/// Authorize user in workflow
	/// </summary>
	/// <param name="usrId">User ID</param>
	/// <param name="request">Authorization request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Authorization result</returns>
	public async Task<bool> AuthorizeUserAsync(string usrId, AuthorizeUserRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Authorizing User in workflow: {UsrId}", usrId);

		// Validate request
		await ValidateAuthorizeRequestAsync(request, cancellationToken);

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
				// Action and Key Parameters
				new LB_DALParam("Action", request.ActionType),
				new LB_DALParam("UsrID", usrId ?? (object)DBNull.Value), // Added null check for safety
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

			_logger.LogDebug("Calling LB_SP_AuthUsrInfo with parameters: {@Parameters}", parameters);

			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_AuthUsrInfo",
				parameters,
				cancellationToken: cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");

			if (rowsAffected > 0)
			{
				_logger.LogInformation("Successfully authorized User: {UsrId}, RowsAffected: {RowsAffected}",
					usrId, rowsAffected);
				return true;
			}
			else
			{
				_logger.LogWarning("No rows affected during authorization of User: {UsrId}", usrId);
				throw new DomainException($"The user authorization failed: No records were updated");
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
			_logger.LogError(ex, "An error occurred while authorizing User in workflow: {UsrId}", usrId);
			throw new DomainException($"The user authorization failed: {ex.Message}");
		}
	}

	#endregion

	#region Private Validation Methods
	private async Task ValidateCreateRequestAsync(CreateUserRequest request, CancellationToken cancellationToken)
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

			throw new ValidationException("The User creation validation failed") { ValidationErrors = errors };
		}
	}

	private async Task ValidateUpdateRequestAsync(UpdateUserRequest request, CancellationToken cancellationToken)
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

			throw new ValidationException("The User update validation failed") { ValidationErrors = errors };
		}
	}

	private async Task ValidateDeleteRequestAsync(DeleteUserRequest request, CancellationToken cancellationToken)
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

			throw new ValidationException("The User deletion validation failed") { ValidationErrors = errors };
		}
	}


	private async Task ValidateWorkflowListRequestAsync(GetUserWorkflowListRequest request, CancellationToken cancellationToken)
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

			throw new ValidationException("The User workflow list validation failed") { ValidationErrors = errors };
		}
	}

	private async Task ValidateAuthorizeRequestAsync(AuthorizeUserRequest request, CancellationToken cancellationToken)
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

			throw new ValidationException("The User authorization validation failed") { ValidationErrors = errors };
		}
	}
	#endregion

}