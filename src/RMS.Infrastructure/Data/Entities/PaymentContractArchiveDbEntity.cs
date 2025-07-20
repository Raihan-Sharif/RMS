using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PaymentContractArchiveDbEntity
{
    public string PaymentRefNo { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public DateTime? LastUpdateDate { get; set; }

    public string? PaymentType { get; set; }

    public decimal? PymtAmt { get; set; }

    public string? PymtInd { get; set; }
}
