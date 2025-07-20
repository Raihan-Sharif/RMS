using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstOff9Bank
{
    public string BankCode { get; set; } = null!;

    public string? BankDesc { get; set; }

    public string? BankValue { get; set; }
}
