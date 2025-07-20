using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntXchg
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Mode { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Enabled { get; set; }

    public string? MktDepth { get; set; }
}
