using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TempStkIndicator
{
    public string? SContent { get; set; }

    public string? XchgCode { get; set; }

    public DateTime? InsertDateTime { get; set; }

    public string? StkCode { get; set; }

    public decimal? StkLastDonePrice { get; set; }
}
