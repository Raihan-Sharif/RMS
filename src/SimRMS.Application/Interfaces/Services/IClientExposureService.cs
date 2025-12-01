using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Client Exposure service interface
    /// Business logic + validation for ClientExposure operations
    /// </summary>
    public interface IClientExposureService
    {
        Task<PagedResult<ClientExposureDto>> GetClientExposureListAsync(int pageNumber = 1, int pageSize = 10, string? clntCode = null, string? coBrchCode = null, string? searchText = null, string? searchColumn = null, CancellationToken cancellationToken = default);
        Task<ClientExposureDto?> GetClientExposureByIdAsync(string clntCode, string coBrchCode, CancellationToken cancellationToken = default);
        Task<ClientExposureDto> UpdateClientExposureAsync(string clntCode, string coBrchCode, UpdateClientExposureRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteClientExposureAsync(string clntCode, string coBrchCode, DeleteClientExposureRequest request, CancellationToken cancellationToken = default);
        Task<bool> ClientExposureExistsAsync(string clntCode, string coBrchCode, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<ClientExposureDto>> GetClientExposureUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? clntCode = null, string? coBrchCode = null, string? searchTerm = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeClientExposureAsync(string clntCode, string coBrchCode, AuthorizeClientExposureRequest request, CancellationToken cancellationToken = default);
    }
}