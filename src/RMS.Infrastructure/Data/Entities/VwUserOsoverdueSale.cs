using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverdueSale
{
    public string Userid { get; set; } = null!;

    public decimal? Totalossale { get; set; }
}
