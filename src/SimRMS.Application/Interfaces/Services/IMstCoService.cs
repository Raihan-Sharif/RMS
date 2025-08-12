using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Market Stock Company service interface
    /// Business logic + validation for MstCo operations
    /// </summary>
    public interface IMstCoService
    {
        Task<PagedResult<MstCoDto>> GetMstCoListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? coCode = null, string? coDesc = null, CancellationToken cancellationToken = default);
        Task<MstCoDto?> GetMstCoByIdAsync(string coCode, CancellationToken cancellationToken = default);
        Task<MstCoDto> UpdateMstCoAsync(string coCode, UpdateMstCoRequest request, CancellationToken cancellationToken = default);
        Task<bool> MstCoExistsAsync(string coCode, CancellationToken cancellationToken = default);
    }
}