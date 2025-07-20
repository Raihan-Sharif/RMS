using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Hd14421logDbEntity
{
    public DateTime LogTime { get; set; }

    public string? Message { get; set; }
}
