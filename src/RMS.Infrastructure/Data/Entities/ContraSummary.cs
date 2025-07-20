using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ContraSummary
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string ContraNo { get; set; } = null!;

    public string ContraType { get; set; } = null!;

    public DateTime ContraDate { get; set; }

    public string StockCode { get; set; } = null!;

    public int ContraQuantity { get; set; }

    public decimal ContraAmount { get; set; }

    public decimal ContraSettleValue { get; set; }

    public decimal ContraSettlePending { get; set; }

    public DateTime ContraDueDate { get; set; }

    public decimal TotalPurchase { get; set; }

    public decimal TotalSold { get; set; }

    public int? Age { get; set; }

    public decimal ServiceCharge { get; set; }

    public string Currency { get; set; } = null!;

    public decimal? InterestAmount { get; set; }

    public string? XchgCode { get; set; }

    public string? StkSname { get; set; }
}
