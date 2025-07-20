using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderPlacedTestDbEntity
{
    public DateTime Timestamp { get; set; }

    public decimal? MatchedAmount { get; set; }
}
