using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsunDueSetOffLossDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totalossetoffloss { get; set; }
}
