using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for Company with Exposure operations
    /// Handles business logic for combined MstCo and MstCoExps operations
    /// </summary>
    public interface ICompanyExposureService
    {
        // Query operations
        Task<PagedResult<MstCompanyWithExposureDto>> GetCompaniesWithExposurePagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<MstCompanyWithExposureDto?> GetCompanyWithExposureByCodeAsync(string coCode, CancellationToken cancellationToken = default);

        // Bulk operations using Table Value Parameters
        Task<ApiResponse<BulkOperationResult>> BulkCreateCompaniesWithExposureAsync(IEnumerable<CreateCompanyExposureRequest> requests, CancellationToken cancellationToken = default);
        Task<ApiResponse<BulkOperationResult>> BulkUpdateCompaniesWithExposureAsync(IEnumerable<UpdateCompanyExposureRequest> requests, CancellationToken cancellationToken = default);
        Task<ApiResponse<BulkOperationResult>> BulkUpsertCompaniesWithExposureAsync(IEnumerable<UpsertCompanyExposureRequest> requests, CancellationToken cancellationToken = default);
    }
}