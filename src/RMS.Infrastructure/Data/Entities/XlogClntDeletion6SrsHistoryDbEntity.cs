using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogClntDeletion6SrsHistoryDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? ClntCode { get; set; }

    public string? DeleteInd { get; set; }
}
