using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstMultiXchgOptList
{
    public string XchgCode { get; set; } = null!;

    public string OptGrpVal { get; set; } = null!;

    public int? OptGrp { get; set; }

    public string? OptText { get; set; }
}
