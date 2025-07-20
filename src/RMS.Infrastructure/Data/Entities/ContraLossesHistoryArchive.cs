using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ContraLossesHistoryArchive
{
    public string ClientCode { get; set; } = null!;

    public string ContraNo { get; set; } = null!;

    public DateTime ContraDate { get; set; }

    public DateTime SettleDate { get; set; }

    public string StockCode { get; set; } = null!;

    public string StockName { get; set; } = null!;

    public decimal ContraQty { get; set; }

    public decimal ContraAmt { get; set; }

    public decimal InterestAmt { get; set; }

    public decimal? Dffee { get; set; }

    public string? PaymentRefNo { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public decimal? Brokerage { get; set; }

    public decimal? StampDuty { get; set; }

    public decimal? ClearingFee { get; set; }

    public string? DocumentType { get; set; }

    public DateTime LastUpdateDate { get; set; }

    public string? XchgCode { get; set; }

    public string? TradeCcy { get; set; }

    public string? SettleCcy { get; set; }

    public decimal OrgQty { get; set; }

    public decimal OrgAmt { get; set; }
}
