using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AllClientDbEntity
{
    public string ClntCode { get; set; } = null!;

    public string ClntBrchCode { get; set; } = null!;

    public string? ClntName { get; set; }

    public string? ClntNicno { get; set; }

    public string? ClntOicno { get; set; }
}
