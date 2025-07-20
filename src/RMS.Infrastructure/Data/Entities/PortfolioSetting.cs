using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PortfolioSetting
{
    public int PfSetId { get; set; }

    public string? PfSetValue { get; set; }

    public string? PfSetDesc { get; set; }
}
