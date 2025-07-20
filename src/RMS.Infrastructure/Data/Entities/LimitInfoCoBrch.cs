using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LimitInfoCoBrch
{
    public string CoCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayNettAmt { get; set; }

    public decimal? IdssgrossTodayEarmarkedAmt { get; set; }

    public decimal? IdssgrossTodayEarmarkedSellAmt { get; set; }

    public decimal? IdssgrossTodayPurchasedAmt { get; set; }

    public decimal? IdssgrossTodaySoldAmt { get; set; }

    public decimal? IdsstodayEarmarkedAmt { get; set; }

    public decimal? IdsstodayEarmarkedSellAmt { get; set; }

    public decimal? IdsstodayPurchasedAmt { get; set; }

    public decimal? IdsstodaySoldAmt { get; set; }
}
