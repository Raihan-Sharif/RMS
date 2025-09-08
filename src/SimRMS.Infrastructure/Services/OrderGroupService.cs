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
/// Title:       Order Group Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Order Group information
/// Creation:    08/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

public class OrderGroupService : IOrderGroupService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateOrderGroupRequest> _createValidator;
    private readonly IValidator<UpdateOrderGroupRequest> _updateValidator;
    private readonly IValidator<DeleteOrderGroupRequest> _deleteValidator;
    private readonly IValidator<AuthorizeOrderGroupRequest> _authorizeValidator;
    private readonly IValidator<GetOrderGroupWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<OrderGroupService> _logger;

    public OrderGroupService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateOrderGroupRequest> createValidator,
        IValidator<UpdateOrderGroupRequest> updateValidator,
        IValidator<DeleteOrderGroupRequest> deleteValidator,
        IValidator<AuthorizeOrderGroupRequest> authorizeValidator,
        IValidator<GetOrderGroupWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<OrderGroupService> logger)
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

    public async Task<PagedResult<OrderGroupDto>> GetOrderGroupListAsync(int pageNumber = 1, int pageSize = 10,
        string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged OrderGroup list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchText = searchTerm,
                UsrID = (object)DBNull.Value,
                IsAuth = (object)DBNull.Value,
                DateFromStart = (object)DBNull.Value,
                DateFromEnd = (object)DBNull.Value,
                SortColumn = "GroupCode",
                SortDirection = "ASC",
                IncludeDeleted = false,
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetOrderGroupList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);

            var result = await _repository.QueryPagedAsync<OrderGroupDto>(
                sqlOrSp: "LB_SP_GetOrderGroupList",
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
            _logger.LogError(ex, "Invalid arguments for OrderGroup list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting OrderGroup list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting OrderGroup list");
            throw new DomainException($"Failed to retrieve order group list: {ex.Message}");
        }
    }

    public async Task<OrderGroupDto?> GetOrderGroupByCodeAsync(int groupCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting OrderGroup by Code: {GroupCode}", groupCode);

        try
        {
            var parameters = new
            {
                GroupCode = groupCode,
                StatusCode = 0, // output param
                StatusMsg = "" // output param
            };

            var result = await _repository.QuerySingleAsync<OrderGroupDto>(
                sqlOrSp: "LB_SP_GetOrderGroupByCode",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved order group: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for OrderGroup retrieval: {GroupCode}", groupCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting OrderGroup by Code: {GroupCode}", groupCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting OrderGroup by Code: {GroupCode}", groupCode);
            throw new DomainException($"Failed to retrieve order group: {ex.Message}");
        }
    }

    public async Task<OrderGroupDto> CreateOrderGroupAsync(CreateOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new OrderGroup: {GroupDesc}", request.GroupDesc);

        // Validate the request
        await ValidateCreateRequestAsync(request, cancellationToken);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.INSERT,
                ["GroupCode"] = (object)DBNull.Value, // Will be auto-generated
                ["GroupDesc"] = request.GroupDesc,
                ["GroupType"] = request.GroupType ?? (object)DBNull.Value,
                ["GroupValue"] = request.GroupValue ?? (object)DBNull.Value,
                ["DateFrom"] = request.DateFrom ?? (object)DBNull.Value,
                ["DateTo"] = request.DateTo ?? (object)DBNull.Value,
                ["UsrID"] = request.UsrID ?? (object)DBNull.Value,
                ["ViewOrder"] = request.ViewOrder,
                ["PlaceOrder"] = request.PlaceOrder,
                ["ViewClient"] = request.ViewClient,
                ["ModifyOrder"] = request.ModifyOrder,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.INSERT,
                ["AuthId"] = (object)DBNull.Value, // NULL for workflow
                ["AuthDt"] = (object)DBNull.Value, // NULL for workflow
                ["AuthTransDt"] = (object)DBNull.Value, // NULL for workflow
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize, // Always start unauthorized
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0, // OUTPUT parameter
                ["StatusCode"] = 0, // OUTPUT parameter
                ["StatusMsg"] = "" // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudOrderGroup with Action=1 (INSERT)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            _logger.LogDebug("Create operation completed. RowsAffected: {RowsAffected}, StatusCode: {StatusCode}, StatusMsg: {StatusMsg}", 
                rowsAffected, statusCode, statusMsg);

            if (rowsAffected <= 0 || statusCode != 1)
            {
                throw new DomainException($"Failed to create order group: {statusMsg}");
            }

            // Return manually constructed DTO since unauthorized records aren't returned by GetById SP
            var newOrderGroup = new OrderGroupDto
            {
                GroupDesc = request.GroupDesc,
                GroupType = request.GroupType,
                GroupValue = request.GroupValue,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                UsrID = request.UsrID,
                ViewOrder = request.ViewOrder,
                PlaceOrder = request.PlaceOrder,
                ViewClient = request.ViewClient,
                ModifyOrder = request.ModifyOrder,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date,
                ActionType = (byte)ActionTypeEnum.INSERT,
                IsAuth = (byte)AuthTypeEnum.UnAuthorize, // Initially unauthorized based on workflow
                AuthLevel = (byte)AuthLevelEnum.Level1,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks
            };

            return newOrderGroup;
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
            _logger.LogError(ex, "Unexpected error creating OrderGroup: {GroupDesc}", request.GroupDesc);
            throw new DomainException($"Failed to create order group: {ex.Message}");
        }
    }

    public async Task<OrderGroupDto> UpdateOrderGroupAsync(int groupCode, UpdateOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating OrderGroup: {GroupCode}", groupCode);

        // Use existing validation method
        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
            var existingGroup = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            if (existingGroup == null)
            {
                throw new DomainException($"Order group not found: {groupCode}");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.UPDATE,
                ["GroupCode"] = groupCode,
                ["GroupDesc"] = request.GroupDesc ?? (object)DBNull.Value,
                ["GroupType"] = request.GroupType ?? (object)DBNull.Value,
                ["GroupValue"] = request.GroupValue ?? (object)DBNull.Value,
                ["DateFrom"] = request.DateFrom ?? (object)DBNull.Value,
                ["DateTo"] = request.DateTo ?? (object)DBNull.Value,
                ["UsrID"] = request.UsrID ?? (object)DBNull.Value,
                ["ViewOrder"] = request.ViewOrder ?? (object)DBNull.Value,
                ["PlaceOrder"] = request.PlaceOrder ?? (object)DBNull.Value,
                ["ViewClient"] = request.ViewClient ?? (object)DBNull.Value,
                ["ModifyOrder"] = request.ModifyOrder ?? (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.UPDATE,
                ["AuthId"] = (object)DBNull.Value, // Reset for workflow
                ["AuthDt"] = (object)DBNull.Value, // Reset for workflow
                ["AuthTransDt"] = (object)DBNull.Value, // Reset for workflow
                ["IsAuth"] = (byte)AuthTypeEnum.UnAuthorize, // Reset to unauthorized for workflow
                ["AuthLevel"] = (byte)AuthLevelEnum.Level1,
                ["IsDel"] = (byte)DeleteStatusEnum.Active,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0, // OUTPUT parameter
                ["StatusCode"] = 0, // OUTPUT parameter
                ["StatusMsg"] = "" // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudOrderGroup with Action=2 (UPDATE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            _logger.LogDebug("Update operation completed. RowsAffected: {RowsAffected}, StatusCode: {StatusCode}, StatusMsg: {StatusMsg}", 
                rowsAffected, statusCode, statusMsg);

            if (rowsAffected <= 0 || statusCode != 1)
            {
                throw new DomainException($"Failed to update order group: {statusMsg}");
            }

            var updatedGroup = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);

            if (updatedGroup == null)
            {
                throw new DomainException($"Updated order group not found: {groupCode}");
            }

            _logger.LogInformation("Successfully updated OrderGroup: {GroupCode}", groupCode);
            return updatedGroup;
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
            _logger.LogError(ex, "Unexpected error updating OrderGroup: {GroupCode}", groupCode);
            throw new DomainException($"Failed to update order group: {ex.Message}");
        }
    }

    public async Task<bool> DeleteOrderGroupAsync(int groupCode, DeleteOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting OrderGroup: {GroupCode}", groupCode);

        // Use existing validation method
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var existingGroup = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            if (existingGroup == null)
            {
                throw new DomainException($"Order group not found: {groupCode}");
            }

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = (byte)ActionTypeEnum.DELETE,
                ["GroupCode"] = groupCode,
                ["GroupDesc"] = (object)DBNull.Value,
                ["GroupType"] = (object)DBNull.Value,
                ["GroupValue"] = (object)DBNull.Value,
                ["DateFrom"] = (object)DBNull.Value,
                ["DateTo"] = (object)DBNull.Value,
                ["UsrID"] = (object)DBNull.Value,
                ["ViewOrder"] = (object)DBNull.Value,
                ["PlaceOrder"] = (object)DBNull.Value,
                ["ViewClient"] = (object)DBNull.Value,
                ["ModifyOrder"] = (object)DBNull.Value,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["MakerId"] = _currentUserService.UserId,
                ["ActionDt"] = DateTime.Now,
                ["TransDt"] = DateTime.Now.Date,
                ["ActionType"] = (byte)ActionTypeEnum.DELETE,
                ["AuthId"] = (object)DBNull.Value,
                ["AuthDt"] = (object)DBNull.Value,
                ["AuthTransDt"] = (object)DBNull.Value,
                ["IsAuth"] = (object)DBNull.Value, // SP handles deletion logic
                ["AuthLevel"] = (object)DBNull.Value,
                ["IsDel"] = (object)DBNull.Value, // SP sets this to 1
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0, // OUTPUT parameter
                ["StatusCode"] = 0, // OUTPUT parameter
                ["StatusMsg"] = "" // OUTPUT parameter
            };

            _logger.LogDebug("Calling LB_SP_CrudOrderGroup with Action=3 (DELETE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            _logger.LogDebug("Delete operation completed. RowsAffected: {RowsAffected}, StatusCode: {StatusCode}, StatusMsg: {StatusMsg}", 
                rowsAffected, statusCode, statusMsg);

            var success = rowsAffected > 0 && statusCode == 1;

            if (success)
            {
                _logger.LogInformation("Successfully deleted OrderGroup: {GroupCode}", groupCode);
            }
            else
            {
                _logger.LogWarning("Delete operation failed: {StatusMsg} - GroupCode: {GroupCode}", statusMsg, groupCode);
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
            _logger.LogError(ex, "Unexpected error deleting OrderGroup: {GroupCode}", groupCode);
            throw new DomainException($"Failed to delete order group: {ex.Message}");
        }
    }

    public async Task<bool> OrderGroupExistsAsync(int groupCode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if OrderGroup exists: {GroupCode}", groupCode);

        try
        {
            var group = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            return group != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking OrderGroup existence: {GroupCode}", groupCode);
            return false;
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<OrderGroupDto>> GetOrderGroupUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? searchTerm = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       CancellationToken cancellationToken = default)
    {
        // Create request model for validation
        var request = new GetOrderGroupWorkflowListRequest
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

        _logger.LogInformation("Getting {authAction} OrderGroup list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = pageSize,
                ["SearchText"] = searchTerm ?? (object)DBNull.Value,
                ["SortColumn"] = "GroupCode",
                ["SortDirection"] = "ASC",
                ["isAuth"] = (byte)isAuth, // 0: Unauthorized or 2: Denied records
                ["MakerId"] = _currentUserService.UserId,
                ["TotalCount"] = 0 // OUTPUT parameter - will be populated by SP
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetOrderGroupListWF", pageNumber, pageSize, isAuth, _currentUserService.UserId);

            var result = await _repository.QueryPagedAsync<OrderGroupDto>(
                sqlOrSp: "LB_SP_GetOrderGroupListWF",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            if (result.TotalCount == 0)
            {
                _logger.LogWarning("SP {StoredProcedure} returned TotalCount=0. Check OUTPUT parameter handling or SP logic.",
                    "LB_SP_GetOrderGroupListWF");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting " + authAction + " order group list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);
            throw;
        }
    }

    public async Task<bool> AuthorizeOrderGroupAsync(int groupCode, AuthorizeOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} OrderGroup in workflow: {GroupCode}", groupCode, authAction);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var ipAddress = _currentUserService.GetClientIPAddress();

            var parameters = new Dictionary<string, object>
            {
                ["Action"] = request.ActionType,
                ["GroupCode"] = groupCode,
                ["IPAddress"] = ipAddress,
                ["AuthID"] = _currentUserService.UserId,
                ["IsAuth"] = request.IsAuth,
                ["ActionType"] = request.ActionType,
                ["Remarks"] = !string.IsNullOrEmpty(request.Remarks) ? request.Remarks : (object)DBNull.Value,
                ["RowsAffected"] = 0, // OUTPUT parameter
                ["StatusCode"] = 0, // OUTPUT parameter
                ["StatusMsg"] = "" // OUTPUT parameter
            };
            
            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthOrderGroup",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (rowsAffected > 0 && statusCode == 1)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized OrderGroup: {GroupCode}, RowsAffected: {RowsAffected}",
                    groupCode, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("No rows affected during {authAction} authorization of OrderGroup: {GroupCode}, StatusMsg: {StatusMsg}", 
                    groupCode, authAction, statusMsg);
                throw new DomainException($"Failed to authorize: {authAction} order group: {statusMsg}");
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
            _logger.LogError(ex, "Error {authAction} authorizing OrderGroup in workflow: {GroupCode}", groupCode, authAction);
            throw new DomainException($"Failed to authorize: {authAction} order group: {ex.Message}");
        }
    }
    #endregion

    #region Private Validation Methods

    private async Task ValidateCreateRequestAsync(CreateOrderGroupRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateUpdateRequestAsync(UpdateOrderGroupRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateDeleteRequestAsync(DeleteOrderGroupRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateAuthorizeRequestAsync(AuthorizeOrderGroupRequest request, CancellationToken cancellationToken)
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

    private async Task ValidateWorkflowListRequestAsync(GetOrderGroupWorkflowListRequest request, CancellationToken cancellationToken)
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