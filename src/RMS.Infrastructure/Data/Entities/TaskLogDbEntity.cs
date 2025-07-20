using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TaskLogDbEntity
{
    public DateTime LogTime { get; set; }

    public int TaskCat { get; set; }

    public string TaskId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime TaskDate { get; set; }
}
