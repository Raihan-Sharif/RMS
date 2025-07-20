using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstGlobalClntEcosLimitExp
{
    public int ExpLevelType { get; set; }

    public string? ExpVal { get; set; }

    public int? ExpLmtType { get; set; }

    public decimal? LimitChangePctg { get; set; }

    public DateTime? LastUpdated { get; set; }
}
