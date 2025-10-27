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
using Azure.Core;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Client information
/// Creation:    16/Sep/2025
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

public class ClientService : IClientService
{
	private readonly IGenericRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IValidator<CreateClientRequest> _createValidator;
	private readonly IValidator<UpdateClientRequest> _updateValidator;
	private readonly IValidator<DeleteClientRequest> _deleteValidator;
	private readonly IValidator<AuthorizeClientRequest> _authorizeValidator;
	private readonly IValidator<GetClientWorkflowListRequest> _workflowListValidator;
	private readonly ICurrentUserService _currentUserService;
	private readonly ILogger<ClientService> _logger;

	public ClientService(
		IGenericRepository repository,
		IUnitOfWork unitOfWork,
		IValidator<CreateClientRequest> createValidator,
		IValidator<UpdateClientRequest> updateValidator,
		IValidator<DeleteClientRequest> deleteValidator,
		IValidator<AuthorizeClientRequest> authorizeValidator,
		IValidator<GetClientWorkflowListRequest> workflowListValidator,
		ICurrentUserService currentUserService,
		ILogger<ClientService> logger)
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

	public async Task<PagedResult<ClientDto>> GetClientListAsync(int pageNumber = 1, int pageSize = 10,
		string? searchTerm = null, string? gcif = null, string? clntName = null, string? clntCode = null, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Retrieving paged Client list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

		// Validate and sanitize inputs
		if (pageNumber < 1) pageNumber = 1;
		if (pageSize < 1 || pageSize > 100) pageSize = 10;

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
				new LB_DALParam("PageNumber", pageNumber),
				new LB_DALParam("PageSize", pageSize),
				new LB_DALParam("GCIF", gcif ?? (object)DBNull.Value),
				new LB_DALParam("ClntName", clntName ?? (object)DBNull.Value),
				new LB_DALParam("ClntCode", clntCode ?? (object)DBNull.Value),
				new LB_DALParam("SortBy", "GCIF"),
				new LB_DALParam("SortOrder", "ASC")
			};


			_logger.LogDebug("Calling LB_SP_GetClientMstAccList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, GCIF={GCIF}, ClntName={ClntName}, ClntCode={ClntCode}",
				pageNumber, pageSize, gcif, clntName, clntCode);

			var result = await _repository.QueryPagedAsync<ClientDto>(
				sqlOrSp: "LB_SP_GetClientMstAccList",
				parameters: parameters,
				isStoredProcedure: true,
				cancellationToken: cancellationToken);

			_logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
				result.TotalCount, result.Data.Count());

			return result;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Client listretrieval");
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while retrieving Client list");
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while retrieving Client list");
			throw new DomainException($"The client list retrieval failed: {ex.Message}");
		}
	}

	public async Task<ClientDto?> GetClientByIdAsync(string gcif, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Retrieving Client by GCIF: {GCIF}", gcif);

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
				new LB_DALParam("GCIF", gcif),

                //  Output parameters
                new LB_DALParam("StatusCode", 0, ParameterDirection.Output),
				new LB_DALParam("StatusMsg", string.Empty, ParameterDirection.Output)
			};


			var result = await _repository.QuerySingleAsync<ClientDto>(
				sqlOrSp: "LB_SP_GetClientMstAccByClientID",
				parameters: parameters,
				isStoredProcedure: true,
				cancellationToken: cancellationToken);

			_logger.LogDebug("Retrieved client: {Found}", result != null ? "Found" : "Not Found");
			return result;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Client retrieval: {GCIF}", gcif);
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while retrieving Client: {GCIF}", gcif);
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while retrieving Client by GCIF: {GCIF}", gcif);
			throw new DomainException($"The client retrieval failed: {ex.Message}");
		}
	}

	public async Task<ClientDto> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Creating Client with ClntCode: {ClntCode}", request.ClntCode);

		// Validate the request
		var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			_logger.LogWarning("Validation failed for creating Client: {Errors}", string.Join(", ", validationResult.Errors));
			throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
		}

		try
		{
			await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Action
                new LB_DALParam("Action", (int)ActionTypeEnum.INSERT),

                // ClntMaster parameters
                new LB_DALParam("GCIF", (string?)null), // SP will auto-generate
                new LB_DALParam("ClntCode", request.ClntCode),
				new LB_DALParam("ClntName", request.ClntName),
				new LB_DALParam("ClntNICNo", request.ClntNICNo),
				new LB_DALParam("ClntAddr", request.ClntAddr),
				new LB_DALParam("ClntPhone", request.ClntPhone),
				new LB_DALParam("ClntMobile", request.ClntMobile),
				new LB_DALParam("Gender", request.Gender),
				new LB_DALParam("Nationality", request.Nationality),
				new LB_DALParam("ClntOffice", request.ClntOffice),
				new LB_DALParam("ClntFax", request.ClntFax),
				new LB_DALParam("ClntEmail", request.ClntEmail),
				new LB_DALParam("CountryCode", request.CountryCode),

                // ClntAcct parameters
                new LB_DALParam("CoBrchCode", request.CoBrchCode),
				new LB_DALParam("ClntStat", request.ClntStat),
				new LB_DALParam("ClntTrdgStat", request.ClntTrdgStat),
				new LB_DALParam("ClntAcctType", request.ClntAcctType),
				new LB_DALParam("ClntCDSNo", request.ClntCDSNo),
				new LB_DALParam("ClntDlrCode", request.ClntDlrCode),
				new LB_DALParam("ClntAllowAssociate", request.ClntAllowAssociate),
				new LB_DALParam("ClntDlrReassign", request.ClntDlrReassign),
				new LB_DALParam("ClntReassignDlrCode", request.ClntReassignDlrCode),
				new LB_DALParam("ClientCommission", request.ClientCommission),
				new LB_DALParam("AllowSME", request.AllowSME),

                // Common audit & workflow parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
				new LB_DALParam("MakerId", _currentUserService.UserId),
				new LB_DALParam("ActionDt", DateTime.Now),
				new LB_DALParam("TransDt", DateTime.Today),
				new LB_DALParam("ActionType", (byte)ActionTypeEnum.INSERT),
				new LB_DALParam("AuthId", _currentUserService.UserId),
				new LB_DALParam("AuthDt", DateTime.Now),
				new LB_DALParam("AuthTransDt", DateTime.Today),
				new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize),
				new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),
				new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
				new LB_DALParam("Remarks", request.Remarks),

                // Output parameters
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output),
				new LB_DALParam("AffectedRowsClntMaster", 0, ParameterDirection.Output),
				new LB_DALParam("AffectedRowsClntAcct", 0, ParameterDirection.Output),
				new LB_DALParam("TotalAffectedRows", 0, ParameterDirection.Output)
			};


			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_CrudClientMstAcc",
				parameters,
				cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");
			if (rowsAffected <= 0)
			{
				_logger.LogWarning("Client creation failed - No rows affected for ClntCode: {ClntCode}", request.ClntCode);
				throw new DomainException("Failed to create client - no rows affected");
			}

			await _unitOfWork.CommitTransactionAsync(cancellationToken);

			_logger.LogInformation("Successfully created Client with ClntCode: {ClntCode}", request.ClntCode);

			string gcifId = await GetGCIFByClientCodeAsync(request.CoBrchCode, request.ClntCode);

			var createdClient = await GetClientByIdAsync(gcifId);

			return createdClient ?? throw new DomainException("Failed to retrieve created client");
		}
		catch (ValidationException)
		{
			throw;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Client creation");
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while creating Client");
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while creating Client");
			throw new DomainException($"The client creation failed: {ex.Message}");
		}
	}

	public async Task<ClientDto> UpdateClientAsync(string gcif, UpdateClientRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Updating Client with GCIF: {GCIF}", gcif);

		// Validate the request
		var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			_logger.LogWarning("Validation failed for updating Client: {Errors}", string.Join(", ", validationResult.Errors));
			throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
		}

		// Check if client exists
		var existingClient = await GetClientByIdAsync(gcif, cancellationToken);
		if (existingClient == null)
		{
			_logger.LogWarning("The Client with GCIF {GCIF} not found for update", gcif);
			throw new NotFoundException("Client", gcif);
		}

		try
		{
			await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Action
                new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE),

                // ClntMaster parameters
                new LB_DALParam("GCIF", gcif),
				new LB_DALParam("ClntCode", request.ClntCode),
				new LB_DALParam("ClntName", request.ClntName),
				new LB_DALParam("ClntNICNo", request.ClntNICNo),
				new LB_DALParam("ClntAddr", request.ClntAddr),
				new LB_DALParam("ClntPhone", request.ClntPhone),
				new LB_DALParam("ClntMobile", request.ClntMobile),
				new LB_DALParam("Gender", request.Gender),
				new LB_DALParam("Nationality", request.Nationality),
				new LB_DALParam("ClntOffice", request.ClntOffice),
				new LB_DALParam("ClntFax", request.ClntFax),
				new LB_DALParam("ClntEmail", request.ClntEmail),
				new LB_DALParam("CountryCode", request.CountryCode),

                // ClntAcct parameters
                new LB_DALParam("CoBrchCode", request.CoBrchCode),
				new LB_DALParam("ClntStat", request.ClntStat),
				new LB_DALParam("ClntTrdgStat", request.ClntTrdgStat),
				new LB_DALParam("ClntAcctType", request.ClntAcctType),
				new LB_DALParam("ClntCDSNo", request.ClntCDSNo),
				new LB_DALParam("ClntDlrCode", request.ClntDlrCode),
				new LB_DALParam("ClntAllowAssociate", request.ClntAllowAssociate),
				new LB_DALParam("ClntDlrReassign", request.ClntDlrReassign),
				new LB_DALParam("ClntReassignDlrCode", request.ClntReassignDlrCode),
				new LB_DALParam("ClientCommission", request.ClientCommission),
				new LB_DALParam("AllowSME", request.AllowSME),

                // Common audit & workflow parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
				new LB_DALParam("MakerId", _currentUserService.UserId),
				new LB_DALParam("ActionDt", DateTime.Now),
				new LB_DALParam("TransDt", DateTime.Today),
				new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),
				new LB_DALParam("AuthId", _currentUserService.UserId),
				new LB_DALParam("AuthDt", DateTime.Now),
				new LB_DALParam("AuthTransDt", DateTime.Today),
				new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize),
				new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),
				new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
				new LB_DALParam("Remarks", request.Remarks),

                // Output parameters
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output),
				new LB_DALParam("AffectedRowsClntMaster", 0, ParameterDirection.Output),
				new LB_DALParam("AffectedRowsClntAcct", 0, ParameterDirection.Output),
				new LB_DALParam("TotalAffectedRows", 0, ParameterDirection.Output)
			};


			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_CrudClientMstAcc",
				parameters,
				cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");
			if (rowsAffected <= 0)
			{
				_logger.LogWarning("Client update failed - No rows affected for GCIF: {GCIF}", gcif);
				throw new DomainException("The client update failed because no rows were affected");
			}

			await _unitOfWork.CommitTransactionAsync(cancellationToken);

			_logger.LogInformation("Successfully updated Client with GCIF: {GCIF}", gcif);

			// Return the updated client
			var updatedClient = await GetClientByIdAsync(gcif, cancellationToken);
			return updatedClient ?? throw new DomainException("Failed to retrieve updated client");
		}
		catch (ValidationException)
		{
			throw;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Client update");
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while updating Client");
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while updating Client");
			throw new DomainException($"The client update failed: {ex.Message}");
		}
	}

	public async Task<bool> DeleteClientAsync(string gcif, DeleteClientRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Deleting Client with GCIF: {GCIF}", gcif);

		// Validate the request
		var validationResult = await _deleteValidator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			_logger.LogWarning("Validation failed for deleting Client: {Errors}", string.Join(", ", validationResult.Errors));
			throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
		}

		// Check if client exists
		var existingClient = await GetClientByIdAsync(gcif, cancellationToken);
		if (existingClient == null)
		{
			_logger.LogWarning("The Client with GCIF {GCIF} not found for deletion", gcif);
			throw new NotFoundException("Client", gcif);
		}

		try
		{
			await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Action
                new LB_DALParam("Action", (int)ActionTypeEnum.DELETE),

                // ClntMaster / ClntAcct identifiers
                new LB_DALParam("GCIF", gcif),
				new LB_DALParam("ClntCode", request.ClntCode),
				new LB_DALParam("CoBrchCode", request.CoBrchCode),

                // Common audit parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
				new LB_DALParam("MakerId", _currentUserService.UserId),
				new LB_DALParam("ActionDt", DateTime.Now),
				new LB_DALParam("TransDt", DateTime.Today),
				new LB_DALParam("ActionType", (byte)ActionTypeEnum.DELETE),
				new LB_DALParam("Remarks", request.Remarks ?? "Soft deleted"),

                // Output parameters
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output),
				new LB_DALParam("AffectedRowsClntMaster", 0, ParameterDirection.Output),
				new LB_DALParam("AffectedRowsClntAcct", 0, ParameterDirection.Output),
				new LB_DALParam("TotalAffectedRows", 0, ParameterDirection.Output)
			};

			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_CrudClientMstAcc",
				parameters,
				cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");
			if (rowsAffected <= 0)
			{
				_logger.LogWarning("Client deletion failed - No rows affected for GCIF: {GCIF}", gcif);
				throw new DomainException("The client deletion failed because no rows were affected");
			}

			await _unitOfWork.CommitTransactionAsync(cancellationToken);

			_logger.LogInformation("Successfully deleted Client with GCIF: {GCIF}", gcif);
			return true;
		}
		catch (ValidationException)
		{
			throw;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Client deletion");
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while deleting Client");
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while deleting Client");
			throw new DomainException($"The client deletion failed: {ex.Message}");
		}
	}

	public async Task<bool> ClientExistsAsync(string clntCode, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("Checking if Client exists with ClntCode: {ClntCode}", clntCode);

		var exists = await GetClientByClientCodeAsync(clntCode, cancellationToken);
		return exists;
	}

	#endregion

	#region Workflow Operations

	public async Task<PagedResult<ClientDto>> GetClientUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10,
		string? searchTerm = null, string? gcif = null, string? clntName = null, string? clntCode = null, int isAuth = 0, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Retrieving paged Client workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}", pageNumber, pageSize, isAuth);

		// Validate and sanitize inputs
		if (pageNumber < 1) pageNumber = 1;
		if (pageSize < 1 || pageSize > 100) pageSize = 10;

		try
		{
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
				new LB_DALParam("PageNumber", pageNumber),
				new LB_DALParam("PageSize", pageSize),
				new LB_DALParam("GCIF", gcif ?? (object)DBNull.Value),
				new LB_DALParam("ClntName", clntName ?? (object)DBNull.Value),
				new LB_DALParam("ClntCode", clntCode ?? (object)DBNull.Value),
				new LB_DALParam("MakerId", _currentUserService.UserId),
				new LB_DALParam("isAuth", (byte)isAuth),
				new LB_DALParam("SortBy", "ClntCode"),
				new LB_DALParam("SortOrder", "ASC")
			};


			_logger.LogDebug("Calling LB_SP_GetClientMstAccListWF with parameters: PageNumber={PageNumber}, PageSize={PageSize}, IsAuth={IsAuth}",
				pageNumber, pageSize, isAuth);

			var result = await _repository.QueryPagedAsync<ClientDto>(
				sqlOrSp: "LB_SP_GetClientMstAccListWF",
				parameters: parameters,
				isStoredProcedure: true,
				cancellationToken: cancellationToken);

			_logger.LogDebug("Workflow SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
				result.TotalCount, result.Data.Count());

			return result;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Client workflow listretrieval");
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while retrieving Client workflow list");
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while retrieving Client workflow list");
			throw new DomainException($"The client workflow list retrieval failed: {ex.Message}");
		}
	}

	public async Task<bool> AuthorizeClientAsync(string gcif, AuthorizeClientRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Authorizing Client with GCIF: {GCIF}, IsAuth: {IsAuth}", gcif, request.IsAuth);

		// Validate the request
		var validationResult = await _authorizeValidator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			_logger.LogWarning("Validation failed for authorizing Client: {Errors}", string.Join(", ", validationResult.Errors));
			throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
		}

		// Check if client exists
		var existingClient = await GetClientByIdAsync(gcif, cancellationToken);
		if (existingClient == null)
		{
			_logger.LogWarning("The Client with GCIF {GCIF} not found for authorization", gcif);
			throw new NotFoundException("Client", gcif);
		}

		try
		{
			await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
				new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE),
				new LB_DALParam("GCIF", gcif),
				new LB_DALParam("ClntCode", request.ClntCode),
				new LB_DALParam("CoBrchCode", request.CoBrchCode),
				new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
				new LB_DALParam("AuthID", _currentUserService.UserId),
				new LB_DALParam("IsAuth", request.IsAuth),
				new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),
				new LB_DALParam("ActionType", request.ActionType),

                // Output parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
			};

			var result = await _repository.ExecuteWithOutputAsync(
				"LB_SP_AuthClientMstAcc",
				parameters,
				cancellationToken);

			var rowsAffected = result.GetOutputValue<int>("RowsAffected");
			if (rowsAffected <= 0)
			{
				_logger.LogWarning("Client authorization failed - No rows affected for GCIF: {GCIF}", gcif);
				throw new DomainException("The client authorization failed because no rows were affected");
			}

			await _unitOfWork.CommitTransactionAsync(cancellationToken);

			_logger.LogInformation("Successfully authorized Client with GCIF: {GCIF}", gcif);
			return true;
		}
		catch (ValidationException)
		{
			throw;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "Invalid arguments were provided for Client authorization");
			throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Database operation encountered an error while authorizing Client");
			throw new DomainException($"The database operation failed: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred while authorizing Client");
			throw new DomainException($"The client authorization failed: {ex.Message}");
		}
	}

	#endregion

	#region Private Methods

	public async Task<string> GetGCIFByClientCodeAsync(string branchCode, string clientCode, CancellationToken cancellationToken = default)
	{
		var sql = @"
        SELECT CM.GCIF
        FROM CLNTMASTER CM
        INNER JOIN CLNTACCT CA ON CM.GCIF = CA.GCIF
        WHERE 1=1";

		List<LB_DALParam> parameters = new List<LB_DALParam>();

		if (!string.IsNullOrWhiteSpace(branchCode))
		{
			sql += " AND CA.COBRCHCODE = @BranchCode";
			parameters.Add(new LB_DALParam("BranchCode", branchCode));
		}

		if (!string.IsNullOrWhiteSpace(clientCode))
		{
			sql += " AND CA.CLNTCODE = @ClientCode";
			parameters.Add(new LB_DALParam("ClientCode", clientCode));

		}

		try
		{
			// Use ExecuteScalarAsync for primitive/scalar types like string
			var gcifResult = await _repository.ExecuteScalarAsync<string>(sql, parameters, false, cancellationToken);

			return gcifResult ?? string.Empty;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting GCIF by client code. BranchCode: {BranchCode}, ClientCode: {ClientCode}", branchCode, clientCode);
			throw;
		}
	}

	private async Task<bool> GetClientByClientCodeAsync(string clntCode, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("Checking client by ClntCode: {ClntCode}", clntCode);

		var sql = @"
            SELECT cm.GCIF
            FROM dbo.ClntMaster cm
            INNER JOIN dbo.ClntAcct ca ON cm.GCIF = ca.GCIF
            WHERE ca.ClntCode = @ClntCode";

		List<LB_DALParam> parameters = new List<LB_DALParam>()
		{
			new LB_DALParam("ClntCode", clntCode),
		};

		try
		{
			var result = await _repository.ExecuteScalarAsync<string>(sql, parameters, false, cancellationToken);
			return !string.IsNullOrEmpty(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting client by client code: {ClntCode}", clntCode);
			throw;
		}
	}

	#endregion
}