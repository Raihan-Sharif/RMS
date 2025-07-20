using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCoBrchExpsNewDbEntity
{
    public string CoCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public bool? CoBrchExpsCheckBuy { get; set; }

    public decimal? CoBrchExpsBuyAmt { get; set; }

    public decimal? CoBrchExpsBuyAmtTopUp { get; set; }

    public decimal? CoBrchExpsBuyDayOrder { get; set; }

    public decimal? CoBrchExpsBuyPrevDayOrder { get; set; }

    public bool? CoBrchExpsCheckSell { get; set; }

    public decimal? CoBrchExpsSellAmt { get; set; }

    public decimal? CoBrchExpsSellAmtTopUp { get; set; }

    public decimal? CoBrchExpsSellDayOrder { get; set; }

    public decimal? CoBrchExpsSellPrevDayOrder { get; set; }

    public bool? CoBrchExpsCheckTotal { get; set; }

    public decimal? CoBrchExpsTotalAmt { get; set; }

    public decimal? CoBrchExpsTotalAmtTopUp { get; set; }

    public decimal? CoBrchExpsTotalDayOrder { get; set; }

    public decimal? CoBrchExpsTotalPrevDayOrder { get; set; }

    public bool? CoBrchExpsCheckNet { get; set; }

    public decimal? CoBrchExpsNetAmt { get; set; }

    public decimal? CoBrchExpsNetAmtTopUp { get; set; }

    public decimal? CoBrchExpsNetDayOrder { get; set; }

    public decimal? CoBrchExpsNetPrevDayOrder { get; set; }

    public bool? CoBrchExpsWithLimit { get; set; }

    public decimal? CoBrchExpsAddLimitPctg { get; set; }

    public int? CoBrchTradeStatus { get; set; }

    public decimal? CoBrchBuyTrnxLimit { get; set; }

    public int? CoBrchBuyLotLimit { get; set; }

    public int? CoBrchBuyBidLimit { get; set; }

    public decimal? CoBrchSellTrnxLimit { get; set; }

    public int? CoBrchSellLotLimit { get; set; }

    public int? CoBrchSellBidLimit { get; set; }

    public string? CoBrchRemarks { get; set; }

    public bool? CoBrchDbtcancel { get; set; }

    public string? CoBrchExpsRemarks { get; set; }

    public decimal? CoBrchNormalUpLimitPctg { get; set; }

    public decimal? CoBrchNormalLowLimitPctg { get; set; }

    public decimal? CoBrchDbtupLimitPctg { get; set; }

    public decimal? CoBrchDbtlowLimitPctg { get; set; }

    public int? CoBrchBuyDbtshrLimit { get; set; }

    public int? CoBrchSellDbtshrLimit { get; set; }

    public decimal? CoBrchClntBuyTlimit { get; set; }

    public decimal? CoBrchClntSellTlimit { get; set; }

    public decimal? CoBrchClntTotalTlimit { get; set; }

    public decimal? CoBrchClntNettTlimit { get; set; }

    public decimal? CoBrchUsrBuyTlimit { get; set; }

    public decimal? CoBrchUsrSellTlimit { get; set; }

    public decimal? CoBrchUsrTotalTlimit { get; set; }

    public decimal? CoBrchUsrNettTlimit { get; set; }

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayNettAmt { get; set; }
}
