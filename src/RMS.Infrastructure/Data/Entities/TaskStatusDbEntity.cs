using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TaskStatusDbEntity
{
    public int TaskCat { get; set; }

    public string TaskId { get; set; } = null!;

    public DateTime TaskDate { get; set; }

    public string? Status { get; set; }
}
