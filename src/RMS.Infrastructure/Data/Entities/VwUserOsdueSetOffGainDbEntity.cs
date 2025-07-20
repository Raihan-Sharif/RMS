using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsdueSetOffGainDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totalossetoffgain { get; set; }
}
