using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Delisted
{
    public string? Sec { get; set; }

    public string? KlseNo { get; set; }

    public string? SecurityName { get; set; }

    public string? DelistedDate { get; set; }
}
