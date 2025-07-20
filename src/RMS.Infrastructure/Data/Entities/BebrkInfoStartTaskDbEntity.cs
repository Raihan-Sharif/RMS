using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class BebrkInfoStartTaskDbEntity
{
    public string TaskCat { get; set; } = null!;

    public string StartTask { get; set; } = null!;

    public string CanStart { get; set; } = null!;
}
