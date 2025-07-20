using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Test
{
    public string? XchgCode { get; set; }

    public string? ClientCode { get; set; }

    public string? BranchCode { get; set; }

    public string? StockCode { get; set; }

    public int? Qty { get; set; }

    public decimal? Price { get; set; }

    public string? TransType { get; set; }

    public string? TransDetail2 { get; set; }

    public DateTime? Timestamp { get; set; }

    public decimal? Avgprice { get; set; }
}
