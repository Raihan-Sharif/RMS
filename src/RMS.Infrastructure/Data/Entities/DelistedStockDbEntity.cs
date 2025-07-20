using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DelistedStockDbEntity
{
    public string? Stkcode { get; set; }

    public string? ShortName { get; set; }

    public string? SecurityName { get; set; }

    public string? Column3 { get; set; }
}
