using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccPymtStatus
{
    public string SPymtStatusCode { get; set; } = null!;

    public string SPymtStatus { get; set; } = null!;

    public string? SPymtStatusDesc { get; set; }
}
