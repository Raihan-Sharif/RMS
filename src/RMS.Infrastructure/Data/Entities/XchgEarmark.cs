using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XchgEarmark
{
    public string XchgCode { get; set; } = null!;

    public bool? EarmarkFlag { get; set; }
}
