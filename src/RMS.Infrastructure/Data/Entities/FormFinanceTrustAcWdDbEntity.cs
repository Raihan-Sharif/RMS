using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormFinanceTrustAcWdDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public decimal? WdSumAmt { get; set; }

    public string? WdPurpose { get; set; }

    public string? Remarks { get; set; }
}
