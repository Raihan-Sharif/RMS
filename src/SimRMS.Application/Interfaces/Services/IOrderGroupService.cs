using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Order Group service interface
    /// Business logic + validation for OrderGroup operations
    /// </summary>
    public interface IOrderGroupService
    {
        Task<PagedResult<OrderGroupDto>> GetOrderGroupListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<OrderGroupDto?> GetOrderGroupByCodeAsync(int groupCode, CancellationToken cancellationToken = default);
        Task<OrderGroupDto> CreateOrderGroupAsync(CreateOrderGroupRequest request, CancellationToken cancellationToken = default);
        Task<OrderGroupDto> UpdateOrderGroupAsync(int groupCode, UpdateOrderGroupRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteOrderGroupAsync(int groupCode, DeleteOrderGroupRequest request, CancellationToken cancellationToken = default);
        Task<bool> OrderGroupExistsAsync(int groupCode, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<OrderGroupDto>> GetOrderGroupUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeOrderGroupAsync(int groupCode, AuthorizeOrderGroupRequest request, CancellationToken cancellationToken = default);
    }
}