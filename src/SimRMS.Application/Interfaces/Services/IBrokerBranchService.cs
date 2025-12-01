using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Broker Branch service interface
    /// Business logic + validation for MstCoBrch operations
    /// </summary>
    public interface IBrokerBranchService
    {
        Task<PagedResult<MstCoBrchDto>> GetMstCoBrchListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? searchColumn = null, string? coCode = null, CancellationToken cancellationToken = default);
        Task<MstCoBrchDto?> GetMstCoBrchByIdAsync(string coCode, string coBrchCode, CancellationToken cancellationToken = default);
        Task<MstCoBrchDto> CreateMstCoBrchAsync(CreateMstCoBrchRequest request, CancellationToken cancellationToken = default);
        Task<MstCoBrchDto> UpdateMstCoBrchAsync(string coCode, string coBrchCode, UpdateMstCoBrchRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteMstCoBrchAsync(string coCode, string coBrchCode, DeleteMstCoBrchRequest request, CancellationToken cancellationToken = default);
        Task<bool> MstCoBrchExistsAsync(string coCode, string coBrchCode, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<MstCoBrchDto>> GetBranchUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? coCode = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeBranchAsync(string coCode, string coBrchCode, AuthorizeMstCoBrchRequest request, CancellationToken cancellationToken = default);
    }
}