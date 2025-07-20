using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EmlLegend
{
    public int? EmlLegendId { get; set; }

    public string? EmlLegendDesc { get; set; }

    public string? EmlLegendAbbrv { get; set; }

    public int? EmlId { get; set; }
}
