using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AllUserClientDbEntity
{
    public string UsrId { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string? UsrBrchCode { get; set; }

    public string ClntBrchCode { get; set; } = null!;

    public string? UsrName { get; set; }

    public string? ClntName { get; set; }

    public string? UsrNicno { get; set; }

    public string? ClntNicno { get; set; }

    public string? UsrPassNo { get; set; }

    public string? ClntOicno { get; set; }

    public string? UsrEmail { get; set; }
}
