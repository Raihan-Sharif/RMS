using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstFpxbankDbEntity
{
    public string BankId { get; set; } = null!;

    public string? BankName { get; set; }

    public string? BankDisplayName { get; set; }
}
