using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderQueue
{
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

    public int? MinQty { get; set; }

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

    public decimal? StopLimitPrice { get; set; }

    public int? OrderValidity { get; set; }

    public byte? PayType { get; set; }

    public string? BasketName { get; set; }

    public string? CounterDealer { get; set; }

    public string? CounterBranch { get; set; }

    public string? CounterBroker { get; set; }

    public int? SellerQty { get; set; }

    public decimal? SellerPrice { get; set; }

    public string? SellerClientCode { get; set; }

    public string? Currency { get; set; }

    public int? PreOpenFlag { get; set; }

    public int? Earmarked { get; set; }

    public int? OriginalSeqNo { get; set; }

    public DateTime? OriginalOrderDate { get; set; }

    public int? SellerSeqNo { get; set; }

    public byte? ForceKey { get; set; }

    public double? CurrencyRate { get; set; }

    public byte? Uplifted { get; set; }

    public string? SellerBranchCode { get; set; }

    public string AmendStatus { get; set; } = null!;

    public int? TriggerParameter { get; set; }

    public int? TriggerSession { get; set; }

    public int? DisplayQty { get; set; }

    public decimal? EarmarkPrice { get; set; }

    public string? Remarks { get; set; }

    public string? SenderType { get; set; }

    public int? RemainingQty { get; set; }

    public int? UseDlrLmt { get; set; }

    public int? CuseDlrLmt { get; set; }

    public string? XchgCode { get; set; }

    public decimal? SellerEarmarkPrice { get; set; }

    public string? SettlementCurrency { get; set; }

    public int? LinkOrderSeqNo { get; set; }

    public string? StockBoard { get; set; }

    public string? PrivateOrder { get; set; }
}
