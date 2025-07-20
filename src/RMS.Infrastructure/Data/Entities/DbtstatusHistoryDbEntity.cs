using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DbtstatusHistoryDbEntity
{
    public string MemberRef { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string Status { get; set; } = null!;

    public int StatusSeqNo { get; set; }
}
