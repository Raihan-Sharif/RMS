using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TaskFrequency20150727DbEntity
{
    public string TaskId { get; set; } = null!;

    public int TaskCat { get; set; }

    public string Frequency { get; set; } = null!;

    public int Ref1 { get; set; }

    public int Ref2 { get; set; }
}
