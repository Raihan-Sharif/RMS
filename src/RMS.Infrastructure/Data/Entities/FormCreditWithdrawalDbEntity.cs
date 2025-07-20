using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditWithdrawalDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? WdFrom { get; set; }

    public decimal? WdSumAmt { get; set; }
}
