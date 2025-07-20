using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientOsundueContraGain
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public decimal? TotalOscontraGain { get; set; }
}
