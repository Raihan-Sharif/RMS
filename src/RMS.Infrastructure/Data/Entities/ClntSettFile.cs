using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntSettFile
{
    public DateTime TrnxDate { get; set; }

    public string TrnxNo { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string BrkBrchCode { get; set; } = null!;

    public string TrnxRefNo { get; set; } = null!;

    public string? TrnxRefType { get; set; }

    public DateTime? TrnxRefDueDate { get; set; }

    public string? StkCode { get; set; }

    public int? TrnxRefQty { get; set; }

    public int? SettQty { get; set; }

    public decimal? TrnxRefPrc { get; set; }

    public decimal? TrnxRefVal { get; set; }

    public decimal? SettVal { get; set; }

    public string? TrnxRmk { get; set; }
}
