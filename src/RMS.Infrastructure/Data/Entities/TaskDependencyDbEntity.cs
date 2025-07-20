using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TaskDependencyDbEntity
{
    public string TaskId { get; set; } = null!;

    public int TaskCat { get; set; }

    public string Dependency { get; set; } = null!;
}
