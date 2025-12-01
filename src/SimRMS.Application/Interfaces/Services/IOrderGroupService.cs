using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Order Group Service Interface (Master-Detail Architecture)
/// Author:      Raihan Sharif
/// Purpose:     This Interface provides methods definitions for managing Order Group master-detail operations
/// Creation:    11/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
/// ===================================================================
/// </para>
/// </summary>


namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Order Group service interface for master-detail operations
    /// Uses single LB_SP_CrudOrderGroup for all CRUD operations
    /// Separate workflows for master and details
    /// </summary>
    public interface IOrderGroupService
    {
        #region Common Operations

        /// <summary>
        /// Get Order Group list with nested user data structure
        /// Uses LB_SP_GetOrderGroupList and groups results
        /// </summary>
        Task<PagedResult<OrderGroupDto>> GetOrderGroupListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? searchColumn = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Order Group by code - returns all users in the group (list format)
        /// Uses LB_SP_GetOrderGroupByCode with @usrId = NULL
        /// </summary>
        Task<OrderGroupDto> GetOrderGroupByCodeAsync(int groupCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Order Group by code and specific user - returns single record
        /// Uses LB_SP_GetOrderGroupByCode with specific @usrId
        /// </summary>
        Task<OrderGroupUserDto?> GetOrderGroupUserByCodeAsync(int groupCode, string usrId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if users exist in group - used for delete validation
        /// </summary>
        Task<OrderGroupDeleteResultDto> CheckGroupDeleteValidationAsync(int groupCode, string? usrId = null, CancellationToken cancellationToken = default);

        #endregion

        #region Master-Detail CRUD Operations (Single SP)

        /// <summary>
        /// Create Order Group (master only or master + user)
        /// Uses LB_SP_CrudOrderGroup with Action = 1
        /// </summary>
        Task<OrderGroupDto> CreateOrderGroupAsync(CreateOrderGroupRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update Order Group (master only or master + user)
        /// Uses LB_SP_CrudOrderGroup with Action = 2
        /// </summary>
        Task<OrderGroupDto> UpdateOrderGroupAsync(int groupCode, UpdateOrderGroupRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete Order Group (master only or master + user)
        /// Uses LB_SP_CrudOrderGroup with Action = 3
        /// </summary>
        Task<bool> DeleteOrderGroupAsync(int groupCode, DeleteOrderGroupRequest request, CancellationToken cancellationToken = default);


        #endregion

        #region Order Group Workflow Operations

        /// <summary>
        /// Get Order groups workflow list for authorization
        /// Uses LB_SP_GetOrderGroupListWF
        /// </summary>
        Task<PagedResult<OrderGroupDto>> GetOrderGroupWorkflowListAsync(GetOrderGroupWorkflowListRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authorize Order group changes
        /// Uses LB_SP_AuthOrderGroup
        /// </summary>
        Task<bool> AuthorizeOrderGroupAsync(int groupCode, AuthorizeOrderGroupRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Detail Group Workflow Operations

        /// <summary>
        /// Get Order Group User workflow list for authorization
        /// Uses LB_SP_GetOrderGroupDtlListWF
        /// </summary>
        Task<PagedResult<OrderGroupUserDto>> GetOrderGroupUserWorkflowListAsync(GetOrderGroupUserWorkflowListRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authorize Order Group User changes
        /// Uses LB_SP_AuthOrderGroupDtl
        /// </summary>
        Task<bool> AuthorizeOrderGroupUserAsync(int groupCode, string usrId, AuthorizeOrderGroupUserRequest request, CancellationToken cancellationToken = default);

        #endregion

      
    }
}