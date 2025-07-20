using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormFinanceRecoveryDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? DraweeBank { get; set; }

    public string? ChqNo { get; set; }

    public string? Place { get; set; }

    public decimal? ChqAmt { get; set; }

    public decimal? CotrLossPrinAmt { get; set; }

    public decimal? PurDsadamt { get; set; }

    public decimal? Intr { get; set; }
}
