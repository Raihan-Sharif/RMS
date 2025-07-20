using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClientTurnOverDbEntity
{
    public string ClntCode { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string Year { get; set; } = null!;

    public int Month { get; set; }

    public decimal? TurnOver { get; set; }

    public decimal? Brokerage { get; set; }

    public string Source { get; set; } = null!;
}
