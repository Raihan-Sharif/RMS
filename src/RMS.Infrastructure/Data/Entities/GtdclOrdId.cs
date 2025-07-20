using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class GtdclOrdId
{
    public int SequenceNo { get; set; }

    public string ClOrdId { get; set; } = null!;

    public string? OrderType { get; set; }

    public string XchgCode { get; set; } = null!;
}
