using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ErrAcctList
{
    public string BrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string? PrntBrchCode { get; set; }
}
