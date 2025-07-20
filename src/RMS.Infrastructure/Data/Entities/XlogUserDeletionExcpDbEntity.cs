using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogUserDeletionExcpDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? UsrId { get; set; }

    public string? DeleteInd { get; set; }

    public string? Remarks { get; set; }
}
