using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientOsdueCreditInterest
{
    public string ClientCode { get; set; } = null!;

    public decimal? TotalOsaccruedInterest { get; set; }
}
