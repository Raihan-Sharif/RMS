using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DbtdeclarationDbEntity
{
    public DateTime Timestamp { get; set; }

    public string? MemberRef { get; set; }

    public string Tcsref { get; set; } = null!;

    public string? PrevInd { get; set; }

    public string? MembCode { get; set; }

    public string? PrincipalCode { get; set; }

    public string? OrderType { get; set; }

    public int? Quantity { get; set; }

    public string? StockCode { get; set; }

    public decimal? Price { get; set; }

    public string? CounterMembCode { get; set; }

    public string? CounterPrincCode { get; set; }

    public DateTime? DeclareTime { get; set; }

    public string? OperationInd { get; set; }

    public int? StartVwap { get; set; }

    public int? EndVwap { get; set; }

    public string? TraderId { get; set; }

    public string? CounterTraderId { get; set; }

    public string? LastStatus { get; set; }

    public int? StatusSeqNo { get; set; }

    public string? ExecId { get; set; }
}
