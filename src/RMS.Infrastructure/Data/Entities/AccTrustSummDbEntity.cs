using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccTrustSummDbEntity
{
    public string SClientCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public decimal? DTrustAmt { get; set; }

    public decimal? DTrustAmtEarmark { get; set; }

    public DateTime DtLastUpdateDate { get; set; }
}
