using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstBrkDbEntity
{
    public string BrkCode { get; set; } = null!;

    public string? BrkName { get; set; }

    public string? BrkSname { get; set; }

    public string? ForeignInd { get; set; }
}
