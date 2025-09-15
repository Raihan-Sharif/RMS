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
using System.Data;
using System.Text.Json;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Order Group Service (Master-Detail Architecture)
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Order Group master-detail operations
/// Creation:    15/Sep/2025
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
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<OrderGroupService> _logger;

    // Unified CRUD validators
    private readonly IValidator<CreateOrderGroupRequest> _createValidator;
    private readonly IValidator<UpdateOrderGroupRequest> _updateValidator;
    private readonly IValidator<DeleteOrderGroupRequest> _deleteValidator;
    private readonly IValidator<AuthorizeOrderGroupRequest> _authorizeValidator;

    public OrderGroupService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<OrderGroupService> logger,
        IValidator<CreateOrderGroupRequest> createValidator,
        IValidator<UpdateOrderGroupRequest> updateValidator,
        IValidator<DeleteOrderGroupRequest> deleteValidator,
        IValidator<AuthorizeOrderGroupRequest> authorizeValidator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
    }

    #region Common Operations

    public async Task<PagedResult<OrderGroupDto>> GetOrderGroupListAsync(int pageNumber = 1, int pageSize = 10, string? SearchText = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group list - Page: {PageNumber}, Size: {PageSize}, Search: {SearchText}", pageNumber, pageSize, SearchText);

            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchText = SearchText,
                UsrId = (object)DBNull.Value,
                SortColumn = "GroupCode",
                SortDirection = "ASC",
                TotalCount = 0 // output param
            };

            // Get flat data from SP using proper DTO
            var flatResults = await _repository.QueryPagedAsync<OrderGroupCombinedDto>(
                "LB_SP_GetOrderGroupList",
                pageNumber,
                pageSize,
                parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            // Group the flat results into nested structure
            var groupedData = flatResults.Data
                .GroupBy(x => x.GroupCode)
                .Select(group =>
                {
                    var first = group.First();
                    return new OrderGroupDto
                    {
                        GroupCode = first.GroupCode,
                        GroupDesc = first.GroupDesc,
                        GroupType = first.GroupType,
                        GroupValue = first.GroupValue,
                        DateFrom = first.DateFrom,
                        DateTo = first.DateTo,
                        MakeBy = first.MakeBy,
                        AuthBy = first.AuthBy,
                        GroupStatus = first.GroupStatus,
                        AuthorizationStatus = first.AuthorizationStatus,
                        RecordStatus = first.RecordStatus,
                        UserCount = first.UserCount,
                        Users = group.Where(x => !string.IsNullOrEmpty(x.UsrID))
                            .Select(user => new OrderGroupUserDto
                            {
                                GroupCode = first.GroupCode,
                                UsrID = user.UsrID ?? string.Empty,
                                ViewOrder = user.ViewOrder ?? false,
                                PlaceOrder = user.PlaceOrder ?? false,
                                ViewClient = user.ViewClient ?? false,
                                ModifyOrder = user.ModifyOrder ?? false,
                                MakeBy = user.MakeBy,
                                AuthBy = user.AuthBy
                            }).ToList()
                    };
                }).ToList();

            return new PagedResult<OrderGroupDto>
            {
                Data = groupedData,
                PageNumber = flatResults.PageNumber,
                PageSize = flatResults.PageSize,
                TotalCount = flatResults.TotalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Order Group list");
            throw new DomainException($"Failed to retrieve Order Group list: {ex.Message}");
        }
    }

    public async Task<OrderGroupUserDto?> GetOrderGroupUserByCodeAsync(int groupCode, string usrId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group user by code: {GroupCode}, User: {UsrId}", groupCode, usrId);

            var parameters = new
            {
                GroupCode = groupCode,
                UsrId = usrId ?? (object)DBNull.Value,
                statusCode = string.Empty, //output param
                statusMsg = string.Empty // output param
            };

            var orderGroups = await _repository.QueryAsync<OrderGroupUserDto>(
                "LB_SP_GetOrderGroupByCode",
                parameters,
                isStoredProcedure: true,
                cancellationToken);

            return orderGroups.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OrderGroup by code: {GroupCode}", groupCode);
            throw new DomainException($"Failed to retrieve OrderGroup: {ex.Message}");
        }
    }

    public async Task<OrderGroupDto> GetOrderGroupByCodeAsync(int groupCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group by code: {GroupCode}", groupCode);

            var parameters = new
            {
                GroupCode = groupCode,
                UsrId = (object)DBNull.Value,
                statusCode = string.Empty, //output param
                statusMsg = string.Empty // output param
            };

            // Get flat data from SP using proper DTO
            var flatResults = await _repository.QueryAsync<OrderGroupCombinedDto>(
                "LB_SP_GetOrderGroupByCode",
                parameters,
                isStoredProcedure: true,
                cancellationToken);

            // Group the flat results into nested structure
            var groupedResults = flatResults
                .GroupBy(x => x.GroupCode)
                .Select(group =>
                {
                    var first = group.First();
                    return new OrderGroupDto
                    {
                        GroupCode = first.GroupCode,
                        GroupDesc = first.GroupDesc,
                        GroupType = first.GroupType,
                        GroupValue = first.GroupValue,
                        DateFrom = first.DateFrom,
                        DateTo = first.DateTo,
                        GroupStatus = first.GroupStatus,
                        AuthorizationStatus = first.AuthorizationStatus,
                        RecordStatus = first.RecordStatus,
                        MakeBy = first.MakeBy,
                        AuthBy = first.AuthBy,
                        UserCount = first.UserCount,
                        Users = group.Where(x => !string.IsNullOrEmpty(x.UsrID))
                            .Select(user => new OrderGroupUserDto
                            {
                                GroupCode = first.GroupCode,
                                UsrID = user.UsrID ?? string.Empty,
                                ViewOrder = user.ViewOrder ?? false,
                                PlaceOrder = user.PlaceOrder ?? false,
                                ViewClient = user.ViewClient ?? false,
                                ModifyOrder = user.ModifyOrder ?? false,
                                MakeBy = user.MakeBy,
                                AuthBy = user.AuthBy
                            }).ToList()
                    };
                }).ToList();

            return groupedResults.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OrderGroup by code: {GroupCode}", groupCode);
            throw new DomainException($"Failed to retrieve OrderGroup: {ex.Message}");
        }
    }

    public async Task<bool> OrderGroupExistsAsync(int groupCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking OrderGroup existence: {GroupCode}", groupCode);
            return false;
        }
    }

    public async Task<OrderGroupDeleteResultDto> CheckGroupDeleteValidationAsync(int groupCode, string? usrId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking delete validation for Order Group: {GroupCode}", groupCode);

            // Get order group with all users to count them
            var orderGroups = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
          
            if (orderGroups == null)
            {
                return new OrderGroupDeleteResultDto
                {
                    GroupCode = groupCode,
                    UserCount = 0,
                    CanDelete = false,
                    ValidationMessage = $"Order Group with code '{groupCode}' not found"
                };
            }

            // Count users in nested structure
            var userCount = orderGroups.Users.Count();

            return new OrderGroupDeleteResultDto
            {
                GroupCode = groupCode,
                UserCount = userCount,
                CanDelete = userCount == 0 || !string.IsNullOrEmpty(usrId),
                ValidationMessage = userCount > 0 
                    ? $"Cannot delete group. Please remove all {userCount} user(s) first."
                    : "Group can be deleted safely"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking delete validation for Order Group: {GroupCode}", groupCode);
            return new OrderGroupDeleteResultDto
            {
                GroupCode = groupCode,
                UserCount = -1,
                CanDelete = false,
                ValidationMessage = $"Error validating group deletion: {ex.Message}"
            };
        }
    }

    #endregion

    #region DML Operations (Unified CRUD Master and child table)

    public async Task<OrderGroupDto> CreateOrderGroupAsync(CreateOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateCreateRequestAsync(request, cancellationToken);

        _logger.LogInformation("Creating Order Group with nested users: {GroupDesc}", request.GroupDesc);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Serialize nested users to JSON if available
            string? orderGroupDtlData = null;
            if (request.Users != null && request.Users.Any())
            {
                var userDataForJson = request.Users.Select(u => new
                {
                    UsrID = u.UsrID,
                    ViewOrder = u.ViewOrder ?? false,
                    PlaceOrder = u.PlaceOrder ?? false,
                    ViewClient = u.ViewClient ?? false,
                    ModifyOrder = u.ModifyOrder ?? false
                }).ToList();
                
                orderGroupDtlData = JsonSerializer.Serialize(userDataForJson);
                _logger.LogInformation("Serialized user data for Order Group: {UserData}", orderGroupDtlData);
            }

            var parameters = new
            {
                Action = (int)ActionTypeEnum.INSERT,
                GroupCode = (object)DBNull.Value,
                GroupDesc = request.GroupDesc,
                GroupType = request.GroupType,
                GroupValue = request.GroupValue,
                DateFrom = request.DateFrom ?? (object)DBNull.Value,
                DateTo = request.DateTo ?? (object)DBNull.Value,
                OrderGroupDtlData = orderGroupDtlData ?? (object)DBNull.Value,
                // No legacy single user properties
                UsrID = (object)DBNull.Value,
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var newGroupCode = result.GetOutputValue<int>("NewGroupCode");

            if (rowsAffected < 1)
            {
                _logger.LogWarning("Failed to create Order Group: {rowsAffected}", rowsAffected);
                throw new DomainException($"Failed to create Order Group rows Affected: {rowsAffected}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the created order group with nested structure
            var createdGroup = await GetOrderGroupByCodeAsync(newGroupCode, cancellationToken);
            return createdGroup ?? throw new DomainException("Failed to retrieve created Order Group");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Order Group");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to create Order Group: {ex.Message}");
        }
    }

    public async Task<OrderGroupDto> UpdateOrderGroupAsync(int groupCode, UpdateOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateUpdateRequestAsync(request, cancellationToken);

        _logger.LogInformation("Updating Order Group with nested users: {GroupCode}", groupCode);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Serialize nested users to JSON if available
            string? orderGroupDtlData = null;
            if (request.Users != null && request.Users.Any())
            {
                var userDataForJson = request.Users.Select(u => new
                {
                    UsrID = u.UsrID,
                    ViewOrder = u.ViewOrder ?? false,
                    PlaceOrder = u.PlaceOrder ?? false,
                    ViewClient = u.ViewClient ?? false,
                    ModifyOrder = u.ModifyOrder ?? false
                }).ToList();
                
                orderGroupDtlData = JsonSerializer.Serialize(userDataForJson);
                _logger.LogInformation("Serialized user data for Order Group update: {UserData}", orderGroupDtlData);
            }

            var parameters = new
            {
                Action = (int)ActionTypeEnum.UPDATE,
                GroupCode = groupCode,
                GroupDesc = request.GroupDesc,
                GroupType = request.GroupType,
                GroupValue = request.GroupValue,
                DateFrom = request.DateFrom ?? (object)DBNull.Value,
                DateTo = request.DateTo ?? (object)DBNull.Value,
                OrderGroupDtlData = orderGroupDtlData ?? (object)DBNull.Value,
                // No legacy single user properties
                UsrID = (object)DBNull.Value,
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0, // output param
                NewGroupCode = 0  // output param
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected < 0)
            {
                _logger.LogWarning("Failed to update Order Group rowsAffected: {rowsAffected}", rowsAffected);
                throw new DomainException($"Failed to update Order Group rows Affected: {rowsAffected}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the updated order group with nested structure
            var updatedGroup = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            return updatedGroup ?? throw new DomainException("Failed to retrieve updated Order Group");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Order Group");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to update Order Group: {ex.Message}");
        }
    }

    public async Task<bool> DeleteOrderGroupAsync(int groupCode, DeleteOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateDeleteRequestAsync(request, cancellationToken);

        _logger.LogInformation("Deleting Order Group: {GroupCode}", groupCode);

        // First check if group can be deleted (validation check)
        var validationResult = await CheckGroupDeleteValidationAsync(groupCode, request?.UsrID, cancellationToken);
        
        if (!validationResult.CanDelete)
        {
            if (validationResult.UserCount > 0)
            {
                throw new DomainException($"Cannot delete Order Group {groupCode}. It has {validationResult.UserCount} active users. Remove all users first.");
            }
            else
            {
                throw new DomainException($"Cannot delete Order Group {groupCode}. {validationResult.ValidationMessage}");
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // For legacy delete, we delete the entire group (master + all details)
            var parameters = new
            {
                Action = (int)ActionTypeEnum.DELETE,
                GroupCode = groupCode,
                GroupDesc = (object)DBNull.Value,
                GroupType = (object)DBNull.Value,
                GroupValue = (object)DBNull.Value,
                DateFrom = (object)DBNull.Value,
                DateTo = (object)DBNull.Value,
                UsrID = request.UsrID, // No specific user = delete entire group
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if ( rowsAffected < 1)
            {
                _logger.LogWarning("Failed to delete Order Group rowsAffected: {rowsAffected}", rowsAffected);
                throw new DomainException($"Failed to delete Order Group rowsAffected: {rowsAffected}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Order Group (Legacy)");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to delete Order Group: {ex.Message}");
        }
    }

    #endregion

    #region Work Flow Order Group 
    public async Task<PagedResult<OrderGroupDto>> GetOrderGroupWorkflowListAsync(GetOrderGroupWorkflowListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group Workflow List - Page: {PageNumber}, IsAuth: {IsAuth}, GroupCode: {GroupCode}", request.PageNumber, request.IsAuth, request.UsrID);

            var parameters = new
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SearchText = request.SearchTerm,
                UsrID = request.UsrID ?? (object)DBNull.Value,
                DateFromStart = request.DateFromStart ?? (object)DBNull.Value,
                DateFromEnd = request.DateFromEnd ?? (object)DBNull.Value,
                SortColumn = request.SortColumn,
                SortDirection = request.SortDirection,
                IsAuth = request.IsAuth,
                MakerId = _currentUserService.UserId,
                TotalCount = 0
            };

            var pagedResult = await _repository.QueryPagedAsync<OrderGroupDto>(
                "LB_SP_GetOrderGroupListWF",
                request.PageNumber,
                request.PageSize,
                parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Master Group Workflow List");
            throw new DomainException($"Failed to retrieve Order Group Workflow List: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeOrderGroupAsync(int groupCode, AuthorizeOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        _logger.LogInformation("Authorizing Order Group: {GroupCode}", groupCode);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["GroupCode"] = groupCode,
                ["ActionType"] = request.ActionType,
                ["IsAuth"] = request.IsAuth,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["AuthID"] = _currentUserService.UserId,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthOrderGroup", // Correct SP name for master authorization
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully authorized OrderGroup master: {GroupCode}", groupCode);
                return true;
            }
            else
            {
                throw new DomainException("Failed to authorize order group master - no rows affected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing OrderGroup master: {GroupCode}", groupCode);
            throw new DomainException($"Failed to authorize order group master: {ex.Message}");
        }
    }

    #endregion

    #region Work Flow Order Group User

    public async Task<PagedResult<OrderGroupUserDto>> GetOrderGroupUserWorkflowListAsync(GetOrderGroupUserWorkflowListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group User Workflow List - GroupCode: {GroupCode}, Page: {PageNumber}, IsAuth: {IsAuth}", request.GroupCode, request.PageNumber, request.IsAuth);

            var parameters = new
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SearchText = request.SearchTerm,
                GroupCode = request.GroupCode,
                UsrID = request.UsrID ?? (object)DBNull.Value,
                DateFromStart = request.DateFromStart ?? (object)DBNull.Value,
                DateFromEnd = request.DateFromEnd ?? (object)DBNull.Value,
                SortColumn = request.SortColumn,
                SortDirection = request.SortDirection,
                IsAuth = request.IsAuth,
                MakerId = _currentUserService.UserId,
                TotalCount = 0
            };

            var pagedResult = await _repository.QueryPagedAsync<OrderGroupUserDto>(
                "LB_SP_GetOrderGroupDtlListWF",
                request.PageNumber,
                request.PageSize,
                parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Detail Group Workflow List");
            throw new DomainException($"Failed to retrieve Order Group User Workflow List: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeOrderGroupUserAsync(int groupCode, string usrId, AuthorizeOrderGroupUserRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAuthorizeUserRequestAsync(request, cancellationToken);

        _logger.LogInformation("Authorizing OrderGroup User: {GroupCode}, {UsrID}", groupCode, usrId);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["GroupCode"] = groupCode,
                ["UsrID"] = usrId,
                ["ActionType"] = request.ActionType,
                ["IsAuth"] = request.IsAuth,
                ["IPAddress"] = _currentUserService.GetClientIPAddress(),
                ["AuthID"] = _currentUserService.UserId,
                ["Remarks"] = request.Remarks ?? (object)DBNull.Value,
                ["RowsAffected"] = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthOrderGroupDtl", // Correct SP name for detail authorization
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully authorized OrderGroup detail: {GroupCode}, {UsrID}", groupCode, usrId);
                return true;
            }
            else
            {
                throw new DomainException("Failed to authorize order group detail - no rows affected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing OrderGroup detail: {GroupCode}, {UsrID}", groupCode, usrId);
            throw new DomainException($"Failed to authorize order group detail: {ex.Message}");
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

            throw new ValidationException("Create Order Group validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Update Order Group validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Delete Order Group validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Authorize Order Group validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeUserRequestAsync(AuthorizeOrderGroupUserRequest request, CancellationToken cancellationToken)
    {
        // Basic validation for authorize user request
        if (string.IsNullOrEmpty(request.UsrID))
        {
            throw new ValidationException("User ID is required for authorization");
        }

        if (request.IsAuth < 0 || request.IsAuth > 2)
        {
            throw new ValidationException("Invalid authorization type");
        }

        if (request.IsAuth == (byte)AuthTypeEnum.Deny && string.IsNullOrEmpty(request.Remarks))
        {
            throw new ValidationException("Remarks are required when denying authorization");
        }
    }

    #endregion
}