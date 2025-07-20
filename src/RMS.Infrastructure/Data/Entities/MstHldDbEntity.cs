using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstHldDbEntity
{
    public DateTime HldDate { get; set; }

    public string? HldDesc { get; set; }

    public string? Type { get; set; }

    public string XchgCode { get; set; } = null!;

    public DateTime? TradeStartTime { get; set; }

    public DateTime? TradeEndTime { get; set; }
}
