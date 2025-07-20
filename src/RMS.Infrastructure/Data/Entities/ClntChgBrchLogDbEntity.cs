using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntChgBrchLogDbEntity
{
    public long SequenceNo { get; set; }

    public DateTime LogTime { get; set; }

    public string LogMsg { get; set; } = null!;
}
