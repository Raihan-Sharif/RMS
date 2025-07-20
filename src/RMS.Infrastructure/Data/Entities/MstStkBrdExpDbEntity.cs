using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkBrdExpDbEntity
{
    public string XchgCode { get; set; } = null!;

    public string BrdCode { get; set; } = null!;

    public int? AllowPdt { get; set; }
}
