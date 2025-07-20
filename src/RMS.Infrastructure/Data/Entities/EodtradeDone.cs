using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EodtradeDone
{
    public string? UsrId { get; set; }

    public string? BranchCode { get; set; }

    public string? ClientCode { get; set; }

    public string? StockCode { get; set; }

    public string? OrderType { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public int? MatchedQty { get; set; }

    public decimal? MatchedAmount { get; set; }

    public DateTime? PlaceOrderTime { get; set; }

    public string? Currency { get; set; }

    public double? CurrencyRate { get; set; }

    public bool? IsFirstTradeDone { get; set; }

    public DateTime? LogDate { get; set; }

    public string? LogUsr { get; set; }
}
