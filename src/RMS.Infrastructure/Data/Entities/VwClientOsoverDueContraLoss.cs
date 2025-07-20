using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientOsoverDueContraLoss
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public decimal? TotalOscontraLoss { get; set; }
}
