using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstOptList202201281859
{
    public int OptGrp { get; set; }

    public string OptVal { get; set; } = null!;

    public string? OptText { get; set; }

    public int? OptSeq { get; set; }
}
