using System.ComponentModel.DataAnnotations;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company Exposure Request Models
/// Author:      Md. Raihan Sharif
/// Purpose:     These models are used for creating, updating, and upserting company exposure data.
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

namespace SimRMS.Application.Models.Requests
{
    /// <summary>
    /// Request model for creating company with exposure
    /// Only includes fields that exist in the MstCoWithExp table type
    /// </summary>
    public class CreateCompanyExposureRequest
    {
        /// <summary>
        /// Company Code - leave empty for auto-generation
        /// </summary>
        [StringLength(5)]
        public string? CoCode { get; set; }  // Made optional for auto-generation

        [StringLength(50)]
        public string? CoDesc { get; set; }

        public bool? EnableExchangeWideSellProceed { get; set; }

        [StringLength(200)]
        public string? Remarks { get; set; }
        
        // Note: Removed fields that don't exist in table type:
        // - TradingPolicy, CoExpsBuyAmt, CoExpsSellAmt, CoExpsTotalAmt, CoExpsNetAmt, CoTradeStatus
        // These should be handled separately if needed
    }

    /// <summary>
    /// Request model for updating company with exposure
    /// </summary>
    public class UpdateCompanyExposureRequest : CreateCompanyExposureRequest
    {
        // Inherits all fields from CreateCompanyExposureRequest
    }

    /// <summary>
    /// Request model for upsert (insert or update) operations
    /// </summary>
    public class UpsertCompanyExposureRequest : CreateCompanyExposureRequest
    {
        // Inherits all fields from CreateCompanyExposureRequest
    }
}