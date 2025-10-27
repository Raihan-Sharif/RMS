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
using System.Data;
using System.Text.Json;
using LB.DAL.Core.Common;
using System;

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
            _logger.LogInformation("Getting Order Group(User Group) list - Page: {PageNumber}, Size: {PageSize}, Search: {SearchText}", pageNumber, pageSize, SearchText);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination and Sorting Parameters
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),
	            new LB_DALParam("SortColumn", "GroupDesc"),
	            new LB_DALParam("SortDirection", "ASC"),

                // Filter Parameters (Handling potential nulls)
                new LB_DALParam("SearchText", SearchText ?? (object)DBNull.Value),
	            new LB_DALParam("UsrId", (object)DBNull.Value),
            };

			// Get flat data from SP using proper DTO
			var flatResults = await _repository.QueryPagedAsync<OrderGroupCombinedDto>(
                "LB_SP_GetOrderGroupList",
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
                        Remarks = first.Remarks,
                        MakerId = first.MakerId,
                        TransDt = first.TransDt,
                        IsAuth = first.IsAuth,
                        IsDel =first.IsDel,
                        AuthLevel = first.AuthLevel,
                        AuthDt = first.AuthDt,
                        AuthTransDt = first.AuthTransDt,
                        RecordStatus = first.RecordStatus,
                        UserCount = first.UserCount,
                        Users = group.Where(x => !string.IsNullOrEmpty(x.UsrID))
                            .Select(user => new OrderGroupUserDto
                            {
                                GroupCode = first.GroupCode,
                                UsrID = user.UsrID ?? string.Empty,
                                UsrName = user.UsrName ?? string.Empty,
                                ViewOrder = user.ViewOrder ?? false,
                                PlaceOrder = user.PlaceOrder ?? false,
                                ViewClient = user.ViewClient ?? false,
                                ModifyOrder = user.ModifyOrder ?? false
                                //MakeBy = user.MakeBy,
                                //AuthBy = user.AuthBy
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
            _logger.LogError(ex, "An error occurred while retrieving Order Group(User Group) list");
            throw new DomainException($"Failed to retrieve Order Group(User Group) list: {ex.Message}");
        }
    }

    public async Task<OrderGroupUserDto?> GetOrderGroupUserByCodeAsync(int groupCode, string usrId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group(User Group) user by code: {GroupCode}, User: {UsrId}", groupCode, usrId);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("GroupCode", groupCode),
                new LB_DALParam("UsrId", usrId ?? (object)DBNull.Value),

                // Output Parameters (Status Code and Message)
                new LB_DALParam("statusCode", string.Empty, ParameterDirection.Output),
	            new LB_DALParam("statusMsg", string.Empty, ParameterDirection.Output)
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
            _logger.LogError(ex, "An error occurred while retrieving Order Group(User Group) by code: {GroupCode}", groupCode);
            throw new DomainException($"Failed to retrieve Order Group(User Group): {ex.Message}");
        }
    }

    public async Task<OrderGroupDto> GetOrderGroupByCodeAsync(int groupCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group(User Group) by code: {GroupCode}", groupCode);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
			{
                // Input Parameters
                new LB_DALParam("GroupCode", groupCode),
				new LB_DALParam("UsrId", (object)DBNull.Value),

                // Output Parameters (Status Code and Message)
                new LB_DALParam("statusCode", string.Empty, ParameterDirection.Output),
				new LB_DALParam("statusMsg", string.Empty, ParameterDirection.Output)
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
                        Remarks = first.Remarks,
                        MakerId = first.MakerId,
                        TransDt = first.TransDt,
                        IsAuth = first.IsAuth,
                        IsDel = first.IsDel,
                        AuthLevel = first.AuthLevel,
                        AuthDt = first.AuthDt,
                        AuthTransDt = first.AuthTransDt,
                        UserCount = first.UserCount,
                        Users = group.Where(x => !string.IsNullOrEmpty(x.UsrID))
                            .Select(user => new OrderGroupUserDto
                            {
                                GroupCode = first.GroupCode,
                                UsrID = user.UsrID ?? string.Empty,
                                UsrName = user.UsrName ?? string.Empty,
                                ViewOrder = user.ViewOrder ?? false,
                                PlaceOrder = user.PlaceOrder ?? false,
                                ViewClient = user.ViewClient ?? false,
                                ModifyOrder = user.ModifyOrder ?? false
                                //MakeBy = user.MakeBy,
                                //AuthBy = user.AuthBy
                            }).ToList()
                    };
                }).ToList();

            return groupedResults.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving Order Group(User Group) by code: {GroupCode}", groupCode);
            throw new DomainException($"Failed to retrieve Order Group(User Group): {ex.Message}");
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
            _logger.LogError(ex, "Error checking Order Group(User Group) existence: {GroupCode}", groupCode);
            return false;
        }
    }

    public async Task<OrderGroupDeleteResultDto> CheckGroupDeleteValidationAsync(int groupCode, string? usrId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking delete validation for Order Group(User Group): {GroupCode}", groupCode);

            // Get order group with all users to count them
            var orderGroups = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
          
            if (orderGroups == null)
            {
                return new OrderGroupDeleteResultDto
                {
                    GroupCode = groupCode,
                    UserCount = 0,
                    CanDelete = false,
                    ValidationMessage = $"Order Group(User Group) with code '{groupCode}' not found"
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
                    ? $"Cannot delete Order Group(User Group). Please remove all {userCount} user(s) first."
                    : "Order Group(User Group) can be deleted safely"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking delete validation for Order Group(User Group): {GroupCode}", groupCode);
            return new OrderGroupDeleteResultDto
            {
                GroupCode = groupCode,
                UserCount = -1,
                CanDelete = false,
                ValidationMessage = $"Error validating Order Group(User Group) deletion: {ex.Message}"
            };
        }
    }

    #endregion

    #region DML Operations (Unified CRUD Master and child table)

    public async Task<OrderGroupDto> CreateOrderGroupAsync(CreateOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateCreateRequestAsync(request, cancellationToken);

        _logger.LogInformation("Creating Order Group(User Group) with nested users: {GroupDesc}", request.GroupDesc);

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
                _logger.LogInformation("Serialized user data for Order Group(User Group): {UserData}", orderGroupDtlData);
            }

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Data Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.INSERT),
	            new LB_DALParam("GroupCode", (object)DBNull.Value),
                new LB_DALParam("GroupDesc", request.GroupDesc),
	            new LB_DALParam("GroupType", request.GroupType),
	            new LB_DALParam("GroupValue", request.GroupValue),

                // Date/Data Parameters with Null Checks
                new LB_DALParam("DateFrom", request.DateFrom ?? (object)DBNull.Value),
	            new LB_DALParam("DateTo", request.DateTo ?? (object)DBNull.Value),
	            new LB_DALParam("OrderGroupDtlData", orderGroupDtlData ?? (object)DBNull.Value), 

                new LB_DALParam("UsrID", (object)DBNull.Value),
	            new LB_DALParam("ViewOrder", (object)DBNull.Value),
	            new LB_DALParam("PlaceOrder", (object)DBNull.Value),
	            new LB_DALParam("ViewClient", (object)DBNull.Value),
	            new LB_DALParam("ModifyOrder", (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("TransDt", DateTime.Now),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value), // Added null check for robustness

                // Output Parameters
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output),
	            new LB_DALParam("NewGroupCode", 0, ParameterDirection.Output) // Likely returns the newly created ID
            };

			var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var newGroupCode = result.GetOutputValue<int>("NewGroupCode");

            if (rowsAffected < 1)
            {
                _logger.LogWarning("Failed to create Order Group(User Group): {rowsAffected}", rowsAffected);
                throw new DomainException($"Failed to create Order Group(User Group) rows Affected: {rowsAffected}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the created order group with nested structure
            var createdGroup = await GetOrderGroupByCodeAsync(newGroupCode, cancellationToken);
            return createdGroup ?? throw new DomainException("Failed to retrieve created Order Group(User Group)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating Order Group(User Group)");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to create Order Group(User Group): {ex.Message}");
        }
    }

    public async Task<OrderGroupDto> UpdateOrderGroupAsync(int groupCode, UpdateOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateUpdateRequestAsync(request, cancellationToken);

        _logger.LogInformation("Updating Order Group(User Group) with nested users: {GroupCode}", groupCode);

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
                _logger.LogInformation("Serialized user data for Order Group(User Group) update: {UserData}", orderGroupDtlData);
            }

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE),
	            new LB_DALParam("GroupCode", groupCode), // The key for UPDATE

                // Data Parameters
                new LB_DALParam("GroupDesc", request.GroupDesc ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("GroupType", request.GroupType ?? (object)DBNull.Value),
	            new LB_DALParam("GroupValue", request.GroupValue ?? (object)DBNull.Value),

                // Date/Detail Parameters
                new LB_DALParam("DateFrom", request.DateFrom ?? (object)DBNull.Value),
	            new LB_DALParam("DateTo", request.DateTo ?? (object)DBNull.Value),
	            new LB_DALParam("OrderGroupDtlData", orderGroupDtlData ?? (object)DBNull.Value),

                // Legacy/Unused/Defaulted Parameters
                new LB_DALParam("UsrID", (object)DBNull.Value),
	            new LB_DALParam("ViewOrder", (object)DBNull.Value),
	            new LB_DALParam("PlaceOrder", (object)DBNull.Value),
	            new LB_DALParam("ViewClient", (object)DBNull.Value),
	            new LB_DALParam("ModifyOrder", (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("TransDt", DateTime.Now),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameters
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output),
                // Although it's an UPDATE, sometimes the SP returns the ID, so treating as output based on the source:
                new LB_DALParam("NewGroupCode", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected < 0)
            {
                _logger.LogWarning("Failed to update Order Group(User Group) rowsAffected: {rowsAffected}", rowsAffected);
                throw new DomainException($"Failed to update Order Group(User Group) rows Affected: {rowsAffected}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Return the updated order group with nested structure
            var updatedGroup = await GetOrderGroupByCodeAsync(groupCode, cancellationToken);
            return updatedGroup ?? throw new DomainException("Failed to retrieve updated Order Group(User Group)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating Order Group(User Group)");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to update Order Group(User Group): {ex.Message}");
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
           
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.DELETE),
	            new LB_DALParam("GroupCode", groupCode), // The key for DELETE

                // Data/Detail Parameters (Set to DBNull as only the key is needed for deletion)
                new LB_DALParam("GroupDesc", (object)DBNull.Value),
	            new LB_DALParam("GroupType", (object)DBNull.Value),
	            new LB_DALParam("GroupValue", (object)DBNull.Value),
	            new LB_DALParam("DateFrom", (object)DBNull.Value),
	            new LB_DALParam("DateTo", (object)DBNull.Value),

                // User ID (Used to determine scope of deletion: entire group or specific user from group)
                new LB_DALParam("UsrID", request.UsrID ?? (object)DBNull.Value),

                // Legacy/Unused Parameters (Explicitly set to DBNull.Value)
                new LB_DALParam("ViewOrder", (object)DBNull.Value),
	            new LB_DALParam("PlaceOrder", (object)DBNull.Value),
	            new LB_DALParam("ViewClient", (object)DBNull.Value),
	            new LB_DALParam("ModifyOrder", (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("TransDt", DateTime.Now),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value), // Added null check for safety

                // Output Parameters (RowsAffected is the main result for DELETE)
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output),
	            new LB_DALParam("NewGroupCode", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudOrderGroup",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if ( rowsAffected < 1)
            {
                _logger.LogWarning("Failed to delete Order Group(User Group) rowsAffected: {rowsAffected}", rowsAffected);
                throw new DomainException($"Failed to delete Order Group(User Group) rowsAffected: {rowsAffected}");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting Order Group(User Group) (Legacy)");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            if (ex is DomainException)
                throw;
            
            throw new DomainException($"Failed to delete Order Group(User Group): {ex.Message}");
        }
    }

    #endregion

    #region Work Flow Order Group 
    public async Task<PagedResult<OrderGroupDto>> GetOrderGroupWorkflowListAsync(GetOrderGroupWorkflowListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group(User Group) Workflow List - Page: {PageNumber}, IsAuth: {IsAuth}, GroupCode: {GroupCode}", request.PageNumber, request.IsAuth, request.UsrID);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", request.PageNumber),
	            new LB_DALParam("PageSize", request.PageSize),

                // Filter Parameters
                new LB_DALParam("SearchText", request.SearchTerm ?? (object)DBNull.Value), // Assuming SearchTerm can be null
                new LB_DALParam("DateFromStart", request.DateFromStart ?? (object)DBNull.Value),
	            new LB_DALParam("DateFromEnd", request.DateFromEnd ?? (object)DBNull.Value),

                // Sorting Parameters
                new LB_DALParam("SortColumn", request.SortColumn ?? (object)DBNull.Value),
	            new LB_DALParam("SortDirection", request.SortDirection ?? (object)DBNull.Value),

                // Control/Audit Parameters
                new LB_DALParam("IsAuth", request.IsAuth),
	            new LB_DALParam("MakerId", _currentUserService.UserId)
            };

			var pagedResult = await _repository.QueryPagedAsync<OrderGroupDto>(
                "LB_SP_GetMasterGroupListWF",
                parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving Order Group(User Group) Group Workflow List");
            throw new DomainException($"Failed to retrieve Order Group Workflow List: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeOrderGroupAsync(int groupCode, AuthorizeOrderGroupRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        _logger.LogInformation("Authorizing Order Group(User Group): {GroupCode}", groupCode);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Action and Key Parameters
                new LB_DALParam("Action", request.ActionType),
	            new LB_DALParam("GroupCode", groupCode),
	            new LB_DALParam("ActionType", request.ActionType), 
                // Authorization/Audit Parameters
                new LB_DALParam("IsAuth", request.IsAuth),
	            new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("AuthID", _currentUserService.UserId),
    
                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthOrderGroup", // Correct SP name for master authorization
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully authorized Order Group(User Group) Group: {GroupCode}", groupCode);
                return true;
            }
            else
            {
                throw new DomainException("Failed to authorize Order Group(User Group) Group - no rows affected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while authorizing Order Group(User Group) Group: {GroupCode}", groupCode);
            throw new DomainException($"Failed to authorize Order Group(User Group) Group: {ex.Message}");
        }
    }

    #endregion

    #region Work Flow Order Group User

    public async Task<PagedResult<OrderGroupUserDto>> GetOrderGroupUserWorkflowListAsync(GetOrderGroupUserWorkflowListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting Order Group(User Group)User Workflow List - GroupCode: {GroupCode}, Page: {PageNumber}, IsAuth: {IsAuth}", request.PageNumber, request.IsAuth);

			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", request.PageNumber),
	            new LB_DALParam("PageSize", request.PageSize),

                // Filter Parameters
                new LB_DALParam("SearchText", request.SearchTerm ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("UsrID", request.UsrID ?? (object)DBNull.Value),
	            new LB_DALParam("DateFromStart", request.DateFromStart ?? (object)DBNull.Value),
	            new LB_DALParam("DateFromEnd", request.DateFromEnd ?? (object)DBNull.Value),

                // Sorting Parameters
                new LB_DALParam("SortColumn", request.SortColumn ?? (object)DBNull.Value),
	            new LB_DALParam("SortDirection", request.SortDirection ?? (object)DBNull.Value),

                // Control/Audit Parameters
                new LB_DALParam("IsAuth", request.IsAuth),
	            new LB_DALParam("MakerId", _currentUserService.UserId)
            };

			var pagedResult = await _repository.QueryPagedAsync<OrderGroupUserDto>(
                "LB_SP_GetOrderGroupDtlListWF",
                parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving Detail Order Group(User Group) User Workflow List");
            throw new DomainException($"Failed to retrieve Order Group(User Group) User Workflow List: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeOrderGroupUserAsync(int groupCode, string usrId, AuthorizeOrderGroupUserRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAuthorizeUserRequestAsync(request, cancellationToken);

        _logger.LogInformation("Authorizing Order Group(User Group) User: {GroupCode}, {UsrID}", groupCode, usrId);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Action and Key Parameters
                new LB_DALParam("Action", request.ActionType),
	            new LB_DALParam("GroupCode", groupCode ),
                new LB_DALParam("UsrID", usrId ?? (object)DBNull.Value),         // Added null check for safety
                new LB_DALParam("ActionType", request.ActionType), // Redundant parameter, but included as per source

                // Authorization/Audit Parameters
                new LB_DALParam("IsAuth", request.IsAuth),
	            new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("AuthID", _currentUserService.UserId),

                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthOrderGroupDtl", // Correct SP name for detail authorization
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully authorized Order Group(User Group) User: {GroupCode}, {UsrID}", groupCode, usrId);
                return true;
            }
            else
            {
                throw new DomainException("Failed to authorize Order Group(User Group) User - no rows affected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while authorizing OrderGroup detail: {GroupCode}, {UsrID}", groupCode, usrId);
            throw new DomainException($"Failed to authorize Order Group(User Group) User: {ex.Message}");
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

            throw new ValidationException("Create Order Group(User Group) validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Update Order Group(User Group) validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Delete Order Group(User Group) validation failed") { ValidationErrors = errors };
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

            throw new ValidationException("Authorize Order Group(User Group) validation failed") { ValidationErrors = errors };
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