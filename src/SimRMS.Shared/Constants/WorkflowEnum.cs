/// <summary>
/// <para>
/// ===================================================================
/// Title:       Workflow Enumeration
/// Author:      Md. Raihan Sharif
/// Purpose:     Defines workflow names for workflow-level validation
/// Creation:    11/Nov/2025
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
    /// Represents workflow items for different modules or entities
    /// </summary>
    public enum WorkflowEnum : byte
    {
        /// <summary>
        /// Workflow for Broker Branch operations
        /// </summary>
        BrokerBranch = 1,

        /// <summary>
        /// Workflow for Traders data operations
        /// </summary>
        Traders = 2,

        /// <summary>
        /// Workflow for Company master data operations
        /// </summary>
        Company = 3,

        /// <summary>
        /// Workflow for User profile and management operations
        /// </summary>
        User = 4,

        /// <summary>
        /// Workflow for User Exposure related authorization operations
        /// </summary>
        UserExposure = 5,

        /// <summary>
        /// Workflow for User Group creation and management
        /// </summary>
        UserGroup = 6,

        /// <summary>
        /// Workflow for User Group Details operations
        /// </summary>
        UserGroupDetails = 7,

        /// <summary>
        /// Workflow for Client Information master data operations
        /// </summary>
        ClientInfo = 8,

        /// <summary>
        /// Workflow for Client Exposure authorization and checks
        /// </summary>
        ClientExposure = 9,

        /// <summary>
        /// Workflow for Stock Information master data management
        /// </summary>
        StockInfo = 10,

        /// <summary>
        /// Workflow for Client Share Information operations
        /// </summary>
        ClientShareInfo = 11,

        /// <summary>
        /// Workflow for Stock Control and inventory-related operations
        /// </summary>
        StockControl = 12
    }
}
