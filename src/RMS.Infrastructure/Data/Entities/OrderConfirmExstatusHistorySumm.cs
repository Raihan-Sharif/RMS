using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderConfirmExstatusHistorySumm
{
    public string OrderNo { get; set; } = null!;

    public int TimestampFmt { get; set; }

    public DateTime Timestamp { get; set; }

    public int StatusSeqNo { get; set; }

    public string? LastStatus { get; set; }
}
