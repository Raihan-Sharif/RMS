using System.ComponentModel.DataAnnotations;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       MstCompanyWithExposure Entity
/// Author:      Md. Raihan Sharif
/// Purpose:     Model representing the combined structure of MstCo and MstCoExps tables
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

namespace SimRMS.Domain.Entities
{
    /// <summary>
    /// Combined entity representing MstCo and MstCoExps tables for unified operations
    /// Useful for table value parameters and bulk operations
    /// </summary>
    public class MstCompanyWithExposure : BaseEntity
    {
        // MstCo fields
        [Required]
        [StringLength(5)]
        public string CoCode { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CoDesc { get; set; }

        public long? TradingPolicy { get; set; }

        public string? SenderType { get; set; }

        public int? UsrAccessLimitDefExprDate { get; set; }

        public DateTime? UsrAccessLimitDefStartTime { get; set; }

        public DateTime? UsrAccessLimitDefEndTime { get; set; }

        public int? UsrAccessLimitDefDays { get; set; }

        public bool? EnableExchangeWideSellProceed { get; set; }

        [Required]
        [StringLength(39)]
        public string IPAddress { get; set; } = string.Empty;

        [Required]
        public int MakerId { get; set; }

        [Required]
        public DateTime ActionDt { get; set; }

        [Required]
        public DateTime TransDt { get; set; }

        [Required]
        public byte ActionType { get; set; } = 1;

        public int? AuthId { get; set; }

        public DateTime? AuthDt { get; set; }

        public DateTime? AuthTransDt { get; set; }

        [Required]
        public byte IsAuth { get; set; } = 0;

        [Required]
        public byte AuthLevel { get; set; } = 1;

        [Required]
        public byte IsDel { get; set; } = 0;

        [StringLength(200)]
        public string? Remarks { get; set; }

        // MstCoExps fields
        public bool? CoExpsCkBuy { get; set; }

        public decimal? CoExpsBuyAmt { get; set; }

        public decimal? CoExpsBuyAmtTopUp { get; set; }

        public decimal? CoExpsBuyDayOrder { get; set; }

        public decimal? CoExpsBuyPrevDayOrder { get; set; }

        public bool? CoExpsCkSell { get; set; }

        public decimal? CoExpsSellAmt { get; set; }

        public decimal? CoExpsSellAmtTopUp { get; set; }

        public decimal? CoExpsSellDayOrder { get; set; }

        public decimal? CoExpsSellPrevDayOrder { get; set; }

        public bool? CoExpsCheckTotal { get; set; }

        public decimal? CoExpsTotalAmt { get; set; }

        public decimal? CoExpsTotalAmtTopUp { get; set; }

        public decimal? CoExpsTotalDayOrder { get; set; }

        public decimal? CoExpsTotalPrevDayOrder { get; set; }

        public bool? CoExpsCheckNet { get; set; }

        public decimal? CoExpsNetAmt { get; set; }

        public decimal? CoExpsNetAmtTopUp { get; set; }

        public decimal? CoExpsNetDayOrder { get; set; }

        public decimal? CoExpsNetPrevDayOrder { get; set; }

        public bool? CoExpsWithLimit { get; set; }

        public decimal? CoExpsAddLimitPctg { get; set; }

        public decimal? CoNormalUpLimitPctg { get; set; }

        public decimal? CoNormalLowLimitPctg { get; set; }

        public decimal? CoDBTUpLimitPctg { get; set; }

        public decimal? CoDBTLowLimitPctg { get; set; }

        public int? CoTradeStatus { get; set; }

        public decimal? CoBuyTrnxLimit { get; set; }

        public int? CoBuyLotLimit { get; set; }

        public int? CoBuyBidLimitNormal { get; set; }

        public int? CoBuyBidLimitOdd { get; set; }

        public int? CoBuyBidLimitNormalOdd { get; set; }

        public int? CoDBTBuyShrLimit { get; set; }

        public decimal? CoSellTrnxLimit { get; set; }

        public int? CoSellLotLimit { get; set; }

        public int? CoSellBidLimitNormal { get; set; }

        public int? CoSellBidLimitOdd { get; set; }

        public int? CoSellBidLimitNormalOdd { get; set; }

        public int? CoDBTSellShrLimit { get; set; }

        [StringLength(255)]
        public string? CoRemarks { get; set; }

        public bool? CoDBTCancel { get; set; }

        [StringLength(255)]
        public string? CoExpsRemarks { get; set; }

        public bool? CoExpsCheckIDSSGross { get; set; }

        public decimal? CoExpsIDSSGrossAmt { get; set; }

        public decimal? CoExpsIDSSGrossAmtTopUp { get; set; }

        public decimal? CoExpsIDSSGrossDayOrder { get; set; }

        public decimal? CoExpsIDSSGrossPrevDayOrder { get; set; }

        public bool? CoExpsCheckIDSS { get; set; }

        public decimal? CoExpsIDSSAmt { get; set; }

        public decimal? CoExpsIDSSAmtTopUp { get; set; }

        public decimal? CoExpsIDSSDayOrder { get; set; }

        public decimal? CoExpsIDSSPrevDayOrder { get; set; }
    }

    /// <summary>
    /// Lightweight DTO for table value parameter operations
    /// Contains only essential fields to minimize data transfer
    /// </summary>
    public class CompanyExposureTableParam
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