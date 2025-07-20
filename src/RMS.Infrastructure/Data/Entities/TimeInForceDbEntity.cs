using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TimeInForceDbEntity
{
    public string TimeInForce1 { get; set; } = null!;

    public string? TimeInForceDesc { get; set; }
}
