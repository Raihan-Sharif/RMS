using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormFinanceReversal
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public DateTime? RevDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public decimal? CotrLossAmt { get; set; }

    public decimal? CotrIntr { get; set; }

    public decimal? RevTotalAmt { get; set; }
}
