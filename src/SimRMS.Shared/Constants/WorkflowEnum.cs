/// <summary>
/// <para>
/// ===================================================================
/// Title:       Workflow Enumeration
/// Author:      Md. Raihan Sharif
/// Purpose:     Defines workflow names for authorization levels
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
    /// Represents different workflow names for DML operations
    /// </summary>
    public static class WorkflowEnum
    {
        /// <summary>
        /// Market Stock Company operations workflow
        /// </summary>
        public const string MSTCO = "MSTCo";

        /// <summary>
        /// User Information operations workflow
        /// </summary>
        public const string USRINFO = "UsrInfo";

        /// <summary>
        /// Company Exposure operations workflow
        /// </summary>
        public const string COMPANY_EXPOSURE = "CompanyExposure";

        /// <summary>
        /// General Master Data workflow
        /// </summary>
        public const string MASTER_DATA = "MasterData";

        /// <summary>
        /// Client Information workflow
        /// </summary>
        public const string CLIENT_INFO = "ClientInfo";

        /// <summary>
        /// Transaction workflow
        /// </summary>
        public const string TRANSACTION = "Transaction";
    }
}