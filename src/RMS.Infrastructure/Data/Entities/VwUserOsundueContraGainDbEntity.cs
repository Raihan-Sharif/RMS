using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsundueContraGainDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? TotaloscontraGain { get; set; }
}
