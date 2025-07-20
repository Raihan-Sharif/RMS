using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsduePurchase
{
    public string Userid { get; set; } = null!;

    public decimal? Totalospurchase { get; set; }
}
