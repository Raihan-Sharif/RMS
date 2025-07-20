using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderConfirmEx
{
    public DateTime Timestamp { get; set; }

    public int? TransactionNo { get; set; }

    public string OrderNo { get; set; } = null!;

    public string ClOrdId { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string? StockCode { get; set; }

    public string? LastStatus { get; set; }

    public int? Quantity { get; set; }

    public int? MatchedQty { get; set; }

    public decimal? MatchedAmount { get; set; }

    public decimal? Price { get; set; }

    public decimal TotalMatchedPrice { get; set; }

    public string? OrderType { get; set; }

    public string? Modality { get; set; }

    public string? ConfirmFlag { get; set; }

    public int? UpliftedFlag { get; set; }

    public string? XchgCode { get; set; }

    public string? OriginalClntCode { get; set; }

    public int SequenceNo { get; set; }

    public int? UseDlrLmt { get; set; }

    public string? OriginalCoBrchCode { get; set; }

    public string? Trsno { get; set; }
}
