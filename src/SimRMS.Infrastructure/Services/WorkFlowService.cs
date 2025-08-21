using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Domain.Exceptions;
using SimRMS.Application.Interfaces;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       WorkFlow Service Implementation
/// Author:      Raihan Sharif
/// Purpose:     Service for managing workflow operations (pending authorizations and denied items)
/// Creation:    21/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

public class WorkFlowService : IWorkFlowService
{
    private readonly IGenericRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<WorkFlowService> _logger;

    public WorkFlowService(
        IGenericRepository repository,
        ICurrentUserService currentUserService,
        ILogger<WorkFlowService> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IEnumerable<WorkFlowItemDto>> GetPendingAuthorizationsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting pending authorization workflow items for user: {UserId}", _currentUserService.UserId);

        try
        {
            var parameters = new
            {
                MakerID = _currentUserService.UserId,
                IsAuth = (byte)0 // Unauthorized
            };

            var result = await _repository.QueryAsync<WorkFlowItemDto>(
                sqlOrSp: "LB_sp_sys_WFGet",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved {Count} pending authorization items", result.Count());
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending authorization workflow items");
            throw new DomainException($"Failed to retrieve pending authorization items: {ex.Message}");
        }
    }

    public async Task<IEnumerable<WorkFlowItemDto>> GetDeniedItemsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting denied workflow items for user: {UserId}", _currentUserService.UserId);

        try
        {
            var parameters = new
            {
                MakerID = _currentUserService.UserId,
                IsAuth = (byte)2 // Denied
            };

            var result = await _repository.QueryAsync<WorkFlowItemDto>(
                sqlOrSp: "LB_sp_sys_WFGet",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved {Count} denied items", result.Count());
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting denied workflow items");
            throw new DomainException($"Failed to retrieve denied items: {ex.Message}");
        }
    }

    public async Task<WorkFlowSummaryDto> GetWorkFlowSummaryAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting combined workflow summary for user: {UserId}", _currentUserService.UserId);

        try
        {
            // Execute sequentially to avoid database connection conflicts
            var unauthorizedItems = await GetPendingAuthorizationsAsync(cancellationToken);
            var deniedItems = await GetDeniedItemsAsync(cancellationToken);

            var summary = new WorkFlowSummaryDto
            {
                TotalUnauthorized = unauthorizedItems.Sum(x => x.TotalRecords),
                TotalDenied = deniedItems.Sum(x => x.TotalRecords),
                UnauthorizedItems = unauthorizedItems,
                DeniedItems = deniedItems
            };

            _logger.LogDebug("Retrieved workflow summary: {UnauthorizedCount} unauthorized items, {DeniedCount} denied items", 
                summary.TotalUnauthorized, summary.TotalDenied);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow summary");
            throw new DomainException($"Failed to retrieve workflow summary: {ex.Message}");
        }
    }
}