using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EdsvariableDbEntity
{
    public string VariableName { get; set; } = null!;

    public string? Content { get; set; }
}
