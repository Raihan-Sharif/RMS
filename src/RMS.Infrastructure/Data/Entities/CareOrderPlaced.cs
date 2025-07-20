using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CareOrderPlaced
{
    public DateTime Timestamp { get; set; }

    public int SequenceNo { get; set; }

    public string? TokenNo { get; set; }

    public string? RouteNo { get; set; }

    public string? RouteBy { get; set; }

    public string? RouteStatus { get; set; }

    public string? StockCode { get; set; }

    public string StockName { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string OrderType { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public int MatchedQty { get; set; }

    public decimal AvgPrice { get; set; }

    public decimal? MatchedAmount { get; set; }

    public string? LastStatus { get; set; }

    public string? OrderId { get; set; }
}
