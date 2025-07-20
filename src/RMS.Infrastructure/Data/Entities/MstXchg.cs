using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstXchg
{
    public string XchgCode { get; set; } = null!;

    public string XchgDesc { get; set; } = null!;

    public int XchgStartTradeTime { get; set; }

    public int XchgEndTradeTime { get; set; }

    public string? TradeUntilNextDay { get; set; }
}
