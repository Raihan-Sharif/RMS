using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstFbmstkDbEntity
{
    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public int? Fbm { get; set; }
}
