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
using Azure;
using System.Reflection;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Stock Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Client Stock information
/// Creation:    29/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// Asif Zaman         30-Sep-25     Aligned the parameters of workflow methods as per SP
/// Asif Zaman         11-Nov-25     TpOms integration for client share holding update   
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

public class ClientStockService : IClientStockService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetClientStockListRequest> _listValidator;
    private readonly IValidator<GetClientStockByKeyRequest> _getByKeyValidator;
    private readonly IValidator<CreateClientStockRequest> _createValidator;
    private readonly IValidator<UpdateClientStockRequest> _updateValidator;
    private readonly IValidator<DeleteClientStockRequest> _deleteValidator;
    private readonly IValidator<AuthorizeClientStockRequest> _authorizeValidator;
    private readonly IValidator<GetClientStockWorkflowListRequest> _workflowListValidator;
    private readonly ITpOmsService _tpOmsService; //tpoms injection
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ClientStockService> _logger;


    public ClientStockService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<GetClientStockListRequest> listValidator,
        IValidator<GetClientStockByKeyRequest> getByKeyValidator,
        IValidator<CreateClientStockRequest> createValidator,
        IValidator<UpdateClientStockRequest> updateValidator,
        IValidator<DeleteClientStockRequest> deleteValidator,
        IValidator<AuthorizeClientStockRequest> authorizeValidator,
        IValidator<GetClientStockWorkflowListRequest> workflowListValidator,
        ITpOmsService tpOmsService,
        ICurrentUserService currentUserService,
        ILogger<ClientStockService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _listValidator = listValidator;
        _getByKeyValidator = getByKeyValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
        _workflowListValidator = workflowListValidator;
        _tpOmsService = tpOmsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Main CRUD Operations

    public async Task<PagedResult<ClientStockDto>> GetClientStockListAsync(GetClientStockListRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client Stock list with filters - Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);

        // Validate request
        var validationResult = await _listValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetClientStockListRequest: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            throw new ValidationException("The request parameters were invalid") { ValidationErrors = errors };
        }

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
	            new LB_DALParam("PageNumber", request.PageNumber),
	            new LB_DALParam("PageSize", request.PageSize),
	            new LB_DALParam("BranchCode", request.BranchCode ?? (object)DBNull.Value),
	            new LB_DALParam("ClientCode", request.ClientCode ?? (object)DBNull.Value),
	            new LB_DALParam("StockCode", request.StockCode ?? (object)DBNull.Value),
	            new LB_DALParam("XchgCode", request.XchgCode ?? (object)DBNull.Value),
	            new LB_DALParam("SearchText", request.SearchText ?? (object)DBNull.Value),
	            new LB_DALParam("SearchColumn", request.SearchColumn ?? (object)DBNull.Value),
                new LB_DALParam("SortColumn", request.SortColumn ?? "ClientCode"),
	            new LB_DALParam("SortDirection", request.SortDirection ?? "ASC")
            };

			var result = await _repository.QueryPagedAsync<ClientStockDto>(
                sqlOrSp: "LB_SP_GetShareInfoList",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            var pagedResult = result;

            _logger.LogInformation("Retrieved {Count} Client Stock records out of {Total}", result.Data.Count(), result.TotalCount);
            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving Client Stock list");
            throw new DomainException($"Failed to retrieve Client Stock list: {ex.Message}", ex);
        }
    }

    public async Task<ClientStockDto?> GetClientStockByKeyAsync(GetClientStockByKeyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client Stock by key - Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            request.BranchCode, request.ClientCode, request.StockCode);

        // Validate request
        var validationResult = await _getByKeyValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetClientStockByKeyRequest: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            throw new ValidationException("The request parameters were invalid") { ValidationErrors = errors };
        }

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
				new LB_DALParam("BranchCode", request.BranchCode ?? (object)DBNull.Value),
				new LB_DALParam("ClientCode", request.ClientCode ?? (object)DBNull.Value),
				new LB_DALParam("StockCode", request.StockCode ?? (object)DBNull.Value),
                // Output parameters
				new LB_DALParam("StatusCode", 0, ParameterDirection.Output),
				new LB_DALParam("StatusMsg", string.Empty, ParameterDirection.Output)
			};

			var result = await _repository.QuerySingleAsync<ClientStockDto>(
                sqlOrSp: "LB_SP_GetShareInfoByKey",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved Client Stock: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client Stock retrieval: {BranchCode}-{ClientCode}-{StockCode}", request.BranchCode, request.ClientCode, request.StockCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Client Stock by key: {BranchCode}-{ClientCode}-{StockCode}", request.BranchCode, request.ClientCode, request.StockCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Client Stock by key: {BranchCode}-{ClientCode}-{StockCode}", request.BranchCode, request.ClientCode, request.StockCode);
            throw new DomainException($"Failed to retrieve Client Stock: {ex.Message}");
        }
    }

    public async Task<ClientStockDto> CreateClientStockAsync(CreateClientStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Client Stock for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            request.BranchCode, request.ClientCode, request.StockCode);

        // Validate request
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreateClientStockRequest: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            throw new ValidationException("The request parameters were invalid") { ValidationErrors = errors };
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.INSERT),
	            new LB_DALParam("BranchCode", request.BranchCode),
	            new LB_DALParam("ClientCode", request.ClientCode),
	            new LB_DALParam("StockCode", request.StockCode),
	            new LB_DALParam("XchgCode", request.XchgCode),
	            new LB_DALParam("OpenFreeBalance", request.OpenFreeBalance),
	            new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", (object)DBNull.Value),
	            new LB_DALParam("TransDt", (object)DBNull.Value),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.INSERT),
	            new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize), // Always start unauthorized
                new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter (RowsAffected)
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteAsync("LB_SP_CrudShareInfo", parameters, true, cancellationToken);

            if (result <= 0)
            {
                throw new DomainException("Failed to create Client Stock record");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Client Stock created successfully for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
                request.BranchCode, request.ClientCode, request.StockCode);

            // Return the created record
            var getRequest = new GetClientStockByKeyRequest
            {
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode
            };

            var createdRecord = await GetClientStockByKeyAsync(getRequest, cancellationToken);
            return createdRecord ?? throw new DomainException("Failed to retrieve created Client Stock record");
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating Client Stock");
            throw new DomainException($"Failed to create Client Stock: {ex.Message}", ex);
        }
    }

    public async Task<ClientStockDto> UpdateClientStockAsync(UpdateClientStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Client Stock for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            request.BranchCode, request.ClientCode, request.StockCode);

        // Validate request
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdateClientStockRequest: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            throw new ValidationException("The request parameters were invalid") { ValidationErrors = errors };
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE),
	            new LB_DALParam("BranchCode", request.BranchCode),
	            new LB_DALParam("ClientCode", request.ClientCode),
	            new LB_DALParam("StockCode", request.StockCode),
	            new LB_DALParam("PendingFreeBalance", request.PendingFreeBalance),

	            new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", (object)DBNull.Value),
	            new LB_DALParam("TransDt", (object)DBNull.Value),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),
	            new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize), // Reset to unauthorized for workflow
                new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };
			var result = await _repository.ExecuteAsync("LB_SP_CrudShareInfo", parameters, true, cancellationToken);

            if (result <= 0)
            {
                throw new NotFoundException("Client Stock", $"Branch: {request.BranchCode}, Client: {request.ClientCode}, Stock: {request.StockCode}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Client Stock updated successfully for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
                request.BranchCode, request.ClientCode, request.StockCode);

            // Return the updated record
            var getRequest = new GetClientStockByKeyRequest
            {
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode
            };

            var updatedRecord = await GetClientStockByKeyAsync(getRequest, cancellationToken);
            return updatedRecord ?? throw new DomainException("Failed to retrieve updated Client Stock record");
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating Client Stock");
            throw new DomainException($"Failed to update Client Stock: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteClientStockAsync(DeleteClientStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Client Stock for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            request.BranchCode, request.ClientCode, request.StockCode);

        // Validate request
        var validationResult = await _deleteValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for DeleteClientStockRequest: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            throw new ValidationException("The request parameters were invalid") { ValidationErrors = errors };
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.DELETE),
	            new LB_DALParam("BranchCode", request.BranchCode),
	            new LB_DALParam("ClientCode", request.ClientCode),
	            new LB_DALParam("StockCode", request.StockCode),
	            new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
                // Assuming Remarks can be null and should be converted to DBNull.Value
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteAsync("LB_SP_CrudShareInfo", parameters, true, cancellationToken);

            if (result <= 0)
            {
                throw new NotFoundException("Client Stock", $"Branch: {request.BranchCode}, Client: {request.ClientCode}, Stock: {request.StockCode}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Client Stock deleted successfully for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
                request.BranchCode, request.ClientCode, request.StockCode);

            return true;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting Client Stock");
            throw new DomainException($"Failed to delete Client Stock: {ex.Message}", ex);
        }
    }

    public async Task<bool> ClientStockExistsAsync(GetClientStockByKeyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if Client Stock exists for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
            request.BranchCode, request.ClientCode, request.StockCode);

        try
        {
            var sql = @"
                SELECT si.BranchCode, si.ClientCode, si.StockCode
                FROM dbo.ShareInfo si
                WHERE si.BranchCode = @branchCode
                  AND si.ClientCode = @clientCode
                  AND si.StockCode = @stockCode";

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("branchCode", request.BranchCode ?? (object)DBNull.Value),
	            new LB_DALParam("clientCode", request.ClientCode ?? (object)DBNull.Value),
	            new LB_DALParam("stockCode", request.StockCode ?? (object)DBNull.Value)
            };

			var result = await _repository.ExecuteScalarAsync<string>(sql, parameters, false, cancellationToken);
            return !string.IsNullOrEmpty(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Client Stock existence: Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}",
                request.BranchCode, request.ClientCode, request.StockCode);
            throw new DomainException($"Failed to check Client Stock existence: {ex.Message}", ex);
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<ClientStockDto>> GetClientStockWorkflowListAsync(GetClientStockWorkflowListRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client Stock workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}",
            request.PageNumber, request.PageSize, request.IsAuth);

        // Validate request
        var validationResult = await _workflowListValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetClientStockWorkflowListRequest: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            throw new ValidationException("The request parameters were invalid") { ValidationErrors = errors };
        }

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination and Sorting Parameters
                new LB_DALParam("PageNumber", request.PageNumber),
	            new LB_DALParam("PageSize", request.PageSize),
    
                // Filtering Parameters (assuming they can be nullable and need DBNull)
                new LB_DALParam("BranchCode", request.BranchCode ?? (object)DBNull.Value),
	            new LB_DALParam("ClientCode", request.ClientCode ?? (object)DBNull.Value),
	            new LB_DALParam("StockCode", request.StockCode ?? (object)DBNull.Value),
	            new LB_DALParam("XchgCode", request.XchgCode ?? (object)DBNull.Value),
	            new LB_DALParam("SearchText", request.SearchText ?? (object)DBNull.Value),
    
                // Defaulted Parameters
                new LB_DALParam("SortColumn", request.SortColumn ?? "ClientCode"),
	            new LB_DALParam("SortDirection", request.SortDirection ?? "ASC"),
    
                // Status and User Parameter
                new LB_DALParam("IsAuth", (int)request.IsAuth),
	            new LB_DALParam("MakerId", _currentUserService.UserId)
            };

			var result = await _repository.QueryPagedAsync<ClientStockDto>(
                sqlOrSp: "LB_SP_GetShareInfoListWF",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            var pagedResult = result;

            _logger.LogInformation("Retrieved {Count} Client Stock workflow records out of {Total}", result.Data.Count(), result.TotalCount);
            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving Client Stock workflow list");
            throw new DomainException($"Failed to retrieve Client Stock workflow list: {ex.Message}", ex);
        }
    }

    public async Task<bool> AuthorizeClientStockAsync(AuthorizeClientStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing Client Stock for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}, Action: {AuthAction}",
            request.BranchCode, request.ClientCode, request.StockCode,request.AuthAction);

        // Validate request
        var validationResult = await _authorizeValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for AuthorizeClientStockRequest: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            throw new ValidationException("The request parameters were invalid") { ValidationErrors = errors };
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Key/Action Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE),
	            new LB_DALParam("BranchCode", request.BranchCode),
	            new LB_DALParam("ClientCode", request.ClientCode),
	            new LB_DALParam("StockCode", request.StockCode),
    
                // Data/Balance Parameter (Note the name change from request to DAL)
                new LB_DALParam("PendingFreeBal", request.PendingFreeBalance),
    
                // Control/Audit Parameters
                new LB_DALParam("ActionType", (int)ActionTypeEnum.UPDATE),
	            new LB_DALParam("IsAuth", request.IsAuth), // Authorization status (e.g., 1 for Authorized)
                new LB_DALParam("AuthId", _currentUserService.UserId), // The user performing the authorization
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
    
                // Remarks (Ensuring null is passed as DBNull)
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter for Rows Affected
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteAsync("LB_SP_AuthShareInfo", parameters, true, cancellationToken);

            if (result <= 0)
            {
                throw new NotFoundException("Client Stock", $"Branch: {request.BranchCode}, Client: {request.ClientCode}, Stock: {request.StockCode}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Client Stock authorization completed successfully for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}, Action: {AuthAction}",
                request.BranchCode, request.ClientCode, request.StockCode, request.AuthAction);

            //tpOms call to update share holding
            var tpOmsRequest = new TpOmsUpdateShareHoldingRequest
            {
                clientId = request.ClientCode,
                branchId = Convert.ToInt32(request.BranchCode),
                stockCode = request.StockCode,
                exchangeCode = "DSE",
            };
            var tpOmsResult = await _tpOmsService.UpdateShareHoldingAsync(tpOmsRequest, cancellationToken);


            return true;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while authorizing Client Stock");
            throw new DomainException($"Failed to authorize Client Stock: {ex.Message}", ex);
        }
    }

    #endregion
}