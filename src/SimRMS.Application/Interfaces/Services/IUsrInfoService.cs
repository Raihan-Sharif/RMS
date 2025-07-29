using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for UsrInfo business operations
    /// Interface stays in Application layer, implementation goes to Infrastructure
    /// </summary>
    public interface IUsrInfoService
    {
        /// <summary>
        /// Get paginated list of users with filtering
        /// </summary>
        Task<PagedResult<UsrInfoDto>> GetUsersAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? usrStatus = null,
            string? coCode = null,
            string? dlrCode = null,
            string? rmsType = null,
            string? searchTerm = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get user by ID
        /// </summary>
        Task<UsrInfoDto> GetUserByIdAsync(string usrId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create new user - using single request model
        /// </summary>
        Task<UsrInfoDto> CreateUserAsync(UsrInfoRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update existing user - using single request model
        /// </summary>
        Task<UsrInfoDto> UpdateUserAsync(string usrId, UsrInfoRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete user
        /// </summary>
        Task<bool> DeleteUserAsync(string usrId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if user exists
        /// </summary>
        Task<bool> UserExistsAsync(string usrId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get user statistics
        /// </summary>
        Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
    }
}