using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class BidSettingDbEntity
{
    public decimal Price { get; set; }

    public decimal? Size { get; set; }

    public string XchgCode { get; set; } = null!;
}
