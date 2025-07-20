using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class RptOptLst
{
    public string RptGrpId { get; set; } = null!;

    public string RptId { get; set; } = null!;

    public string? RptDesc { get; set; }

    public string? RptFileName { get; set; }
}
