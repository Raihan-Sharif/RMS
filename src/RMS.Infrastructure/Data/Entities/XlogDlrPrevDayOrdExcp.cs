using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogDlrPrevDayOrdExcp
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? DlrCode { get; set; }

    public decimal? PrevBuyDayOrder { get; set; }

    public decimal? PrevSellDayOrder { get; set; }

    public decimal? PrevNettDayOrder { get; set; }

    public decimal? PrevTotalDayOrder { get; set; }

    public int? TransactionNo { get; set; }

    public string? Remarks { get; set; }
}
