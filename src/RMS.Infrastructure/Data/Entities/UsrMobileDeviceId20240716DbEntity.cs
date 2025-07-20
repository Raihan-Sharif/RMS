using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrMobileDeviceId20240716DbEntity
{
    public string UsrId { get; set; } = null!;

    public string DeviceId { get; set; } = null!;

    public string PlatCode { get; set; } = null!;

    public DateTime? UsrLastUpdated { get; set; }

    public string Uuid { get; set; } = null!;

    public string? DeviceName { get; set; }
}
