using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverDuePurchase
{
    public string Userid { get; set; } = null!;

    public decimal? Totalospurchase { get; set; }
}
