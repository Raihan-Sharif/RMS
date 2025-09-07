/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Exposure DTOs
/// Author:      Raihan Sharif 
/// Purpose:     Data Transfer Objects for User Exposure operations
/// Creation:    04/Sep/2025
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
    public class UserExposureDto : BaseEntityDto
    {
        public string UsrId { get; set; } = null!;
        public bool UsrExpsCheckBuy { get; set; }
        public decimal UsrExpsBuyAmt { get; set; }
        public bool UsrExpsCheckSell { get; set; }
        public decimal UsrExpsSellAmt { get; set; }
        public bool UsrExpsCheckTotal { get; set; }
        public decimal UsrExpsTotalAmt { get; set; }
        public bool UsrExpsWithShrLimit { get; set; }
    }

    public class UserExposureUpdateDto
    {
        public bool? UsrExpsCheckBuy { get; set; }
        public decimal? UsrExpsBuyAmt { get; set; }
        public bool? UsrExpsCheckSell { get; set; }
        public decimal? UsrExpsSellAmt { get; set; }
        public bool? UsrExpsCheckTotal { get; set; }
        public decimal? UsrExpsTotalAmt { get; set; }
        public bool? UsrExpsWithShrLimit { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime TransDt { get; set; }
        public string? Remarks { get; set; }
    }

    public class UserExposureSearchDto
    {
        public string? UsrId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}