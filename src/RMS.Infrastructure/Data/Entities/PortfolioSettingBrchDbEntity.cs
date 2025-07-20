using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PortfolioSettingBrchDbEntity
{
    public string CompanyCode { get; set; } = null!;

    public int PfSetId { get; set; }

    public string? PfSetValue { get; set; }
}
