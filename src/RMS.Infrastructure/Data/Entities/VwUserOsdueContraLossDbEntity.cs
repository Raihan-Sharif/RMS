using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwUserOsdueContraLossDbEntity
{
    public string Userid { get; set; } = null!;

    public decimal? Totaloscontraloss { get; set; }
}
