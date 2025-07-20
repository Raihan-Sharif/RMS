using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditEarmark
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public decimal? ClredTabal { get; set; }

    public decimal? EarmarkAmt { get; set; }
}
