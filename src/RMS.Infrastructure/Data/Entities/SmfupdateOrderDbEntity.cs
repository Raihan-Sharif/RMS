using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class SmfupdateOrderDbEntity
{
    public int RecordNo { get; set; }

    public DateTime? Timestamp { get; set; }

    public string ClientCode { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string OrderAction { get; set; } = null!;

    public string? OrderSrc { get; set; }

    public int? OrderSeqNo { get; set; }

    public int SequenceNo { get; set; }

    public string StockCode { get; set; } = null!;

    public string OrderType { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal? MatchedPrice { get; set; }

    public int? MatchedQty { get; set; }

    public string? LastStatus { get; set; }

    public string? OrderNo { get; set; }

    public int? ReleaseType { get; set; }

    public int? ProcessedFlag { get; set; }

    public int? Retry { get; set; }
}
