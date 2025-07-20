using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ShareInfo
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string StockCode { get; set; } = null!;

    public int? OpenFreeBalance { get; set; }

    public int? OpenPurchaseQty { get; set; }

    public int? OpenSalesQty { get; set; }

    public int? TodayEarmarkedQty { get; set; }

    public int? TodayPurchaseQty { get; set; }

    public int? TodaySoldQty { get; set; }

    public int? TodayTransferOutQty { get; set; }

    public int? MaxWithdrawalQty { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public string XchgCode { get; set; } = null!;

    public DateTime? LastUpdateDate { get; set; }

    public int? IdsssoldQty { get; set; }

    public int? IdssbuyBackQty { get; set; }

    public int? TodayDbtearmarkBuyQty { get; set; }
}
