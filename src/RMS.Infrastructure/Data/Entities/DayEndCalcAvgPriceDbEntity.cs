using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DayEndCalcAvgPriceDbEntity
{
    public int Oid { get; set; }

    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public DateTime CalcDateFrom { get; set; }

    public DateTime CalcDateTo { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public string? XchgCode { get; set; }
}
