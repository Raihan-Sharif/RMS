using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserMtdturnoverDbEntity
{
    public string? Userid { get; set; }

    public string Source { get; set; } = null!;

    public string Year { get; set; } = null!;

    public int Month { get; set; }

    public decimal? Turnover { get; set; }

    public decimal? Brokerage { get; set; }
}
