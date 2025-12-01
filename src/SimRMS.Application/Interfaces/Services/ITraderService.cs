using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Trader service interface
    /// </summary>
    public interface ITraderService
    {
        //Read operations
        Task<PagedResult<MstTraderDto>> GetMstTraderListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? searchColumn = null, string? xchgCode = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<MstTraderDto> GetMstTraderByIdAsync(string xchgCode, string dlrCode, CancellationToken cancellationToken = default);

        //CRUD operations
        Task<MstTraderDto> CreateMstTraderAsync(CreateMstTraderRequest request, CancellationToken cancellationToken = default);
        Task<MstTraderDto> UpdateMstTraderAsync(string xchgCode, string dlrCode, UpdateMstTraderRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteMstTraderAsync(string xchgCode, string dlrCode, DeleteMstTraderRequest request, CancellationToken cancellationToken = default);

        //Workflow operations
        Task<PagedResult<MstTraderDto>> GetTraderUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? sortDirection = null ,string? searchTerm = null, string? xchgCode = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeTraderAsync(string xchgCode, string dlrCode, AuthorizeMstTraderRequest request, CancellationToken cancellationToken = default);

    }
}