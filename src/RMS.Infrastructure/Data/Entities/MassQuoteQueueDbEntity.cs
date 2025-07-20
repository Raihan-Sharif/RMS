using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MassQuoteQueueDbEntity
{
    public int MassQuoteId { get; set; }

    public int OrderSeqNo { get; set; }

    public DateTime PlaceOrderTime { get; set; }

    public string UserId { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string? StockCode { get; set; }

    public string StockName { get; set; } = null!;

    public string OrderType { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int LotSize { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? Session { get; set; }

    public int? SeqNo { get; set; }

    public string? OrderNo { get; set; }

    public string? OrdFlag { get; set; }

    public string? OrderSrc { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? OrderEntryType { get; set; }

    public string? Modality { get; set; }

    public int? OrderValidity { get; set; }

    public string? Currency { get; set; }

    public int? Earmarked { get; set; }

    public double? CurrencyRate { get; set; }

    public byte? Uplifted { get; set; }

    public decimal? EarmarkPrice { get; set; }

    public string? SenderType { get; set; }

    public int? UseDlrLmt { get; set; }

    public int? CuseDlrLmt { get; set; }

    public string? XchgCode { get; set; }
}
