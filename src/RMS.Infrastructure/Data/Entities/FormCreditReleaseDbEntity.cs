using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditReleaseDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public decimal? EarmarkAmt { get; set; }

    public decimal? ReleaseAmt { get; set; }

    public decimal? BalanceAmt { get; set; }
}
