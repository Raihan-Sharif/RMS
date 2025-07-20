using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntLimitDtlHistory
{
    public long RefNo { get; set; }

    public DateTime OrderDate { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public int SequenceNo { get; set; }

    public string? OrderNo { get; set; }

    public string? StkCode { get; set; }

    public string? OrderType { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public string? OrderStatus { get; set; }

    public decimal? AvailableBuyLimit { get; set; }

    public decimal? AvailableSellLimit { get; set; }

    public decimal? ReinstatedBuyAmt { get; set; }

    public decimal? ReinstatedSellAmt { get; set; }
}
