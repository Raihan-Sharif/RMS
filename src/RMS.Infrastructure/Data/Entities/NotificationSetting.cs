using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class NotificationSetting
{
    public string UsrId { get; set; } = null!;

    public string DevId { get; set; } = null!;

    public bool? TradeDone { get; set; }

    public bool? PortfolioAnn { get; set; }

    public bool? Ipo { get; set; }

    public bool? GeneralAnn { get; set; }

    public bool? PriceAlert { get; set; }

    public bool? Order { get; set; }
}
