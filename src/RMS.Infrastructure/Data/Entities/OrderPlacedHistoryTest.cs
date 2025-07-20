using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderPlacedHistoryTest
{
    public DateTime Timestamp { get; set; }

    public int SequenceNo { get; set; }

    public string UserId { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string StockCode { get; set; } = null!;

    public string StockName { get; set; } = null!;

    public string OrderType { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int LotSize { get; set; }

    public int? Session { get; set; }

    public string? ClOrdId { get; set; }

    public string? RemisierId { get; set; }

    public string OrderNo { get; set; } = null!;

    public int? TerminalId { get; set; }

    public int MatchedQty { get; set; }

    public decimal? MatchedAmount { get; set; }

    public int ReducedQty { get; set; }

    public string? LastStatus { get; set; }

    public byte? Uplifted { get; set; }

    public int OldSeqNo { get; set; }

    public decimal TotalMatchedPrice { get; set; }

    public int TransactionNo { get; set; }

    public byte AutoSend { get; set; }

    public string? Validation { get; set; }

    public string? OrderSrc { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public byte? Cfind { get; set; }

    public DateTime? PlaceOrderTime { get; set; }

    public string? OrderEntryType { get; set; }

    public string? Modality { get; set; }

    public decimal? StopLimitPrice { get; set; }

    public int? OrderValidity { get; set; }

    public string? BasketName { get; set; }

    public int? MinQty { get; set; }

    public string? CounterDealer { get; set; }

    public string? CounterBranch { get; set; }

    public string? CounterBroker { get; set; }

    public int? SellerQty { get; set; }

    public decimal? SellerPrice { get; set; }

    public string? SellerClientCode { get; set; }

    public string? Currency { get; set; }

    public int? PreOpenFlag { get; set; }

    public string? AmendId { get; set; }

    public string? AmendClientCode { get; set; }

    public int? OriginalSeqNo { get; set; }

    public DateTime? OriginalOrderDate { get; set; }

    public int? ExchOrderSeqNo { get; set; }

    public int? SellerSeqNo { get; set; }

    public byte? ForceKey { get; set; }

    public double? CurrencyRate { get; set; }

    public DateTime? FirstMatchTime { get; set; }

    public string? FirstMatchOrderNo { get; set; }

    public string? SellerBranchCode { get; set; }

    public int? TriggerParameter { get; set; }

    public int? TriggerSession { get; set; }

    public int? DisplayQty { get; set; }

    public decimal? EarmarkPrice { get; set; }

    public string? AmendBranchCode { get; set; }

    public string? Remarks { get; set; }

    public string? OriginalClntCode { get; set; }

    public int? OriginalQuantity { get; set; }

    public decimal? OriginalPrice { get; set; }

    public int? WsorderNo { get; set; }

    public string? SenderType { get; set; }

    public int? UseDlrLmt { get; set; }

    public int? CuseDlrLmt { get; set; }

    public string? OrderSrcRemarks { get; set; }

    public int TimestampFmt { get; set; }

    public string? XchgCode { get; set; }

    public int? OrderSeqNo { get; set; }
}
