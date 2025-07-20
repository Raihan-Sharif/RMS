using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogClntDeletionExcpDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string BranchId { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string? DeleteInd { get; set; }

    public string? Remarks { get; set; }
}
