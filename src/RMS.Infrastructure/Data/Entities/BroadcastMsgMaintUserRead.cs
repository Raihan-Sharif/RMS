using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class BroadcastMsgMaintUserRead
{
    public int NewsId { get; set; }

    public string Channel { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime? ReadDate { get; set; }
}
