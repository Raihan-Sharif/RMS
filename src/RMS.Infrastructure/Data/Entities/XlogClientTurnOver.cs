using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogClientTurnOver
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    public string? BranchCode { get; set; }

    public string? ClientCode { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }

    public string Source { get; set; } = null!;
}
