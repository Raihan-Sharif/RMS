using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TaskFrequency202403261047
{
    public string TaskId { get; set; } = null!;

    public int TaskCat { get; set; }

    public string Frequency { get; set; } = null!;

    public int Ref1 { get; set; }

    public int Ref2 { get; set; }
}
