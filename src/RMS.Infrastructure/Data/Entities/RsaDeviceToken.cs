using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class RsaDeviceToken
{
    public string UsrId { get; set; } = null!;

    public string? DeviceToken { get; set; }

    public DateTime? LastUpdatedDate { get; set; }
}
