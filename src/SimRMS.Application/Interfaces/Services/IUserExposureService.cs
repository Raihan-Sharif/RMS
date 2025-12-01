using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// User Exposure service interface
    /// Business logic + validation for UserExposure operations
    /// </summary>
    public interface IUserExposureService
    {
        Task<PagedResult<UserExposureDto>> GetUserExposureListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? searchColumn = null, CancellationToken cancellationToken = default);
        Task<UserExposureDto?> GetUserExposureByIdAsync(string usrId, CancellationToken cancellationToken = default);
        Task<UserExposureDto> CreateUserExposureAsync(CreateUserExposureRequest request, CancellationToken cancellationToken = default);
        Task<UserExposureDto> UpdateUserExposureAsync(string usrId, UpdateUserExposureRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserExposureAsync(string usrId, DeleteUserExposureRequest request, CancellationToken cancellationToken = default);
        Task<bool> UserExposureExistsAsync(string usrId, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<UserExposureDto>> GetUserExposureUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeUserExposureAsync(string usrId, AuthorizeUserExposureRequest request, CancellationToken cancellationToken = default);
    }
}