using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class N2nclientTradeInfoDbEntity
{
    public string? SeqNo { get; set; }

    public DateTime? TransDate { get; set; }

    public string? ClientCode { get; set; }

    public string? DealerId { get; set; }

    public string? Cdsno { get; set; }

    public string? XchgCode { get; set; }

    public string? StockCode { get; set; }

    public string? BuySellCode { get; set; }

    public string? MatchedQty { get; set; }

    public string? MatchedPrice { get; set; }

    public string? OrderNo { get; set; }

    public string? BranchId { get; set; }

    public string? ExeRefId { get; set; }

    public string? SubmittedBy { get; set; }
}
