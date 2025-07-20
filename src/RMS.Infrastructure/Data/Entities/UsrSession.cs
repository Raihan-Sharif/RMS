using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrSession
{
    public string UsrId { get; set; } = null!;

    public string UsrSessionId { get; set; } = null!;

    public int SystemType { get; set; }

    public string? UsrRemoteAdd { get; set; }

    public string? UsrLoginSystemType { get; set; }
}
