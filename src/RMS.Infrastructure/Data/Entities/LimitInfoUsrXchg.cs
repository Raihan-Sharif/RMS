using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LimitInfoUsrXchg
{
    public string UsrId { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? TodayNettAmt { get; set; }
}
