using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstBankDbEntity
{
    public string BankCode { get; set; } = null!;

    public string? BankDesc { get; set; }
}
