using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientOsdueDebitInterest
{
    public string ClientCode { get; set; } = null!;

    public decimal? TotalOsaccruedInterest { get; set; }
}
