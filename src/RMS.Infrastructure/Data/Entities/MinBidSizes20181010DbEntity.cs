using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MinBidSizes20181010DbEntity
{
    public string XchgCode { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public int PriceBidType { get; set; }

    public decimal Price { get; set; }

    public decimal? BidSize { get; set; }
}
