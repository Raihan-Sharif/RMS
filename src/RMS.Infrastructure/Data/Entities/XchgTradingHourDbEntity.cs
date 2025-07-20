using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XchgTradingHourDbEntity
{
    public string XchgCode { get; set; } = null!;

    public int TradingStart { get; set; }

    public int TradingEnd { get; set; }

    public int EarmarkTime { get; set; }

    public string? FirstTradeTime { get; set; }

    public string? SecondTradeTime { get; set; }

    public string? FirstTradeEnd { get; set; }

    public string? NextTradeTime { get; set; }

    public int? SecondTradeEnd { get; set; }
}
