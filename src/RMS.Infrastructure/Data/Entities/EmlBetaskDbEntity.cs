using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EmlBetaskDbEntity
{
    public DateTime TaskDate { get; set; }

    public int TaskCat { get; set; }

    public string TaskId { get; set; } = null!;

    public DateTime? LastSent { get; set; }

    public int EmlId { get; set; }

    public string? ReceiverAddr { get; set; }

    public string? ReceiverCcaddr { get; set; }
}
