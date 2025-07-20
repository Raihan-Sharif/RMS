using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntBankAccInfo
{
    public string SCoBrchCode { get; set; } = null!;

    public string SClntCode { get; set; } = null!;

    public string? SClntBankName { get; set; }

    public string? SClntBankBrch { get; set; }

    public string? SClntBankAccNo { get; set; }

    public string SDefaultBankAcc { get; set; } = null!;

    public string SMca { get; set; } = null!;
}
