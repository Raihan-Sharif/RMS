using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsunduePurchaseDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totalospurchase { get; set; }
}
