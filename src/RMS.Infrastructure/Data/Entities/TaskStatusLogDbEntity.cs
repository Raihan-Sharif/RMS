using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TaskStatusLogDbEntity
{
    public int TaskCat { get; set; }

    public string TaskId { get; set; } = null!;

    public DateTime TaskDate { get; set; }

    public string? Status { get; set; }

    public DateTime? TaskStartTime { get; set; }

    public DateTime? TaskEndTime { get; set; }
}
