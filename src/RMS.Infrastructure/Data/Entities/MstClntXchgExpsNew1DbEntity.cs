using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstClntXchgExpsNew1DbEntity
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public decimal? Percentage { get; set; }

    public decimal? ClntBuyAmtTopUp { get; set; }

    public decimal? ClntSellAmtTopUp { get; set; }

    public decimal? ClntTotalAmtTopUp { get; set; }

    public decimal? ClntNettAmtTopUp { get; set; }

    public decimal? ClntBuyPrevDayOrder { get; set; }

    public decimal? ClntSellPrevDayOrder { get; set; }

    public decimal? ClntTotalPrevDayOrder { get; set; }

    public decimal? ClntNettPrevDayOrder { get; set; }

    public int? ClntTradeStatus { get; set; }

    public int? ClntTradeBuyBidLimit { get; set; }

    public int? ClntTradeSellBidLimit { get; set; }

    public int? EcosbuyBidLimit { get; set; }

    public int? EcossellBidLimit { get; set; }

    public int? ClntTradeAllowOrd { get; set; }

    public int? ClntTradeAllowMod { get; set; }

    public int? ClntTradeAllow { get; set; }

    public string? ClntExpsRemarks { get; set; }

    public string? ClntTradeRemarks { get; set; }

    public int? ClntGtcexpiryPeriod { get; set; }

    public string? ClntGtdmode { get; set; }

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? DlrNettAmt { get; set; }

    public decimal? ClntNettAmt { get; set; }

    public decimal? DlrEarmarkedAmt { get; set; }

    public decimal? DlrSoldAmt { get; set; }
}
