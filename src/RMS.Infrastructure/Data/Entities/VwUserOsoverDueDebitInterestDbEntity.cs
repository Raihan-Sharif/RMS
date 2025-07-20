using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverDueDebitInterestDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totalosaccrued { get; set; }
}
