using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderConfirmExstatusDbEntity
{
    public DateTime Timestamp { get; set; }

    public int StatusSeqNo { get; set; }

    public string ClOrdId { get; set; } = null!;

    public string OrderNo { get; set; } = null!;

    public string? Status { get; set; }

    public int SequenceNo { get; set; }
}
