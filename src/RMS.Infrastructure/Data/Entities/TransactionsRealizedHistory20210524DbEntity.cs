using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TransactionsRealizedHistory20210524DbEntity
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string? CoBrchCode { get; set; }

    public string ClntCode { get; set; } = null!;

    public string? XchgCode { get; set; }

    public string? StkCode { get; set; }

    public string? StkName { get; set; }

    public string? CurcyCode { get; set; }

    public decimal? MatchedPrice { get; set; }

    public int? MatchedQty { get; set; }

    public decimal? AvgBuyPrice { get; set; }

    public decimal? RealizedPl { get; set; }

    public int? SeqNo { get; set; }

    public string? ClOrdId { get; set; }

    public decimal CurcyRate { get; set; }
}
