using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwAccContDtlDbEntity
{
    public string ClientCode { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public string? TradeDate { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public string ContractType { get; set; } = null!;

    public DateTime SettleDate { get; set; }

    public string StockCode { get; set; } = null!;

    public string? StockName { get; set; }

    public decimal? QtyOriginal { get; set; }

    public decimal Osqty { get; set; }

    public string TransactionType { get; set; } = null!;

    public decimal? TradedPrice { get; set; }

    public string? TradedCurrency { get; set; }

    public string? SettlementCurrency { get; set; }

    public decimal? AmountOriginal { get; set; }

    public decimal Osamt { get; set; }

    public decimal Osintest { get; set; }

    public decimal? Osttl { get; set; }

    public string? XchgCode { get; set; }

    public string? PaymentRefNo { get; set; }

    public decimal? NetAmountMyr { get; set; }
}
