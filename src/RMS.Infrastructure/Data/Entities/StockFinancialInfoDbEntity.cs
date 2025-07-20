using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class StockFinancialInfoDbEntity
{
    public string StockCode { get; set; } = null!;

    public string? Data { get; set; }
}
