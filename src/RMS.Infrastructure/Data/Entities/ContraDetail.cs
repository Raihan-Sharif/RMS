using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ContraDetail
{
    public string ContraNo { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public DateTime ContractDate { get; set; }

    public string ContractNo { get; set; } = null!;

    public string ContractType { get; set; } = null!;

    public int ContraQuantity { get; set; }

    public int OriginalContractQuantity { get; set; }

    public string StockCode { get; set; } = null!;

    public decimal NetContractAmount { get; set; }

    public decimal NetContraAmount { get; set; }

    public string? XchgCode { get; set; }

    public string? StkSname { get; set; }
}
