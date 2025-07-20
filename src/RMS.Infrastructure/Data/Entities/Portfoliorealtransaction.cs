using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Portfoliorealtransaction
{
    public int SeqNo { get; set; }

    public string ClientCode { get; set; } = null!;

    public string? BranchCode { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? StockCode { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? TransactionCost { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? TransactionType { get; set; }

    public string? OrderNo { get; set; }

    public string? TerminalId { get; set; }
}
