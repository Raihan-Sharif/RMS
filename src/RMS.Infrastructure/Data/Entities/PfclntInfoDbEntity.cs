using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PfclntInfoDbEntity
{
    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public decimal? InitCapital { get; set; }

    public decimal? CashBal { get; set; }

    public DateTime? DtLastUpdate { get; set; }
}
