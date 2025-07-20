using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsdueContraGainDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? TotaloscontraGain { get; set; }
}
