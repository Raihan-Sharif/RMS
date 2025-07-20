using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DailyBizDone
{
    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string? DocType { get; set; }

    public string? PurchaseSell { get; set; }

    public string ContDocNo { get; set; } = null!;

    public DateTime ContTrxDate { get; set; }

    public decimal? GrossAmt { get; set; }

    public int? Qty { get; set; }

    public string CancelRepInd { get; set; } = null!;

    public string? StkCode { get; set; }

    public string? StkName { get; set; }

    public decimal? Price { get; set; }

    public string? RemisierId { get; set; }

    public decimal? NetAmt { get; set; }

    public decimal? Brokerage { get; set; }

    public decimal? StampDuty { get; set; }

    public decimal? ClearingFees { get; set; }

    public decimal? ExchRate { get; set; }

    public string? Currency { get; set; }

    public decimal? Fprice { get; set; }

    public decimal? FconVal { get; set; }

    public decimal? OthersAmt { get; set; }

    public string? FstkName { get; set; }

    public string? Exchange { get; set; }

    public decimal? AmtBeforeGst { get; set; }

    public decimal? Gstsupplies { get; set; }
}
