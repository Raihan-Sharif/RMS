using SimRMS.Application.Models.DTOs;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       WorkFlow Service Interface
/// Author:      Raihan Sharif
/// Purpose:     Interface for workflow management operations
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

namespace SimRMS.Application.Interfaces.Services;

public interface IWorkFlowService
{
    Task<IEnumerable<WorkFlowItemDto>> GetPendingAuthorizationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkFlowItemDto>> GetDeniedItemsAsync(CancellationToken cancellationToken = default);
    Task<WorkFlowSummaryDto> GetWorkFlowSummaryAsync(CancellationToken cancellationToken = default);
}