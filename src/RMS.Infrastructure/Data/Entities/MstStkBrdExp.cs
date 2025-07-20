using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkBrdExp
{
    public string XchgCode { get; set; } = null!;

    public string BrdCode { get; set; } = null!;

    public int? AllowPdt { get; set; }
}
