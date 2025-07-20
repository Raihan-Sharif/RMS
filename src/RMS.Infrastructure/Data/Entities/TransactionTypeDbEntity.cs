using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TransactionTypeDbEntity
{
    public string TransType { get; set; } = null!;

    public string? TransDesc { get; set; }

    public int? Enabled { get; set; }
}
