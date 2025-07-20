using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwOrderStatus
{
    public string? SequenceNo { get; set; }

    public DateTime TimeStamp { get; set; }

    public int TerminalId { get; set; }

    public string? OrderNo { get; set; }

    public string? Status { get; set; }

    public int StatusSeqNo { get; set; }

    public int? TimestampFmt { get; set; }
}
