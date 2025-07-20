using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class IntraDayOrdCotrDbEntity
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public decimal? BuyReinstateAmt { get; set; }

    public decimal? SellReinstateAmt { get; set; }

    public decimal? CotrLossAmt { get; set; }

    public decimal? CotrGainAmt { get; set; }
}
