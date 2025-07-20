using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrdStatusDbEntity
{
    public string OrdStatus1 { get; set; } = null!;

    public string? OrdStatusDesc { get; set; }
}
