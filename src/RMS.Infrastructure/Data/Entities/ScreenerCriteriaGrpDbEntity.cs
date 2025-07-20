using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ScreenerCriteriaGrpDbEntity
{
    public int CriteriaGrpId { get; set; }

    public string? CriteriaGrpDesc { get; set; }

    public string? CriteriaGrpBgimg { get; set; }

    public string? CriteriaGrpTextClass { get; set; }
}
