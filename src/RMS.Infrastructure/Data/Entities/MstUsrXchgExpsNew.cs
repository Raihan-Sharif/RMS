using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstUsrXchgExpsNew
{
    public string UsrId { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public decimal? Percentage { get; set; }

    public decimal? UsrBuyAmtTopUp { get; set; }

    public decimal? UsrSellAmtTopUp { get; set; }

    public decimal? UsrTotalAmtTopUp { get; set; }

    public decimal? UsrNettAmtTopUp { get; set; }

    public decimal? UsrBuyPrevDayOrder { get; set; }

    public decimal? UsrSellPrevDayOrder { get; set; }

    public decimal? UsrTotalPrevDayOrder { get; set; }

    public decimal? UsrNettPrevDayOrder { get; set; }

    public int? UsrTradeStatus { get; set; }

    public decimal? UsrBuyTrnxLimit { get; set; }

    public int? UsrBuyLotLimit { get; set; }

    public int? UsrBuyBidLimit { get; set; }

    public decimal? UsrSellTrnxLimit { get; set; }

    public int? UsrSellLotLimit { get; set; }

    public int? UsrSellBidLimit { get; set; }

    public int? UsrOrderModality { get; set; }

    public int? UsrOrderType { get; set; }

    public string? UsrExpsRemarks { get; set; }

    public string? UsrTradeRemarks { get; set; }

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? TodayNettAmt { get; set; }
}
