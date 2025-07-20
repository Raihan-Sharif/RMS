using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LogInfoGrpDbEntity
{
    public int RptGrpId { get; set; }

    public string? RptGrpDesc { get; set; }

    public int? RptGrpViewNodeId { get; set; }

    public int? RptGrpPurgeNodeId { get; set; }
}
