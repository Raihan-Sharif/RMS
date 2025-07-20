using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkSectDbEntity
{
    public string XchgCode { get; set; } = null!;

    public string BrdCode { get; set; } = null!;

    public string SectCode { get; set; } = null!;

    public string? SectDesc { get; set; }
}
