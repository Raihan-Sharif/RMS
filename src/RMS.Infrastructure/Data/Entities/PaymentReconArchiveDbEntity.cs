using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PaymentReconArchiveDbEntity
{
    public DateTime? PaymentDate { get; set; }

    public string? CompanyId { get; set; }

    public string PaymentRefNo { get; set; } = null!;

    public string? Cdsno { get; set; }

    public string? PayType { get; set; }

    public decimal? Amt { get; set; }

    public string? Status { get; set; }

    public string? BankRef { get; set; }

    public string BankType { get; set; } = null!;

    public string? MbbacctNo { get; set; }

    public DateTime? EffectDate { get; set; }

    public string? Mbbsource { get; set; }

    public string? MbbtransCode { get; set; }

    public string? MbbtransDesc { get; set; }

    public string? MbbmultiCode { get; set; }

    public string? MbbclntCode { get; set; }

    public string? MbbbrchCode { get; set; }
}
