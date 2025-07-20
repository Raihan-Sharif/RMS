using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PfclntActSumDbEntity
{
    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string? AccountType { get; set; }

    public string? DealerId { get; set; }

    public decimal? CashBal { get; set; }

    public decimal? TtlColCash { get; set; }

    public decimal? CreditLimit { get; set; }

    public decimal? ApproveMof { get; set; }

    public decimal? CurrentMof { get; set; }

    public decimal? TopUpCase { get; set; }

    public decimal? TopUpShare { get; set; }

    public decimal? SellDown { get; set; }

    public decimal? MrgnCallRatio { get; set; }

    public decimal? ForceSellRatio { get; set; }

    public decimal? WithdrawalRatio { get; set; }

    public decimal? OpenLimitBal { get; set; }

    public decimal? Osbalance { get; set; }

    public decimal? Interest { get; set; }

    public decimal? CurrInterestrate { get; set; }

    public decimal? OverdueAmount { get; set; }

    public DateTime? DtLastUpdate { get; set; }
}
