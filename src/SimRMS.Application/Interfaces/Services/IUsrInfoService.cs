using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Simplified UsrInfo service interface
    /// Business logic + validation in single place
    /// </summary>
    public interface IUsrInfoService
    {
        Task<PagedResult<UsrInfoDto>> GetUsersAsync(int pageNumber = 1, int pageSize = 10, string? usrStatus = null, string? coCode = null, string? dlrCode = null, string? rmsType = null, string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<UsrInfoDto?> GetUserByIdAsync(string usrId, CancellationToken cancellationToken = default);
        Task<UsrInfoDto> CreateUserAsync(UsrInfoRequest request, CancellationToken cancellationToken = default);
        Task<UsrInfoDto> UpdateUserAsync(string usrId, UsrInfoRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(string usrId, CancellationToken cancellationToken = default);
        Task<bool> UserExistsAsync(string usrId, CancellationToken cancellationToken = default);
        Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
    }
}