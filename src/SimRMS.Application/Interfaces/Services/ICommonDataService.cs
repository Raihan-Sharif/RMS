using SimRMS.Application.Models.DTOs;

namespace SimRMS.Application.Interfaces.Services;

public interface ICommonDataService
{
    Task<IEnumerable<BranchListDto>> GetBranchListAsync(string? companyCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<CompanyListDto>> GetCompanyListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TraderListDto>> GetTraderListAsync(string? exchangeCode = null, CancellationToken cancellationToken = default);
}