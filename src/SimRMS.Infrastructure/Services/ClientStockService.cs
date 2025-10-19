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
/// Title:       Client Stock Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Client Stock information
/// Creation:    29/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// Asif Zaman         30-09-25     Aligned the parameters of workflow methods as per SP
///
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
            var parameters = new
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode,
                XchgCode = request.XchgCode,
                SearchText = request.SearchText,
                SortColumn = request.SortColumn ?? "ClientCode",
                SortDirection = request.SortDirection ?? "ASC",
                TotalCount = 0
            };

            var result = await _repository.QueryPagedAsync<ClientStockDto>(
                sqlOrSp: "LB_SP_GetShareInfoList",
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
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
            var parameters = new
            {
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode,
                StatusCode = 0, // output param
                StatusMsg = "" // output param
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

            var parameters = new
            {
                Action = (int)ActionTypeEnum.INSERT,
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode,
                XchgCode = request.XchgCode,
                OpenFreeBalance = request.OpenFreeBalance,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = (object)DBNull.Value,
                TransDt = (object)DBNull.Value,
                ActionType = (byte)ActionTypeEnum.INSERT,
                AuthId = (object)DBNull.Value,
                AuthDt = (object)DBNull.Value,
                AuthTransDt = (object)DBNull.Value,
                IsAuth = (byte)AuthTypeEnum.UnAuthorize, // Always start unauthorized
                AuthLevel = (byte)AuthLevelEnum.Level1,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks ?? (object)DBNull.Value,
                RowsAffected = 0
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

            var parameters = new
            {
                Action = (int)ActionTypeEnum.UPDATE,
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode,
                PendingFreeBalance = request.PendingFreeBalance,

                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = (object)DBNull.Value,
                TransDt = (object)DBNull.Value,
                ActionType = (byte)ActionTypeEnum.UPDATE,
                AuthId = (object)DBNull.Value,
                AuthDt = (object)DBNull.Value,
                AuthTransDt = (object)DBNull.Value,
                IsAuth = (byte)AuthTypeEnum.UnAuthorize, // Reset to unauthorized for workflow
                AuthLevel = (byte)AuthLevelEnum.Level1,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks ?? (object)DBNull.Value,
                RowsAffected = 0
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

            var parameters = new
            {
                Action = (int)ActionTypeEnum.DELETE,
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                Remarks = request.Remarks,
                RowsAffected = 0
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

            var parameters = new Dictionary<string, object>
            {
                { "branchCode", request.BranchCode },
                { "clientCode", request.ClientCode },
                { "stockCode", request.StockCode }
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
            var parameters = new
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode,
                XchgCode = request.XchgCode,
                SearchText = request.SearchText,
                SortColumn = request.SortColumn ?? "ClientCode",
                SortDirection = request.SortDirection ?? "ASC",
                IsAuth = (int)request.IsAuth,
                MakerId = _currentUserService.UserId,
                TotalCount = 0
            };

            var result = await _repository.QueryPagedAsync<ClientStockDto>(
                sqlOrSp: "LB_SP_GetShareInfoListWF",
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
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

            var parameters = new
            {
                Action = (int)ActionTypeEnum.UPDATE,
                BranchCode = request.BranchCode,
                ClientCode = request.ClientCode,
                StockCode = request.StockCode,
                PendingFreeBal = request.PendingFreeBalance,
                ActionType = (int)ActionTypeEnum.UPDATE,
                //AuthAction = (int)request.AuthAction, // not passed in the AuthShareInfo SP
                IsAuth = request.IsAuth,
                AuthId = _currentUserService.UserId,
                IPAddress = _currentUserService.GetClientIPAddress(),
                Remarks = request.Remarks,
                RowsAffected = 0
            };

            var result = await _repository.ExecuteAsync("LB_SP_AuthShareInfo", parameters, true, cancellationToken);

            if (result <= 0)
            {
                throw new NotFoundException("Client Stock", $"Branch: {request.BranchCode}, Client: {request.ClientCode}, Stock: {request.StockCode}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Client Stock authorization completed successfully for Branch: {BranchCode}, Client: {ClientCode}, Stock: {StockCode}, Action: {AuthAction}",
                request.BranchCode, request.ClientCode, request.StockCode, request.AuthAction);

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