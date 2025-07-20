using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class LogInfo
{
    public int RptGrpId { get; set; }

    public int RptId { get; set; }

    public string? RptDesc { get; set; }

    public string? TableName { get; set; }

    public string? RptKeyDesc { get; set; }

    public string? RptKeyField { get; set; }

    public string? RptSearchStoredProc { get; set; }

    public string? RptGrpHeader { get; set; }

    public int? ReferNodeId { get; set; }
}
