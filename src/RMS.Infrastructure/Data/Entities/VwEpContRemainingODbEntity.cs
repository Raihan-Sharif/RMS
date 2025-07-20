using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwEpContRemainingODbEntity
{
    public string ClientCode { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public string? TradeDate { get; set; }

    public string ContractType { get; set; } = null!;

    public decimal? RemainOsqty { get; set; }

    public decimal? RemainOsamount { get; set; }

    public decimal? RemainOsinterest { get; set; }

    public decimal? RemainOstotal { get; set; }
}
