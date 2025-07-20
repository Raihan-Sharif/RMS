using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsunDueDebitInterest
{
    public string Userid { get; set; } = null!;

    public decimal? Totalosaccrued { get; set; }
}
