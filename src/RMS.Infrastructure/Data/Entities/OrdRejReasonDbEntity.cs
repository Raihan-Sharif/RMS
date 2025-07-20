using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrdRejReasonDbEntity
{
    public string OrdRejReason1 { get; set; } = null!;

    public string? OrdRejReasonDesc { get; set; }
}
