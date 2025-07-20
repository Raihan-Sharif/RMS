using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientOsundueSale
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public decimal? Totalossale { get; set; }
}
