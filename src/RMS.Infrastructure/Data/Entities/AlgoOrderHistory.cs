using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AlgoOrderHistory
{
    public int OrderSeqNo { get; set; }

    public DateTime PlaceOrderTime { get; set; }

    public string? UserId { get; set; }

    public string? BranchCode { get; set; }

    public string? ClientCode { get; set; }

    public string? StockCode { get; set; }

    public string? OrderType { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public int? MinQty { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? OrderFlag { get; set; }

    public string? OrderSrc { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? OrderEntryType { get; set; }

    public string? Modality { get; set; }

    public decimal? StopLimitPrice { get; set; }

    public int? OrderValidity { get; set; }

    public string? Currency { get; set; }

    public int? Earmarked { get; set; }

    public double? CurrencyRate { get; set; }

    public byte? Uplifted { get; set; }

    public int? DisplayQty { get; set; }

    public decimal? EarmarkPrice { get; set; }

    public string? Remarks { get; set; }

    public string? XchgCode { get; set; }

    public decimal? TrailingAmt { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? Aggressiveness { get; set; }

    public int? ExecutedQty { get; set; }

    public int? CareOrderSeqNo { get; set; }
}
