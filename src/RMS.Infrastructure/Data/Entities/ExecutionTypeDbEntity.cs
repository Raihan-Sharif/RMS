using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ExecutionTypeDbEntity
{
    public string ExecType { get; set; } = null!;

    public string? ExecTypeDesc { get; set; }
}
