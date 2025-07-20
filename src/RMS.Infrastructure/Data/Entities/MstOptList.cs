using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstOptList
{
    public int OptGrp { get; set; }

    public string OptVal { get; set; } = null!;

    public string? OptText { get; set; }

    public int? OptSeq { get; set; }
}
