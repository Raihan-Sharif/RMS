using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TaskDependency202404291349
{
    public string TaskId { get; set; } = null!;

    public int TaskCat { get; set; }

    public string Dependency { get; set; } = null!;
}
