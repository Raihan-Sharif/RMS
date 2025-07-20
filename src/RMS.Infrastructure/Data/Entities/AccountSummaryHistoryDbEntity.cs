using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccountSummaryHistoryDbEntity
{
    public string ClientCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public decimal? TrustAccout { get; set; }

    public decimal? ForcedSelling { get; set; }

    public decimal? DueSettlement { get; set; }

    public decimal? SalesProcceedsDue { get; set; }

    public decimal? RpurchaseContract { get; set; }

    public decimal? RsalesContract { get; set; }

    public decimal? ContraGains { get; set; }

    public decimal? ContraLosses { get; set; }

    public decimal? OthersOsbal { get; set; }

    public string? PaymentRefNo { get; set; }

    public decimal? Bills { get; set; }

    public decimal? Miscellaneous { get; set; }

    public DateTime LastUpdateDate { get; set; }

    public decimal? LocalLimit { get; set; }

    public decimal? ForeignLimit { get; set; }

    public decimal? LocalPct { get; set; }

    public decimal? ForeignPct { get; set; }

    public string? Currency { get; set; }

    public decimal? TrustAmtEarmark { get; set; }

    public decimal? AvailableTrustAmt { get; set; }

    public decimal? FloatAmt { get; set; }
}
