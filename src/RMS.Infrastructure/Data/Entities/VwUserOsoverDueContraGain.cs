using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverDueContraGain
{
    public string Userid { get; set; } = null!;

    public decimal? Totaloscontragain { get; set; }
}
