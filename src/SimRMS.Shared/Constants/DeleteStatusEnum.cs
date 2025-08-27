/// <summary>
/// <para>
/// ===================================================================
/// Title:       Delete Status Enumeration
/// Author:      Md. Raihan Sharif
/// Purpose:     Defines delete status for soft delete operations
/// Creation:    27/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Shared.Constants;

/// <summary>
/// Represents delete status for soft delete operations
/// </summary>
public enum DeleteStatusEnum : byte
{
    /// <summary>
    /// Record is active (not deleted)
    /// </summary>
    Active = 0,

    /// <summary>
    /// Record is deleted (soft delete)
    /// </summary>
    Deleted = 1
}