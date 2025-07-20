using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class InternalAcctDbEntity
{
    public string CoBrchCode { get; set; } = null!;

    public string? TrdgCode { get; set; }

    public string ClntSname { get; set; } = null!;

    public string ClntCdsno { get; set; } = null!;

    public string? ClntName { get; set; }
}
