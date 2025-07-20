using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsdueSale
{
    public string Userid { get; set; } = null!;

    public decimal? Totalossale { get; set; }
}
