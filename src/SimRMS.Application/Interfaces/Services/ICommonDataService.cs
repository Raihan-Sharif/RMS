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
    Task<IEnumerable<ClientListDto>> GetClientListAsync(CancellationToken cancellationToken = default);
}