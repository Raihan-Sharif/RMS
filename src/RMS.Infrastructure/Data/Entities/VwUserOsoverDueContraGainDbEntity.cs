using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverDueContraGainDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totaloscontragain { get; set; }
}
