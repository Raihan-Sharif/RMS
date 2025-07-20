using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsdueContraLoss
{
    public string Userid { get; set; } = null!;

    public decimal? Totaloscontraloss { get; set; }
}
