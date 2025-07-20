using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkBrd
{
    public string XchgCode { get; set; } = null!;

    public string BrdCode { get; set; } = null!;

    public string BrdDesc { get; set; } = null!;
}
