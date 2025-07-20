using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderStatusHistoryDbEntity
{
    public int SequenceNo { get; set; }

    public DateTime TimeStamp { get; set; }

    public int TerminalId { get; set; }

    public string OrderNo { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int StatusSeqNo { get; set; }

    public int TimestampFmt { get; set; }
}
