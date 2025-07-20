using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EnqMatchedTradeDbEntity
{
    public DateTime Date { get; set; }

    public string BranchCode { get; set; } = null!;

    public int? KenTrade { get; set; }

    public int? Mobile { get; set; }

    public int? Btx { get; set; }

    public int? Btxmobility { get; set; }
}
