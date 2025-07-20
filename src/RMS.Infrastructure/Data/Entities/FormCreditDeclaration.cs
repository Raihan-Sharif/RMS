using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditDeclaration
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? TrnxType { get; set; }

    public string? Dbttype { get; set; }

    public string? StkCode { get; set; }

    public int? SectInvolved { get; set; }

    public decimal? UnitPrice { get; set; }
}
