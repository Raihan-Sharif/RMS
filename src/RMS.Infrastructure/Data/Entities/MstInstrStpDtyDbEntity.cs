using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstInstrStpDtyDbEntity
{
    public int InstrTypeCd { get; set; }

    public string? InstrTypeDesc { get; set; }

    public decimal? StpDtyMin { get; set; }

    public decimal? StpDtyVal { get; set; }

    public decimal? StpDtyMax { get; set; }
}
