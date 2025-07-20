using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LimitInfo202406211438DbEntity
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public decimal? TodayEarmarkedAmt { get; set; }

    public decimal? TodayEarmarkedSellAmt { get; set; }

    public decimal? TodayPurchasedAmt { get; set; }

    public decimal? TodaySoldAmt { get; set; }

    public DateTime? LastUpdateDate { get; set; }

    public decimal? CashBalance { get; set; }

    public decimal? ApproveTradingLimit { get; set; }

    public decimal? FsttodayEarmarkedAmt { get; set; }

    public decimal? FsttodayEarmarkedSellAmt { get; set; }

    public decimal? FsttodayPurchasedAmt { get; set; }

    public decimal? FsttodaySoldAmt { get; set; }

    public decimal? DlrNettAmt { get; set; }

    public decimal? ClntNettAmt { get; set; }

    public decimal? DlrEarmarkedAmt { get; set; }

    public decimal? DlrSoldAmt { get; set; }

    public decimal? IdssgrossTodayEarmarkedAmt { get; set; }

    public decimal? IdssgrossTodayEarmarkedSellAmt { get; set; }

    public decimal? IdssgrossTodayPurchasedAmt { get; set; }

    public decimal? IdssgrossTodaySoldAmt { get; set; }

    public decimal? IdsstodayEarmarkedAmt { get; set; }

    public decimal? IdsstodayEarmarkedSellAmt { get; set; }

    public decimal? IdsstodayPurchasedAmt { get; set; }

    public decimal? IdsstodaySoldAmt { get; set; }
}
