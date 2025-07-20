using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstOrderGroup
{
    public int GroupCode { get; set; }

    public string? GroupDesc { get; set; }

    public string? GroupType { get; set; }

    public string? GroupValue { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }
}
