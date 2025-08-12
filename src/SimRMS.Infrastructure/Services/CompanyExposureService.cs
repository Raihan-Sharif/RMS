using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using System.Data;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company Exposure Service
/// Author:      Md. Raihan Sharif
/// Purpose:     Service implementation for Company with Exposure operations
/// Creation:    11/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Company with Exposure operations
    /// Demonstrates table value parameter usage with the custom LB DAL
    /// </summary>
    public class CompanyExposureService : ICompanyExposureService
    {
        private readonly IGenericRepository _repository;
        private readonly ILogger<CompanyExposureService> _logger;

        public CompanyExposureService(
            IGenericRepository repository,
            ILogger<CompanyExposureService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResult<MstCompanyWithExposureDto>> GetCompaniesWithExposurePagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting companies with exposure - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}", pageNumber, pageSize, searchTerm);

            // Simplified query to match available columns based on table type definition
            var sql = @"
                SELECT 
                    c.CoCode, c.CoDesc, c.EnableExchangeWideSellProceed,
                    c.IPAddress, c.MakerId, c.ActionDt, c.TransDt, c.ActionType,
                    c.AuthId, c.AuthDt, c.IsAuth, c.AuthLevel, c.IsDel, c.Remarks
                FROM MstCo c
                WHERE c.IsDel = 0";

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sql += " AND (c.CoCode LIKE @SearchTerm OR c.CoDesc LIKE @SearchTerm)";
                parameters["SearchTerm"] = $"%{searchTerm}%";
            }

            return await _repository.QueryPagedAsync<MstCompanyWithExposureDto>(
                sql, pageNumber, pageSize, parameters, "CoCode", false, cancellationToken);
        }

        public async Task<MstCompanyWithExposureDto?> GetCompanyWithExposureByCodeAsync(string coCode, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting company with exposure by code: {CoCode}", coCode);

            const string sql = @"
                SELECT 
                    c.CoCode, c.CoDesc, c.EnableExchangeWideSellProceed,
                    c.IPAddress, c.MakerId, c.ActionDt, c.TransDt, c.ActionType,
                    c.AuthId, c.AuthDt, c.IsAuth, c.AuthLevel, c.IsDel, c.Remarks
                FROM MstCo c
                WHERE c.CoCode = @CoCode AND c.IsDel = 0";

            return await _repository.QuerySingleAsync<MstCompanyWithExposureDto>(
                sql, new { CoCode = coCode }, false, cancellationToken);
        }

        public async Task<ApiResponse<BulkOperationResult>> BulkCreateCompaniesWithExposureAsync(IEnumerable<CreateCompanyExposureRequest> requests, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk creating companies with exposure: {Count}", requests.Count());

            try
            {
                var bulkData = requests.Select(r => MapToMstCoWithExpDto(r));
                var dataTable = _repository.ConvertToTableValueParameter(bulkData);
                
                // Log DataTable structure for debugging
                _logger.LogDebug("DataTable has {ColumnCount} columns: {Columns}", 
                    dataTable.Columns.Count, 
                    string.Join(", ", dataTable.Columns.Cast<System.Data.DataColumn>().Select(c => c.ColumnName)));
                
                var result = await _repository.ExecuteBulkStoredProcedureAsync(
                    "InsertMstCoWithExp",
                    dataTable,
                    "@InputData",  // Use proper parameter name with @
                    null,  // SP only accepts the table parameter, no additional params
                    cancellationToken);

                // Check if the operation actually succeeded
                if (result.IsSuccess)
                {
                    return ApiResponse<BulkOperationResult>.SuccessResult(result, "Bulk create operation completed successfully");
                }
                else
                {
                    // Operation failed - return error response
                    _logger.LogError("Bulk operation failed: {Message}", result.Message);
                    return new ApiResponse<BulkOperationResult>
                    {
                        Success = false,
                        Message = result.Message,
                        Data = result,
                        Errors = new List<string> { result.Message }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk create companies with exposure");
                var failedResult = new BulkOperationResult
                {
                    RowsAffected = 0,
                    Status = "ERROR",
                    Message = ex.Message
                };
                return new ApiResponse<BulkOperationResult>
                {
                    Success = false,
                    Message = $"An error occurred during bulk create operation: {ex.Message}",
                    Data = failedResult,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<BulkOperationResult>> BulkUpdateCompaniesWithExposureAsync(IEnumerable<UpdateCompanyExposureRequest> requests, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk updating companies with exposure: {Count}", requests.Count());

            try
            {
                var bulkData = requests.Select(r => MapToMstCoWithExpDto(r, actionType: 2)); // ActionType = 2 for Update
                var dataTable = _repository.ConvertToTableValueParameter(bulkData);
                
                var result = await _repository.ExecuteBulkStoredProcedureAsync(
                    "SP_BulkUpdateCompanyWithExposure",
                    dataTable,
                    "CompanyData",
                    GetCurrentUserParams(),
                    cancellationToken);

                return ApiResponse<BulkOperationResult>.SuccessResult(result, "Bulk update operation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk update companies with exposure");
                var failedResult = new BulkOperationResult
                {
                    RowsAffected = 0,
                    Status = "ERROR",
                    Message = ex.Message
                };
                return new ApiResponse<BulkOperationResult>
                {
                    Success = false,
                    Message = $"An error occurred during bulk update operation: {ex.Message}",
                    Data = failedResult,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<BulkOperationResult>> BulkUpsertCompaniesWithExposureAsync(IEnumerable<UpsertCompanyExposureRequest> requests, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bulk upserting companies with exposure: {Count}", requests.Count());

            try
            {
                var bulkData = requests.Select(r => MapToMstCoWithExpDto(r));
                var dataTable = _repository.ConvertToTableValueParameter(bulkData);
                
                var result = await _repository.ExecuteBulkStoredProcedureAsync(
                    "SP_BulkUpsertCompanyWithExposure",
                    dataTable,
                    "CompanyData",
                    GetCurrentUserParams(),
                    cancellationToken);

                return ApiResponse<BulkOperationResult>.SuccessResult(result, "Bulk upsert operation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk upsert companies with exposure");
                var failedResult = new BulkOperationResult
                {
                    RowsAffected = 0,
                    Status = "ERROR",
                    Message = ex.Message
                };
                return new ApiResponse<BulkOperationResult>
                {
                    Success = false,
                    Message = $"An error occurred during bulk upsert operation: {ex.Message}",
                    Data = failedResult,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private MstCoWithExpDto MapToMstCoWithExpDto(CreateCompanyExposureRequest request, byte actionType = 1)
        {
            return new MstCoWithExpDto
            {
                // Map only fields that exist in the table type (15 columns)
                CoCode = string.IsNullOrEmpty(request.CoCode) ? null : request.CoCode, // NULL for auto-generation
                CoDesc = request.CoDesc,
                EnableExchangeWideSellProceed = request.EnableExchangeWideSellProceed,
                IPAddress = "127.0.0.1", // Should come from current context
                MakerId = 1, // Should come from current user
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date,
                ActionType = actionType,
                AuthId = null,
                AuthDt = null,
                AuthTransDt = null,
                IsAuth = 0,
                AuthLevel = 1,
                IsDel = 0,
                Remarks = request.Remarks
                
                // Note: Ignoring fields from request that don't exist in table type:
                // - TradingPolicy, CoExpsBuyAmt, CoExpsSellAmt, CoExpsTotalAmt, CoExpsNetAmt, CoTradeStatus
            };
        }

        private object GetCurrentUserParams()
        {
            // In a real application, get these from current user context
            return new
            {
                MakerId = 1, // Get from current user
                IPAddress = "127.0.0.1", // Get from request context
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date
            };
        }
    }
}