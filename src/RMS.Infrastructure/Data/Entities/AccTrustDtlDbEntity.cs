using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccTrustDtlDbEntity
{
    public DateTime DtCreateDate { get; set; }

    public string SClientCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public decimal? DTrustAmt { get; set; }

    public string? SRecType { get; set; }

    public decimal? DTransTrustAmt { get; set; }

    public decimal? DTransEarmarkAmt { get; set; }

    public string? SPymtRefNo { get; set; }
}
