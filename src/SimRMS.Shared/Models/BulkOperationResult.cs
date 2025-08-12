/// <summary>
/// <para>
/// ===================================================================
/// Title:       Bulk Operation Result Model
/// Author:      Md. Raihan Sharif
/// Purpose:     Model for results of bulk operations using table value parameters
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

namespace SimRMS.Shared.Models
{
    /// <summary>
    /// Result model for bulk operations with table value parameters
    /// </summary>
    public class BulkOperationResult
    {
        public int RowsAffected { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess => Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);
    }
}