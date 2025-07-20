using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CareOrderGroupStock
{
    public int GroupId { get; set; }

    public string StockCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;
}
