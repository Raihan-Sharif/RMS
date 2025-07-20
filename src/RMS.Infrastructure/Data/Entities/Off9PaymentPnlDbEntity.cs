using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Off9PaymentPnlDbEntity
{
    public string BkCode { get; set; } = null!;

    public string BkDesc { get; set; } = null!;

    public string BkAcctCode { get; set; } = null!;
}
