using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsdueCreditInterestDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totalosaccrued { get; set; }
}
