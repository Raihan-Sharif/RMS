using SimRMS.Domain.Entities;
using SimRMS.Shared.Models;

namespace SimRMS.Domain.Interfaces.Repo
{
    /// <summary>
    /// Domain interface for UsrInfo repository - follows Clean Architecture
    /// Contains only domain-specific operations with no infrastructure dependencies
    /// </summary>
    public interface IUsrInfoRepository
    {
        // Basic CRUD operations
        Task<UsrInfo?> GetByUserIdAsync(string usrId, CancellationToken cancellationToken = default);
        Task<PagedResult<UsrInfo>> GetUsersPagedAsync(int pageNumber, int pageSize,
            string? usrStatus = null, string? coCode = null, string? dlrCode = null,
            string? rmsType = null, string? searchTerm = null,
            CancellationToken cancellationToken = default);
        Task<UsrInfo> CreateUserAsync(UsrInfo usrInfo, CancellationToken cancellationToken = default);
        Task<UsrInfo> UpdateUserAsync(UsrInfo usrInfo, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(string usrId, CancellationToken cancellationToken = default);

        // Business validation methods
        Task<bool> ExistsByEmailAsync(string email, string? excludeUsrId = null, CancellationToken cancellationToken = default);
        Task<Dictionary<string, object>> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
    }
}