using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Tncaccept
{
    public DateTime DateTime { get; set; }

    public string UsrId { get; set; } = null!;

    public string? Icno { get; set; }

    public string? Accepted { get; set; }

    public int? Id { get; set; }

    public string XchgCode { get; set; } = null!;

    public string? Module { get; set; }
}
