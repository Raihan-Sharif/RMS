using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstBrchGrpDbEntity
{
    public string BrchGrpCode { get; set; } = null!;

    public string? BrchGrpDesc { get; set; }
}
