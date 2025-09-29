using SimRMS.Application.Models.DTOs;

namespace SimRMS.Application.Interfaces.Services;

public interface ICommonDataService
{
    Task<IEnumerable<BranchListDto>> GetBranchListAsync(string? companyCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<CompanyListDto>> GetCompanyListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TraderListDto>> GetTraderListAsync(string? exchangeCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserListDto>> GetUserListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CountryListDto>> GetCountryListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientTypeListDto>> GetClientTypeListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientListDto>> GetClientListAsync(string? branchCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockExchangeListDto>> GetStockExchangeListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockBoardListDto>> GetStockBoardListAsync(string? exchangeCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockBoardMarketListDto>> GetStockBoardMarketListAsync(string? exchangeCode = null, string? boardCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockListDto>> GetStockListAsync(string? exchangeCode = null, string? boardCode = null, string? sectorCode = null, CancellationToken cancellationToken = default);
}