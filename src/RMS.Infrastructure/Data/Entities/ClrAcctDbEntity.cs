using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClrAcctDbEntity
{
    public string? BranchCode { get; set; }

    public string? ClientCode { get; set; }

    public string? CdsNo { get; set; }

    public string? ClientName { get; set; }

    public string? TraderCode { get; set; }

    public string? CreditLimit { get; set; }

    public string? BookInOut { get; set; }

    public string? BookCdsClient { get; set; }
}
