using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Models;

namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// Client Stock service interface
    /// Business logic + validation for Client Stock operations
    /// </summary>
    public interface IClientStockService
    {
        /// <summary>
        /// Get paginated list of client stocks with optional search and filtering
        /// </summary>
        Task<PagedResult<ClientStockDto>> GetClientStockListAsync(GetClientStockListRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get client stock by composite key (BranchCode, ClientCode, StockCode)
        /// </summary>
        Task<ClientStockDto?> GetClientStockByKeyAsync(GetClientStockByKeyRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create new client stock record
        /// </summary>
        Task<ClientStockDto> CreateClientStockAsync(CreateClientStockRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update existing client stock record
        /// </summary>
        Task<ClientStockDto> UpdateClientStockAsync(UpdateClientStockRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete client stock record (soft delete)
        /// </summary>
        Task<bool> DeleteClientStockAsync(DeleteClientStockRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if client stock record exists
        /// </summary>
        Task<bool> ClientStockExistsAsync(GetClientStockByKeyRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paginated list of unauthorized/pending client stock records for workflow
        /// </summary>
        Task<PagedResult<ClientStockDto>> GetClientStockWorkflowListAsync(GetClientStockWorkflowListRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authorize or deny client stock record
        /// </summary>
        Task<bool> AuthorizeClientStockAsync(AuthorizeClientStockRequest request, CancellationToken cancellationToken = default);
    }
}