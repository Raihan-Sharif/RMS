using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormBizProcForeignContDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? Type { get; set; }

    public string? Country { get; set; }

    public string? StkCode { get; set; }

    public int? Qty { get; set; }

    public string? Currency { get; set; }

    public decimal? Price { get; set; }

    public string? PurBasedOn { get; set; }

    public string? AccountNo { get; set; }

    public decimal? EstimatedAmt { get; set; }
}
