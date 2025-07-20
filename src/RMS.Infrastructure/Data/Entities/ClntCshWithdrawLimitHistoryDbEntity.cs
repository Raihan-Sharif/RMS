using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntCshWithdrawLimitHistoryDbEntity
{
    public DateTime LogDate { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public decimal ClntExpsNetAmt { get; set; }

    public decimal WithdrawalLimit { get; set; }

    public string Status { get; set; } = null!;

    public string Remarks { get; set; } = null!;
}
