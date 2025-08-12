/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company Exposure DTOs
/// Author:      Md. Raihan Sharif
/// Purpose:     This class defines DTOs for company exposure data, combining MstCo and MstCoExps tables.
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

namespace SimRMS.Application.Models.DTOs
{
    /// <summary>
    /// DTO for combined Company and Exposure data - matches simplified query output
    /// </summary>
    public class MstCompanyWithExposureDto
    {
        public string CoCode { get; set; } = string.Empty;
        public string? CoDesc { get; set; }
        public bool? EnableExchangeWideSellProceed { get; set; }
        
        // Audit fields that are available in MstCo table
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime ActionDt { get; set; }
        public DateTime TransDt { get; set; }
        public byte ActionType { get; set; }
        public int? AuthId { get; set; }
        public DateTime? AuthDt { get; set; }
        public byte IsAuth { get; set; }
        public byte AuthLevel { get; set; }
        public byte IsDel { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Lightweight model for bulk operations using table value parameters
    /// </summary>
    public class CompanyExposureBulkDto
    {
        public string CoCode { get; set; } = string.Empty;
        public string? CoDesc { get; set; }
        public long? TradingPolicy { get; set; }
        public bool? EnableExchangeWideSellProceed { get; set; }
        public decimal? CoExpsBuyAmt { get; set; }
        public decimal? CoExpsSellAmt { get; set; }
        public decimal? CoExpsTotalAmt { get; set; }
        public decimal? CoExpsNetAmt { get; set; }
        public int? CoTradeStatus { get; set; }
        public byte ActionType { get; set; } = 1;
        public byte IsAuth { get; set; } = 0;
        public byte IsDel { get; set; } = 0;
        public string? Remarks { get; set; }
    }
}