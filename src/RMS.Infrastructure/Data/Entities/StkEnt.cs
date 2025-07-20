using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class StkEnt
{
    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public string StkName { get; set; } = null!;

    public string? StkEntDesc { get; set; }

    public DateTime ExDate { get; set; }

    public DateTime LodgeDate { get; set; }

    public int Share { get; set; }

    public int ConvtShare { get; set; }
}
