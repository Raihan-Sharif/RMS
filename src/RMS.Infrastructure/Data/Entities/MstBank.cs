using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstBank
{
    public string BankCode { get; set; } = null!;

    public string? BankDesc { get; set; }
}
