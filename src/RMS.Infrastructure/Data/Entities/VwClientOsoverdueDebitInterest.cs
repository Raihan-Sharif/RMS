using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientOsoverdueDebitInterest
{
    public string ClientCode { get; set; } = null!;

    public decimal? TotalOsaccruedInterest { get; set; }
}
