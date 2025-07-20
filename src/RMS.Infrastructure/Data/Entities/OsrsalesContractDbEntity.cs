using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OsrsalesContractDbEntity
{
    public string ClientCode { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public DateTime TradeDate { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public DateTime SettleDate { get; set; }

    public string StockCode { get; set; } = null!;

    public string? StockName { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Osqty { get; set; }

    public decimal Osamt { get; set; }

    public decimal? OrgQty { get; set; }

    public decimal? OrgAmt { get; set; }

    public decimal? Dffee { get; set; }

    public string? XchgCode { get; set; }

    public string? TradeCcy { get; set; }

    public string? SettleCcy { get; set; }

    public decimal? NetAmountMyr { get; set; }
}
