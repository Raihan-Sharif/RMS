using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class On9EnqPolicyDbEntity
{
    public int On9EnqId { get; set; }

    public int On9EnqSysType { get; set; }

    public int? On9EnqCondition { get; set; }

    public int? On9EnqCompulsoryCondition { get; set; }

    public bool? On9EnqAllowSelect { get; set; }
}
