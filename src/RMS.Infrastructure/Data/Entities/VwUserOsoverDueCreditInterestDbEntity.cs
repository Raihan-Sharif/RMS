using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverDueCreditInterestDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totalosaccrued { get; set; }
}
