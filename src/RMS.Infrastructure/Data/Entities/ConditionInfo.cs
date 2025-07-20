using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ConditionInfo
{
    public int OrderSeqNo { get; set; }

    public int ConditionId { get; set; }

    public string? ConditionValue { get; set; }
}
