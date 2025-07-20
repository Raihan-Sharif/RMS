using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderStatusHistorySummArchive
{
    public int SequenceNo { get; set; }

    public int TimestampFmt { get; set; }

    public DateTime Timestamp { get; set; }

    public int StatusSeqNo { get; set; }

    public string? LastStatus { get; set; }
}
