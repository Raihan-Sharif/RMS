using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientOsundueCreditInterest
{
    public string ClientCode { get; set; } = null!;

    public decimal? TotalOsaccruedInterest { get; set; }
}
