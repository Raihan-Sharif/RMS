using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MyAccProcDate
{
    public string BranchCode { get; set; } = null!;

    public DateTime? LastUpdatedDate { get; set; }

    public int ProcessStatus { get; set; }

    public string? ProcessType { get; set; }
}
