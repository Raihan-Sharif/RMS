using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Client service interface
    /// Business logic + validation for Client operations
    /// </summary>
    public interface IClientService
    {
        Task<PagedResult<ClientDto>> GetClientListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? gcif = null, string? clntName = null, string? clntCode = null, CancellationToken cancellationToken = default);
        Task<ClientDto?> GetClientByIdAsync(string gcif, CancellationToken cancellationToken = default);
        Task<ClientDto> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default);
        Task<ClientDto> UpdateClientAsync(string gcif, UpdateClientRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteClientAsync(string gcif, DeleteClientRequest request, CancellationToken cancellationToken = default);
        Task<bool> ClientExistsAsync(string gcif, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<ClientDto>> GetClientUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? gcif = null, string? clntName = null, string? clntCode = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeClientAsync(string gcif, AuthorizeClientRequest request, CancellationToken cancellationToken = default);
    }
}