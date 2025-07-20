using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ResearchAnalystDbEntity
{
    public string AnalystCode { get; set; } = null!;

    public string AnalystName { get; set; } = null!;

    public string AnalystDesc { get; set; } = null!;
}
