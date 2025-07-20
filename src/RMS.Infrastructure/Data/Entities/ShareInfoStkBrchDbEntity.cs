using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ShareInfoStkBrchDbEntity
{
    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public string CoCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public int? TodayEarmarkedQty { get; set; }

    public int? TodayPurchaseQty { get; set; }

    public int? TodaySoldQty { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

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
