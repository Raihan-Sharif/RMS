using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverDueSetOffGain
{
    public string Userid { get; set; } = null!;

    public decimal? Totalossetoffgain { get; set; }
}
