using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstIncGrpDbEntity
{
    public int IncGrpCode { get; set; }

    public string? IncGrpDesc { get; set; }

    public decimal? IncMinVal { get; set; }

    public decimal? IncMaxVal { get; set; }
}
