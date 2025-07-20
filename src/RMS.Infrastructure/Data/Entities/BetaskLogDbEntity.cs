using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class BetaskLogDbEntity
{
    public string TaskId { get; set; } = null!;

    public DateTime TimeStamp { get; set; }

    public int SeqNo { get; set; }

    public string? Status { get; set; }
}
