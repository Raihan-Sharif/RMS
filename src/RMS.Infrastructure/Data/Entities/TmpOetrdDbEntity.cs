using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TmpOetrdDbEntity
{
    public DateTime? LastUpdatedTime { get; set; }

    public string? ClientUserId { get; set; }

    public string OrderId { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public string? Source { get; set; }

    public string? ClientCode { get; set; }

    public string? OrderType { get; set; }

    public string? StockCode { get; set; }

    public int? OriginalQuantity { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? MatchedQty { get; set; }

    public double? AvgMatchedPrc { get; set; }

    public double? MatchedAmount { get; set; }

    public string? UserId { get; set; }

    public string? OrdFlag { get; set; }

    public int? LotSize { get; set; }

    public byte? PayType { get; set; }

    public string? Currency { get; set; }

    public int? SequenceNo { get; set; }

    public int? OrderSeqNo { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? LastStatus { get; set; }

    public int? TerminalId { get; set; }

    public string? OrderNo { get; set; }

    public int? Uplifted { get; set; }

    public string? ConfirmFlag { get; set; }

    public string? OrderTypeDesc { get; set; }

    public string? StkSname { get; set; }

    public string? StkSectCode { get; set; }

    public decimal? StkClosePrice { get; set; }

    public string? CompanyCode { get; set; }
}
