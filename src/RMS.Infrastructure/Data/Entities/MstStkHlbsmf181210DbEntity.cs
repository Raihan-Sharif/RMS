using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkHlbsmf181210DbEntity
{
    public string XchgCode { get; set; } = null!;

    public string? StkBrdCode { get; set; }

    public string? StkSectCode { get; set; }

    public string StkCode { get; set; } = null!;

    public string? StkLname { get; set; }

    public string? StkSname { get; set; }

    public string? StkStat { get; set; }

    public decimal? StkLastDonePrice { get; set; }

    public int? StkEcprice { get; set; }

    public decimal? StkClosePrice { get; set; }

    public decimal? StkRefPrc { get; set; }

    public int? StkIsSyariah { get; set; }

    public int? StkLotSize { get; set; }

    public string? StkDelvCode { get; set; }

    public long? StkShareIssue { get; set; }

    public string? CcyCd { get; set; }

    public DateTime? MaturityDt { get; set; }

    public decimal? StkParVal { get; set; }

    public decimal? StkPucapt { get; set; }

    public decimal? YearHigh { get; set; }

    public decimal? YearLow { get; set; }
}
