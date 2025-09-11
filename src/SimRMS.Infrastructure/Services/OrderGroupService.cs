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
/// Creation:    10/Sep/2025
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
                                UsrID = user.UsrID ?? string.Empty,
                                ViewOrder = user.ViewOrder,
                                PlaceOrder = user.PlaceOrder,
                                ViewClient = user.ViewClient,
                                ModifyOrder = user.ModifyOrder,
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

    public async Task<OrderGroupDto?> GetOrderGroupUserByCodeAsync(int groupCode, string usrId, CancellationToken cancellationToken = default)
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

            var orderGroups = await _repository.QueryAsync<OrderGroupDto>(
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

    public async Task<IEnumerable<OrderGroupDto>> GetOrderGroupByCodeAsync(int groupCode, CancellationToken cancellationToken = default)
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
                                UsrID = user.UsrID ?? string.Empty,
                                ViewOrder = user.ViewOrder,
                                PlaceOrder = user.PlaceOrder,
                                ViewClient = user.ViewClient,
                                ModifyOrder = user.ModifyOrder,
                                MakeBy = user.MakeBy,
                                AuthBy = user.AuthBy
                            }).ToList()
                    };
                }).ToList();

            return groupedResults;
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
            return result != null && result.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking OrderGroup existence: {GroupCode}", groupCode);
            return false;
        }
    }

    public async Task<OrderGroupDeleteResultDto> CheckGroupDeleteValidationAsync(int groupCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking delete validation for Order Group: {GroupCode}", groupCode);

            // Get order group with all users to count them
            var orderGroups = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            var groupList = orderGroups?.ToList();

            if (groupList == null || !groupList.Any())
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
            var userCount = groupList.SelectMany(g => g.Users).Count();

            return new OrderGroupDeleteResultDto
            {
                GroupCode = groupCode,
                UserCount = userCount,
                CanDelete = userCount == 0,
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

    #region Master Group Operations

    public async Task<OrderGroupDto> CreateOrderGroupMasterAsync(CreateOrderGroupMasterRequest request, CancellationToken cancellationToken = default)
    {
        // Note: Individual master/detail methods are deprecated in favor of unified CRUD operations
        throw new NotImplementedException("Use unified CreateOrderGroupAsync method instead");

        _logger.LogInformation("Creating Order Group Master: {GroupDesc}", request.GroupDesc);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var currentUser = _currentUserService.GetCurrentUserSession();
            
            var parameters = new
            {
                Action = (int)ActionTypeEnum.INSERT,
                GroupCode = (object)DBNull.Value,
                GroupDesc = request.GroupDesc,
                GroupType = request.GroupType,
                GroupValue = request.GroupValue,
                DateFrom = request.DateFrom ?? (object)DBNull.Value,
                DateTo = request.DateTo ?? (object)DBNull.Value,
                UsrID = (object)DBNull.Value, // No user for master-only creation
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = currentUser?.UserId ?? 0,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");
            var newGroupCode = result.GetOutputValue<int>("NewGroupCode");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to create Order Group Master: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to create Order Group Master: {statusMsg}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the created master
            var createdMaster = await GetCreatedMasterAsync(newGroupCode, cancellationToken);
            return createdMaster ?? throw new DomainException("Failed to retrieve created Order Group Master");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Order Group Master");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to create Order Group Master: {ex.Message}");
        }
    }

    private async Task<OrderGroupDto?> GetCreatedMasterAsync(int groupCode, CancellationToken cancellationToken)
    {
        try
        {
            var parameters = new
            {
                PageNumber = 1,
                PageSize = 1,
                SearchTerm = (object)DBNull.Value,
                UsrID = (object)DBNull.Value,
                DateFromStart = (object)DBNull.Value,
                DateFromEnd = (object)DBNull.Value,
                SortColumn = "GroupCode",
                SortDirection = "ASC",
                IsAuth = 0, // Get unauthorized records
                MakerId = _currentUserService.UserId
            };

            var masters = await _repository.QueryAsync<OrderGroupDto>(
                "LB_SP_GetMasterGroupListWF",
                parameters,
                isStoredProcedure: true,
                cancellationToken);

            return masters.FirstOrDefault(m => m.GroupCode == groupCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving created master group: {GroupCode}", groupCode);
            return null;
        }
    }

    public async Task<OrderGroupDto> UpdateOrderGroupMasterAsync(int groupCode, UpdateOrderGroupMasterRequest request, CancellationToken cancellationToken = default)
    {
        // Note: Individual master/detail methods are deprecated in favor of unified CRUD operations
        throw new NotImplementedException("Use unified UpdateOrderGroupAsync method instead");

        _logger.LogInformation("Updating Order Group Master: {GroupCode}", groupCode);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var parameters = new
            {
                Action = (int)ActionTypeEnum.UPDATE,
                GroupCode = groupCode,
                GroupDesc = request.GroupDesc,
                GroupType = request.GroupType,
                GroupValue = request.GroupValue,
                DateFrom = request.DateFrom ?? (object)DBNull.Value,
                DateTo = request.DateTo ?? (object)DBNull.Value,
                UsrID = (object)DBNull.Value, // No user for master-only update
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to update Order Group Master: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to update Order Group Master: {statusMsg}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the updated master
            var updatedMaster = await GetCreatedMasterAsync(groupCode, cancellationToken);
            return updatedMaster ?? throw new DomainException("Failed to retrieve updated Order Group Master");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Order Group Master");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to update Order Group Master: {ex.Message}");
        }
    }

    public async Task<bool> DeleteOrderGroupMasterAsync(int groupCode, DeleteOrderGroupMasterRequest request, CancellationToken cancellationToken = default)
    {
        // Note: Individual master/detail methods are deprecated in favor of unified CRUD operations
        throw new NotImplementedException("Use unified DeleteOrderGroupAsync method instead");

        _logger.LogInformation("Deleting Order Group Master: {GroupCode}", groupCode);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // First, validate if group can be deleted (check for existing users)
            var validationResult = await CheckGroupDeleteValidationAsync(groupCode, cancellationToken);
            
            if (!validationResult.CanDelete)
            {
                if (validationResult.UserCount == 0)
                {
                    throw new NotFoundException(validationResult.ValidationMessage!, groupCode);
                }
                else
                {
                    throw new DomainException(validationResult.ValidationMessage!);
                }
            }

            // For master deletion, we need to delete the group itself
            // The SP should handle cascading deletion of details if needed
            var parameters = new
            {
                Action = (int)ActionTypeEnum.DELETE,
                GroupCode = groupCode,
                GroupDesc = (object)DBNull.Value,
                GroupType = (object)DBNull.Value,
                GroupValue = (object)DBNull.Value,
                DateFrom = (object)DBNull.Value,
                DateTo = (object)DBNull.Value,
                UsrID = (object)DBNull.Value, // No specific user for master deletion
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to delete Order Group Master: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to delete Order Group Master: {statusMsg}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Order Group Master");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException or NotFoundException)
                throw;
            
            throw new DomainException($"Failed to delete Order Group Master: {ex.Message}");
        }
    }

    public async Task<PagedResult<OrderGroupDto>> GetMasterGroupWorkflowListAsync(GetMasterGroupWorkflowListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Master Group Workflow List - Page: {PageNumber}, IsAuth: {IsAuth}", request.PageNumber, request.IsAuth);

            var parameters = new
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SearchTerm = request.SearchTerm,
                UsrID = request.UsrID,
                DateFromStart = request.DateFromStart ?? (object)DBNull.Value,
                DateFromEnd = request.DateFromEnd ?? (object)DBNull.Value,
                SortColumn = request.SortColumn,
                SortDirection = request.SortDirection,
                IsAuth = request.IsAuth,
                MakerId = _currentUserService.UserId
            };

            var pagedResult = await _repository.QueryPagedAsync<OrderGroupDto>(
                "LB_SP_GetMasterGroupListWF",
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
            throw new DomainException($"Failed to retrieve Master Group Workflow List: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeMasterGroupAsync(int groupCode, AuthorizeOrderGroupMasterRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing OrderGroup master: {GroupCode}", groupCode);

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

    #region Detail/User Operations

    public async Task<OrderGroupDetailDto> AddUserToOrderGroupAsync(AddUserToOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        // Note: Individual master/detail methods are deprecated in favor of unified CRUD operations
        throw new NotImplementedException("Use unified CreateOrderGroupAsync or UpdateOrderGroupAsync methods instead");

        _logger.LogInformation("Adding User to Order Group: {GroupCode}, User: {UsrID}", request.GroupCode, request.UsrID);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var parameters = new
            {
                Action = (int)ActionTypeEnum.INSERT,
                GroupCode = request.GroupCode,
                GroupDesc = (object)DBNull.Value, // Not needed for user operations
                GroupType = (object)DBNull.Value,
                GroupValue = (object)DBNull.Value,
                DateFrom = (object)DBNull.Value,
                DateTo = (object)DBNull.Value,
                UsrID = request.UsrID,
                ViewOrder = request.ViewOrder ?? false,
                PlaceOrder = request.PlaceOrder ?? false,
                ViewClient = request.ViewClient ?? false,
                ModifyOrder = request.ModifyOrder ?? false,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to add user to Order Group: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to add user to Order Group: {statusMsg}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the created detail
            var createdDetail = await GetCreatedDetailAsync(request.GroupCode, request.UsrID, cancellationToken);
            return createdDetail ?? throw new DomainException("Failed to retrieve added user details");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user to Order Group");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to add user to Order Group: {ex.Message}");
        }
    }

    private async Task<OrderGroupDetailDto?> GetCreatedDetailAsync(int groupCode, string usrId, CancellationToken cancellationToken)
    {
        try
        {
            var parameters = new
            {
                PageNumber = 1,
                PageSize = 1,
                SearchTerm = usrId,
                GroupCode = groupCode,
                UsrID = usrId,
                DateFromStart = (object)DBNull.Value,
                DateFromEnd = (object)DBNull.Value,
                SortColumn = "UsrID",
                SortDirection = "ASC",
                IsAuth = 0, // Get unauthorized records
                MakerId = _currentUserService.UserId
            };

            var details = await _repository.QueryAsync<OrderGroupDetailDto>(
                "LB_SP_GetOrderGroupDtlListWF",
                parameters,
                isStoredProcedure: true,
                cancellationToken);

            return details.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving created detail: {GroupCode}, {UsrId}", groupCode, usrId);
            return null;
        }
    }

    public async Task<OrderGroupDetailDto> UpdateOrderGroupUserAsync(int groupCode, string usrId, UpdateOrderGroupUserRequest request, CancellationToken cancellationToken = default)
    {
        // Note: Individual master/detail methods are deprecated in favor of unified CRUD operations
        throw new NotImplementedException("Use unified UpdateOrderGroupAsync method instead");

        _logger.LogInformation("Updating Order Group User: {GroupCode}, User: {UsrID}", groupCode, usrId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var parameters = new
            {
                Action = (int)ActionTypeEnum.UPDATE,
                GroupCode = groupCode,
                GroupDesc = (object)DBNull.Value, // Not needed for user operations
                GroupType = (object)DBNull.Value,
                GroupValue = (object)DBNull.Value,
                DateFrom = (object)DBNull.Value,
                DateTo = (object)DBNull.Value,
                UsrID = usrId,
                ViewOrder = request.ViewOrder ?? false,
                PlaceOrder = request.PlaceOrder ?? false,
                ViewClient = request.ViewClient ?? false,
                ModifyOrder = request.ModifyOrder ?? false,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to update Order Group user: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to update Order Group user: {statusMsg}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the updated detail
            var updatedDetail = await GetCreatedDetailAsync(groupCode, usrId, cancellationToken);
            return updatedDetail ?? throw new DomainException("Failed to retrieve updated user details");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Order Group user");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to update Order Group user: {ex.Message}");
        }
    }

    public async Task<bool> RemoveUserFromGroupAsync(int groupCode, string usrId, RemoveUserFromGroupRequest request, CancellationToken cancellationToken = default)
    {
        // Note: Individual master/detail methods are deprecated in favor of unified CRUD operations
        throw new NotImplementedException("Use unified DeleteOrderGroupAsync method instead");

        _logger.LogInformation("Removing User from Order Group: {GroupCode}, User: {UsrId}", groupCode, usrId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var parameters = new
            {
                Action = (int)ActionTypeEnum.DELETE,
                GroupCode = groupCode,
                GroupDesc = (object)DBNull.Value,
                GroupType = (object)DBNull.Value,
                GroupValue = (object)DBNull.Value,
                DateFrom = (object)DBNull.Value,
                DateTo = (object)DBNull.Value,
                UsrID = usrId,
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to remove user from Order Group: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to remove user from Order Group: {statusMsg}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from Order Group");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to remove user from Order Group: {ex.Message}");
        }
    }

    public async Task<PagedResult<OrderGroupDetailDto>> GetDetailGroupWorkflowListAsync(GetDetailGroupWorkflowListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Detail Group Workflow List - GroupCode: {GroupCode}, Page: {PageNumber}, IsAuth: {IsAuth}", request.GroupCode, request.PageNumber, request.IsAuth);

            var parameters = new
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SearchTerm = request.SearchTerm,
                GroupCode = request.GroupCode,
                UsrID = request.UsrID,
                DateFromStart = request.DateFromStart ?? (object)DBNull.Value,
                DateFromEnd = request.DateFromEnd ?? (object)DBNull.Value,
                SortColumn = request.SortColumn,
                SortDirection = request.SortDirection,
                IsAuth = request.IsAuth,
                MakerId = _currentUserService.UserId
            };

            var pagedResult = await _repository.QueryPagedAsync<OrderGroupDetailDto>(
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
            throw new DomainException($"Failed to retrieve Detail Group Workflow List: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeDetailGroupAsync(int groupCode, string usrId, AuthorizeOrderGroupDetailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing OrderGroup detail: {GroupCode}, {UsrID}", groupCode, usrId);

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

    #region Legacy Support (for backward compatibility)

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
            var createdGroups = await GetOrderGroupByCodeAsync(newGroupCode, cancellationToken);
            var createdGroup = createdGroups?.FirstOrDefault();
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
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to update Order Group: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to update Order Group: {statusMsg}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the updated order group with nested structure
            var updatedGroups = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            var updatedGroup = updatedGroups?.FirstOrDefault();
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
        var validationResult = await CheckGroupDeleteValidationAsync(groupCode, cancellationToken);
        
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
                UsrID = (object)DBNull.Value, // No specific user = delete entire group
                ViewOrder = (object)DBNull.Value,
                PlaceOrder = (object)DBNull.Value,
                ViewClient = (object)DBNull.Value,
                ModifyOrder = (object)DBNull.Value,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                TransDt = DateTime.Now,
                Remarks = request.Remarks,
                RowsAffected = 0,
                StatusCode = 0,
                StatusMsg = "",
                NewGroupCode = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var statusCode = result.GetOutputValue<int>("StatusCode");
            var statusMsg = result.GetOutputValue<string>("StatusMsg");

            if (statusCode != 1 || rowsAffected == 0)
            {
                _logger.LogWarning("Failed to delete Order Group: {StatusMsg}", statusMsg);
                throw new DomainException($"Failed to delete Order Group: {statusMsg}");
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

    public async Task<PagedResult<OrderGroupDto>> GetOrderGroupUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int isAuth = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group Workflow List (Legacy) - Page: {PageNumber}, IsAuth: {IsAuth}", pageNumber, isAuth);

            // Use master workflow list for legacy support
            var masterRequest = new GetMasterGroupWorkflowListRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IsAuth = isAuth,
                MakerId = _currentUserService.UserId
            };

            var masterResult = await GetMasterGroupWorkflowListAsync(masterRequest, cancellationToken);

            // Convert master results to legacy DTO format
            var legacyItems = masterResult.Data.Select(m => new OrderGroupDto
            {
                GroupCode = m.GroupCode,
                GroupDesc = m.GroupDesc,
                GroupType = m.GroupType,
                GroupValue = m.GroupValue,
                DateFrom = m.DateFrom,
                DateTo = m.DateTo,
                GroupStatus = m.GroupStatus,
                AuthorizationStatus = m.AuthorizationStatus,
                RecordStatus = m.RecordStatus,
                MakeBy = m.MakeBy,
                AuthBy = m.AuthBy
            }).ToList();

            return new PagedResult<OrderGroupDto>
            {
                Data = legacyItems,
                TotalCount = masterResult.TotalCount,
                PageNumber = masterResult.PageNumber,
                PageSize = masterResult.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Order Group Workflow List (Legacy)");
            throw new DomainException($"Failed to retrieve Order Group Workflow List: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeOrderGroupAsync(int groupCode, AuthorizeOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        // For legacy support, delegate to master authorization
        var masterRequest = new AuthorizeOrderGroupMasterRequest
        {
            GroupCode = groupCode,
            ActionType = request.ActionType,
            IsAuth = request.IsAuth,
            Remarks = request.Remarks
        };

        return await AuthorizeMasterGroupAsync(groupCode, masterRequest, cancellationToken);
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

    #endregion
}