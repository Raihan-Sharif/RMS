using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserMtdturnoverChart
{
    public string? Userid { get; set; }

    public string Year { get; set; } = null!;

    public int Month { get; set; }

    public decimal? Turnover { get; set; }

    public decimal? Brokerage { get; set; }
}
