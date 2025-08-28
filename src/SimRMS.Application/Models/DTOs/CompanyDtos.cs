/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company DTOs
/// Author:      Md. Raihan Sharif
/// Purpose:     Data Transfer Objects for Company operations (Read, Update, Authorization)
/// Creation:    12/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Models.DTOs
{
    public class CompanyDto : BaseEntityDto
    {
        public string CoCode { get; set; } = null!;
        public string? CoDesc { get; set; }
        //public long? TradingPolicy { get; set; }
        //public int? SenderType { get; set; }
        //public DateTime? UsrAccessLimitDefExprDate { get; set; }
        //public DateTime? UsrAccessLimitDefStartTime { get; set; }
        //public DateTime? UsrAccessLimitDefEndTime { get; set; }
        //public int? UsrAccessLimitDefDays { get; set; }
        public bool EnableExchangeWideSellProceed { get; set; }
    }
}