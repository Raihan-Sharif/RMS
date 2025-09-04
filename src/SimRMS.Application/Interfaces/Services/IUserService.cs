using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// User service interface
    /// </summary>
    public interface IUserService
    {
        #region User list operations
        Task<PagedResult<UserDto>> GetUserListAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchText = null,
            string? usrStatus = null,
            string? dlrCode = null,
            string? coCode = null,
            string? coBrchCode = null,
            string? sortColumn = "UsrID",
            string? sortDirection = "ASC",
            CancellationToken cancellationToken = default);

        Task<UserDetailDto?> GetUserByIdAsync(string usrId, CancellationToken cancellationToken = default);
        #endregion

        # region User CRUD operations
        Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<UserDto> UpdateUserAsync(string usrId, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(string usrId, DeleteUserRequest request, CancellationToken cancellationToken = default);
        #endregion

        #region User WF operations
        Task<PagedResult<UserDto>> GetUserUnAuthDeniedListAsync(
              int pageNumber = 1,
              int pageSize = 10,
              string? sortColumn = "UsrID",
              string? sortDirection = "ASC",
              string? searchText = null,
              string? usrStatus = null,
              string? dlrCode = null,
              string? coCode = null,
              string? coBrchCode = null,
              int isAuth = 0,
              CancellationToken cancellationToken = default);

        Task<bool> AuthorizeUserAsync(string usrId, AuthorizeUserRequest request, CancellationToken cancellationToken = default);
        #endregion

    }
}