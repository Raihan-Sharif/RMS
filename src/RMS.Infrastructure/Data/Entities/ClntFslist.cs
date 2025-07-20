using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntFslist
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string? DlrCode { get; set; }

    public string? BrkCode { get; set; }

    public string? StockCode { get; set; }

    public string? BuySellCode { get; set; }

    public int? OrderQty { get; set; }

    public int? OrderType { get; set; }

    public decimal? OrderPrice { get; set; }

    public DateTime? DatePurchased { get; set; }
}
