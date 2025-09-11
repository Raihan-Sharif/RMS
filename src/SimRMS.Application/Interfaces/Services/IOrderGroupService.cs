using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

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
        Task<PagedResult<OrderGroupDto>> GetOrderGroupListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Order Group by code - returns all users in the group (list format)
        /// Uses LB_SP_GetOrderGroupByCode with @usrId = NULL
        /// </summary>
        Task<IEnumerable<OrderGroupDto>> GetOrderGroupByCodeAsync(int groupCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Order Group by code and specific user - returns single record
        /// Uses LB_SP_GetOrderGroupByCode with specific @usrId
        /// </summary>
        Task<OrderGroupDto?> GetOrderGroupUserByCodeAsync(int groupCode, string usrId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if users exist in group - used for delete validation
        /// </summary>
        Task<OrderGroupDeleteResultDto> CheckGroupDeleteValidationAsync(int groupCode, CancellationToken cancellationToken = default);

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

        /// <summary>
        /// Authorize Order Group (unified authorization for master and detail)
        /// Uses LB_SP_AuthOrderGroup with Action type detection
        /// </summary>
        Task<bool> AuthorizeOrderGroupAsync(int groupCode, AuthorizeOrderGroupRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Master Group Workflow Operations

        /// <summary>
        /// Get master groups workflow list for authorization
        /// Uses LB_SP_GetMasterGroupListWF
        /// </summary>
        Task<PagedResult<OrderGroupDto>> GetMasterGroupWorkflowListAsync(GetMasterGroupWorkflowListRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authorize master group changes
        /// Uses LB_SP_AuthOrderGroup
        /// </summary>
        Task<bool> AuthorizeMasterGroupAsync(int groupCode, AuthorizeOrderGroupMasterRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Detail Group Workflow Operations

        /// <summary>
        /// Get detail/user workflow list for authorization
        /// Uses LB_SP_GetOrderGroupDtlListWF
        /// </summary>
        Task<PagedResult<OrderGroupDetailDto>> GetDetailGroupWorkflowListAsync(GetDetailGroupWorkflowListRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authorize detail/user changes
        /// Uses LB_SP_AuthOrderGroupDtl
        /// </summary>
        Task<bool> AuthorizeDetailGroupAsync(int groupCode, string usrId, AuthorizeOrderGroupDetailRequest request, CancellationToken cancellationToken = default);

        #endregion
    }
}