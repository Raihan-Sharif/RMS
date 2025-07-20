using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DbgenFileCountDbEntity
{
    public DateTime? LogDate { get; set; }

    public string Item { get; set; } = null!;

    public int? Count { get; set; }

    public string? ProcessDate { get; set; }
}
