using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrXchgModeChg
{
    public int Id { get; set; }

    public string UsrId { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Mode { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Enabled { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? ActionType { get; set; }
}
