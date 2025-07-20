using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class BuyingInDefaulter
{
    public int TransId { get; set; }

    public DateTime? TransactDate { get; set; }

    public string? OrderBic { get; set; }

    public int TradeNo { get; set; }

    public DateTime TradeDate { get; set; }

    public decimal? TradedQty { get; set; }

    public decimal? TradedPrice { get; set; }

    public decimal? BuyingInMarketFee { get; set; }

    public int? TradeNoBis { get; set; }

    public string? InstruLongCode { get; set; }

    public string? OrderBookPositionAccId { get; set; }

    public string? OrderTraderId { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public string? LastUpdatedBy { get; set; }
}
