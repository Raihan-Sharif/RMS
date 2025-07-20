using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LimitInfoClntXchgDbEntity
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? DlrNettAmt { get; set; }

    public decimal? ClntNettAmt { get; set; }

    public decimal? DlrEarmarkedAmt { get; set; }

    public decimal? DlrSoldAmt { get; set; }
}
