/// <summary>
/// <para>
/// ===================================================================
/// Title:       Action Type Enumeration
/// Author:      Md. Raihan Sharif
/// Purpose:     Defines action types for DML operations
/// Creation:    12/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Shared.Constants
{
    /// <summary>
    /// Represents different action types for database operations
    /// </summary>
    public enum ActionTypeEnum : byte
    {
        /// <summary>
        /// Insert operation
        /// </summary>
        INSERT = 1,

        /// <summary>
        /// Update operation
        /// </summary>
        UPDATE = 2,

        /// <summary>
        /// Delete operation
        /// </summary>
        DELETE = 3
    }
}