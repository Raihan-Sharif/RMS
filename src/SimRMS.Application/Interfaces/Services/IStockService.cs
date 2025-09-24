using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Stock service interface
    /// Business logic + validation for Stock operations
    /// </summary>
    public interface IStockService
    {
        Task<PagedResult<StockDto>> GetStockListAsync(int pageNumber = 1, int pageSize = 10, string? xchgCode = null, string? stkCode = null, string? searchTerm = null, string sortColumn = "StkCode", string sortDirection = "ASC", CancellationToken cancellationToken = default);
        Task<StockDto?> GetStockByKeyAsync(string xchgCode, string stkCode, CancellationToken cancellationToken = default);
        Task<StockDto> CreateStockAsync(CreateStockRequest request, CancellationToken cancellationToken = default);
        Task<StockDto> UpdateStockAsync(string xchgCode, string stkCode, UpdateStockRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteStockAsync(string xchgCode, string stkCode, DeleteStockRequest request, CancellationToken cancellationToken = default);
        Task<bool> StockExistsAsync(string xchgCode, string stkCode, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<StockDto>> GetStockUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, int isAuth = 0, string sortColumn = "StkCode", string sortDirection = "ASC", CancellationToken cancellationToken = default);
        Task<bool> AuthorizeStockAsync(string xchgCode, string stkCode, AuthorizeStockRequest request, CancellationToken cancellationToken = default);
    }
}