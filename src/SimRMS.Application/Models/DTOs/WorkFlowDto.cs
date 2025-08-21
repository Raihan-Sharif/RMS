using System.ComponentModel.DataAnnotations;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       WorkFlow Data Transfer Objects
/// Author:      Raihan Sharif
/// Purpose:     DTOs for workflow management (pending authorizations and denied items)
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

namespace SimRMS.Application.Models.DTOs;

public class WorkFlowItemDto
{
    public int TotalRecords { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Module { get; set; } = string.Empty;
    
    public int WFTypeID { get; set; }
    
    [Required]
    [StringLength(100)]
    public string WFTypeName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string WFUrl { get; set; } = string.Empty;
}

public class WorkFlowSummaryDto
{
    public int TotalUnauthorized { get; set; }
    public int TotalDenied { get; set; }
    public IEnumerable<WorkFlowItemDto> UnauthorizedItems { get; set; } = new List<WorkFlowItemDto>();
    public IEnumerable<WorkFlowItemDto> DeniedItems { get; set; } = new List<WorkFlowItemDto>();
}