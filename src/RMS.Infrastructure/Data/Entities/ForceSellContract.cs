using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ForceSellContract
{
    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public DateTime ContractDate { get; set; }

    public string ContractType { get; set; } = null!;

    public string StockCode { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal GrossAmount { get; set; }

    public decimal Value { get; set; }

    public decimal Rate { get; set; }

    public decimal Brokerage { get; set; }

    public decimal StampDuty { get; set; }

    public decimal ClearingFee { get; set; }

    public decimal ServiceCharge { get; set; }

    public DateTime ContractDueDate { get; set; }

    public string Currency { get; set; } = null!;

    public decimal ContractSettleAmount { get; set; }

    public int ContractSettleQty { get; set; }

    public int? Age { get; set; }
}
