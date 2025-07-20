using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsoverDueContraLoss
{
    public string Userid { get; set; } = null!;

    public decimal? Totaloscontraloss { get; set; }
}
