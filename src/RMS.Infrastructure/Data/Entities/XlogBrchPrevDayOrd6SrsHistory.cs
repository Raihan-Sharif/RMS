using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogBrchPrevDayOrd6SrsHistory
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public int? ServerId { get; set; }

    public decimal? PrevBuyDayOrder { get; set; }

    public decimal? PrevSellDayOrder { get; set; }

    public decimal? PrevNettDayOrder { get; set; }

    public decimal? PrevTotalDayOrder { get; set; }

    public int? TransactionNo { get; set; }
}
