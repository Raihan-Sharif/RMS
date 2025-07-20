using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCo
{
    public string CoCode { get; set; } = null!;

    public string? CoDesc { get; set; }

    public long? TradingPolicy { get; set; }

    public int? SenderType { get; set; }

    public DateTime? UsrAccessLimitDefExprDate { get; set; }

    public DateTime? UsrAccessLimitDefStartTime { get; set; }

    public DateTime? UsrAccessLimitDefEndTime { get; set; }

    public int? UsrAccessLimitDefDays { get; set; }
}
