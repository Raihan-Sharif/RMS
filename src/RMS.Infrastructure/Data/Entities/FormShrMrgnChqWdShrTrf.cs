using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormShrMrgnChqWdShrTrf
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public decimal? ChqWdAmt { get; set; }

    public string? StkCode { get; set; }

    public int? TrfQty { get; set; }
}
