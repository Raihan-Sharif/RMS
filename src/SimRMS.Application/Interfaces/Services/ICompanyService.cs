using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Company service interface
    /// Business logic + validation for Company operations (Read, Update, Authorization)
    /// </summary>
    public interface ICompanyService
    {
        Task<PagedResult<CompanyDto>> GetCompanyListAsync(int pageNumber = 1, int pageSize = 10, string? searchText = null, string? SearchColumn = null,  string? coCode = null, CancellationToken cancellationToken = default);
        Task<CompanyDto?> GetCompanyByIdAsync(string coCode, CancellationToken cancellationToken = default);
        Task<CompanyDto> UpdateCompanyAsync(string coCode, UpdateCompanyRequest request, CancellationToken cancellationToken = default);

        // Work Flow methods
        Task<PagedResult<CompanyDto>> GetCompanyUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? coCode = null, int isAuth = 0, CancellationToken cancellationToken = default);
        Task<bool> AuthorizeCompanyAsync(string coCode, AuthorizeCompanyRequest request, CancellationToken cancellationToken = default);
    }
}