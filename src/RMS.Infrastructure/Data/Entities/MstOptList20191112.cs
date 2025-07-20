using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstOptList20191112
{
    public int OptGrp { get; set; }

    public string OptVal { get; set; } = null!;

    public string? OptText { get; set; }

    public int? OptSeq { get; set; }
}
