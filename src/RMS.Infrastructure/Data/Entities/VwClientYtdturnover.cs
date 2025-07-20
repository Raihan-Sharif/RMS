using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientYtdturnover
{
    public string ClntCode { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string Year { get; set; } = null!;

    public decimal? Turnover { get; set; }

    public decimal? Brokerage { get; set; }
}
