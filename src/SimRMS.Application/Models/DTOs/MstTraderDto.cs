/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Trader DTOs
/// Author:      Asif Zaman
/// Purpose:     Data Transfer Objects for Market Stock Trader operations
/// Creation:    20/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// Asif Zaman      20/Aug/2025   Initial creation of MstTraderDto and MstTraderSearchDto
/// 
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Models.DTOs
{
    public class MstTraderDto : BaseEntityDto
    {
        public string XchgCode { get; set; } = string.Empty;
        public string DlrCode { get; set; } = string.Empty;
        public int XchgPrefix { get; set; }
        public string BrokerCode { get; set; } = string.Empty;
    }
}