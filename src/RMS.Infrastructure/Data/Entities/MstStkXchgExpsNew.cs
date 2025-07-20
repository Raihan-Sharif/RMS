using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkXchgExpsNew
{
    public string XchgCode { get; set; } = null!;

    public bool? XchgExpsCheckBuy { get; set; }

    public decimal? XchgExpsBuyAmt { get; set; }

    public decimal? XchgExpsBuyAmtTopUp { get; set; }

    public decimal? XchgExpsBuyDayOrder { get; set; }

    public decimal? XchgExpsBuyPrevDayOrder { get; set; }

    public bool? XchgExpsCheckSell { get; set; }

    public decimal? XchgExpsSellAmt { get; set; }

    public decimal? XchgExpsSellAmtTopUp { get; set; }

    public decimal? XchgExpsSellDayOrder { get; set; }

    public decimal? XchgExpsSellPrevDayOrder { get; set; }

    public bool? XchgExpsCheckTotal { get; set; }

    public decimal? XchgExpsTotalAmt { get; set; }

    public decimal? XchgExpsTotalAmtTopUp { get; set; }

    public decimal? XchgExpsTotalDayOrder { get; set; }

    public decimal? XchgExpsTotalPrevDayOrder { get; set; }

    public bool? XchgExpsCheckNet { get; set; }

    public decimal? XchgExpsNetAmt { get; set; }

    public decimal? XchgExpsNetAmtTopUp { get; set; }

    public decimal? XchgExpsNetDayOrder { get; set; }

    public decimal? XchgExpsNetPrevDayOrder { get; set; }

    public bool? XchgExpsWithLimit { get; set; }

    public decimal? XchgExpsAddLimitPctg { get; set; }

    public decimal? XchgNormalUpLimitPctg { get; set; }

    public decimal? XchgNormalLowLimitPctg { get; set; }

    public decimal? XchgDbtupLimitPctg { get; set; }

    public decimal? XchgDbtlowLimitPctg { get; set; }

    public int? XchgTradeStatus { get; set; }

    public decimal? XchgBuyTrnxLimit { get; set; }

    public int? XchgBuyLotLimit { get; set; }

    public int? XchgBuyBidLimitNormal { get; set; }

    public int? XchgBuyBidLimitOdd { get; set; }

    public int? XchgBuyBidLimitNormalOdd { get; set; }

    public int? XchgDbtbuyShrLimit { get; set; }

    public decimal? XchgSellTrnxLimit { get; set; }

    public int? XchgSellLotLimit { get; set; }

    public int? XchgSellBidLimitNormal { get; set; }

    public int? XchgSellBidLimitOdd { get; set; }

    public int? XchgSellBidLimitNormalOdd { get; set; }

    public int? XchgDbtsellShrLimit { get; set; }

    public string? XchgRemarks { get; set; }

    public bool? XchgDbtcancel { get; set; }

    public bool? XchgIrregularTrds { get; set; }

    public string? XchgExpsRemarks { get; set; }

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayNettAmt { get; set; }
}
