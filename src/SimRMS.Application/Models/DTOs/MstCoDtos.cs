/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company DTOs
/// Author:      Md. Raihan Sharif
/// Purpose:     Data Transfer Objects for Market Stock Company operations
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

namespace SimRMS.Application.Models.DTOs
{
    public class MstCoDto
    {
        public string CoCode { get; set; } = null!;
        public string? CoDesc { get; set; }
        public long? TradingPolicy { get; set; }
        public int? SenderType { get; set; }
        public DateTime? UsrAccessLimitDefExprDate { get; set; }
        public DateTime? UsrAccessLimitDefStartTime { get; set; }
        public DateTime? UsrAccessLimitDefEndTime { get; set; }
        public int? UsrAccessLimitDefDays { get; set; }
        public bool EnableExchangeWideSellProceed { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime ActionDt { get; set; }
        public DateTime TransDt { get; set; }
        public byte ActionType { get; set; }
        public int? AuthId { get; set; }
        public DateTime? AuthDt { get; set; }
        public DateTime? AuthTransDt { get; set; }
        public byte IsAuth { get; set; }
        public byte AuthLevel { get; set; }
        public byte IsDel { get; set; }
        public string? Remarks { get; set; }
    }

    public class MstCoUpdateDto
    {
        public string CoCode { get; set; } = null!;
        public string? CoDesc { get; set; }
        public bool EnableExchangeWideSellProceed { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime TransDt { get; set; }
        public string? Remarks { get; set; }
    }

    public class MstCoSearchDto
    {
        public string? CoCode { get; set; }
        public string? CoDesc { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}