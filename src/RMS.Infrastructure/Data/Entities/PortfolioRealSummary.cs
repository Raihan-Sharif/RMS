using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PortfolioRealSummary
{
    public string ClientCode { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string StockCode { get; set; } = null!;

    public int TotalBuyQty { get; set; }

    public decimal TotalBuyAmt { get; set; }

    public decimal TotalBuyOverhead { get; set; }

    public int TotalSellQty { get; set; }

    public decimal TotalSellAmt { get; set; }

    public decimal TotalSellOverhead { get; set; }

    public decimal RealizedGain { get; set; }

    public decimal Dividend { get; set; }

    public decimal VwavgBuyPrice { get; set; }

    public decimal VwavgBuyPriceIncOvh { get; set; }

    public decimal? RealizedGainIncDiv { get; set; }

    public decimal CapPrice { get; set; }

    public decimal? AvgBuyPrice { get; set; }

    public decimal? AvgSellPrice { get; set; }

    public int? BalanceQty { get; set; }

    public string XchgCode { get; set; } = null!;

    public decimal? GrossBuyAmt { get; set; }
}
