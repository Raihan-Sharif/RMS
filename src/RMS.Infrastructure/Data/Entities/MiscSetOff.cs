using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MiscSetOff
{
    public string ClientCode { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string TransNo { get; set; } = null!;

    public DateTime TransDate { get; set; }

    public decimal? TransAmount { get; set; }

    public string TransType { get; set; } = null!;

    public DateTime TransDueDate { get; set; }

    public int? Age { get; set; }
}
