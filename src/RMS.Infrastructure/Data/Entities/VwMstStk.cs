using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwMstStk
{
    public string XchgCode { get; set; } = null!;

    public string? StkBrdCode { get; set; }

    public string? StkSectCode { get; set; }

    public string StkCode { get; set; } = null!;

    public string? StkLname { get; set; }

    public string? StkSname { get; set; }
}
