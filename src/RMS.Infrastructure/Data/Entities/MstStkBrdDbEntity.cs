using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkBrdDbEntity
{
    public string XchgCode { get; set; } = null!;

    public string BrdCode { get; set; } = null!;

    public string BrdDesc { get; set; } = null!;
}
