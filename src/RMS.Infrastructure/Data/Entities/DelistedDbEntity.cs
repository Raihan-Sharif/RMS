using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DelistedDbEntity
{
    public string? Sec { get; set; }

    public string? KlseNo { get; set; }

    public string? SecurityName { get; set; }

    public string? DelistedDate { get; set; }
}
