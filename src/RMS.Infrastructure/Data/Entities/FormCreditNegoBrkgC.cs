using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditNegoBrkgC
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? BrkgType { get; set; }

    public bool? InternetTrd { get; set; }

    public decimal? TrdLessThan { get; set; }

    public decimal? TrdMoreThan { get; set; }

    public decimal? MinBrkg { get; set; }

    public string? Remarks { get; set; }
}
