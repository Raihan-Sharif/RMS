using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCurcy
{
    public string CurcyCode { get; set; } = null!;

    public decimal? CurcyBursaRate { get; set; }

    public DateTime? CurcyBursaRateChngDate { get; set; }

    public decimal? CurcyBrkRate { get; set; }

    public DateTime? CurcyBrkRateChngDate { get; set; }

    /// <summary>
    /// 0 - Using Bursa Rate (CurcyBursaRate); 1 - Using Broker Rate (CurcyBrkRate);
    /// </summary>
    public int? CurcyRate2Use { get; set; }

    public decimal? CurcyBuyRate { get; set; }

    public decimal? CurcySellRate { get; set; }
}
