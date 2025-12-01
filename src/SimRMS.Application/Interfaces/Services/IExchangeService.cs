using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Exchange service interface
    /// </summary>
    public interface IExchangeService
    {
        //Read operations
        Task<PagedResult<ExchangeDto>> GetExchangeListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? searchColumn = null, string? xchgCode = null, string? sortDirection = null, CancellationToken cancellationToken = default);
        Task<ExchangeDto> GetExchangeByIdAsync(string xchgCode, int xchgPrefix, string brokerCode, CancellationToken cancellationToken = default);

        //CRUD operations
        Task<ExchangeDto> CreateExchangeAsync(CreateExchangeRequest request, CancellationToken cancellationToken = default);
        Task<ExchangeDto> UpdateExchangeAsync(string xchgCode, int xchgPrefix, string brokerCode, UpdateExchangeRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteExchangeAsync(string xchgCode, int xchgPrefix, string brokerCode, DeleteExchangeRequest request, CancellationToken cancellationToken = default);

        //Workflow operations
        Task<PagedResult<ExchangeDto>> GetExchangeUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? sortDirection = null, string? searchText = null, string? searchColumn = null, string? xchgCode = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeExchangeAsync(string xchgCode, int xchgPrefix, string brokerCode, AuthorizeExchangeRequest request, CancellationToken cancellationToken = default);

    }
}
