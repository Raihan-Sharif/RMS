using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Stock Exposure service interface
    /// Business logic + validation for Stock Exposure operations
    /// </summary>
    public interface IStockExposureService
    {
        Task<PagedResult<StockExposureDto>> GetStockExposureListAsync(int pageNumber = 1, int pageSize = 10, string? usrID = null, string? clntCode = null, string? stkCode = null, string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<StockExposureDto?> GetStockExposureByKeyAsync(GetStockExposureByKeyRequest request, CancellationToken cancellationToken = default);
        Task<StockExposureDto> CreateStockExposureAsync(CreateStockExposureRequest request, CancellationToken cancellationToken = default);
        Task<StockExposureDto> UpdateStockExposureAsync(UpdateStockExposureRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteStockExposureAsync(DeleteStockExposureRequest request, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<StockExposureDto>> GetStockExposureUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? usrID = null, string? clntCode = null, string? stkCode = null, string? searchTerm = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeStockExposureAsync(AuthorizeStockExposureRequest request, CancellationToken cancellationToken = default);
    }
}
