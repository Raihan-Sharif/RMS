using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntTrustAcTrnxDbEntity
{
    public string ClientCode { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public string? Particulars { get; set; }

    public decimal? TransactionAmount { get; set; }
}
