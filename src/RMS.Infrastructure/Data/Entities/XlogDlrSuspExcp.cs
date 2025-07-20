using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogDlrSuspExcp
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string BranchId { get; set; } = null!;

    public string DealerId { get; set; } = null!;

    public string? AccessSuspendResume { get; set; }

    public string? BuySuspendResume { get; set; }

    public string? SellSuspendResume { get; set; }

    public string? Remarks { get; set; }

    public string? ErrRemarks { get; set; }
}
